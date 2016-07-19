﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using VVVV.PluginInterfaces.V2;
using VVVV.Utils;
using VVVV.Core.Logging;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using System.Windows.Forms;

namespace VVVV.Packs.Messaging.Nodes
{
    public abstract class TypeablePinsNode : AbstractFormularableNode, IWin32Window, ICustomQueryInterface
    {
        #region fields & pins
        protected const string Tags = "Formular";

        protected Dictionary<string, IIOContainer> FPins = new Dictionary<string, IIOContainer>();
        protected int DynPinCount = 5;

        protected bool RemovePinsFirst;

        protected FormularLayoutPanel LayoutPanel = new FormularLayoutPanel();

        #endregion fields & pins

        #region Initialisation

        [Import()]
        protected IIOFactory FIOFactory;

        public override void OnImportsSatisfied()
        {
            base.OnImportsSatisfied();

            FormularUpdate += UpdatePanelOnChange;
            FormularUpdate += TryDefinePins;

            LayoutPanel.Changed += ReadLayoutPanel;

        }

        private void ReadLayoutPanel(object sender, MessageFormular formular)
        {
            Formular = formular;
        }

        protected void UpdatePanelOnChange(object sender, MessageFormular newFormular)
        {
            // no obvious changes. 
            var oldFormular = LayoutPanel.Formular;
//            if (oldFormular.Configuration == newFormular.Configuration) return;

            // dynamic will only append. will usually even append nothing
            if (newFormular.IsDynamic)
            {
                UpdateWindow(newFormular, true);
                return;
            }
                
            // just an update
            if (oldFormular.Name == newFormular.Name)
            {
                UpdateWindow(newFormular, true);
             
            }
            // entirely new Formular
            else
            {
                UpdateWindow(newFormular, false);
            }
        }

        #endregion Initialisation

        #region pin management

        protected bool SyncPins()
        {
            bool changed = false;
            foreach (string name in FPins.Keys)
            {
                var pin = FPins[name].ToISpread();
                pin.Sync();
                if (pin.IsChanged) changed = true;
            }
            return changed;
        }

        protected bool CopyFromPins(Message message, int index, bool checkPinForChange = false)
        {
            var hasCopied = false;

            foreach (string name in FPins.Keys)
            {
                // don't change if pin data still the same
                if (!checkPinForChange || FPins[name].ToISpread().IsChanged)
                {
                    var pinSpread = FPins[name].ToISpread();
                    var type = Formular[name].Type;

                    IEnumerable bin;

                    if (pinSpread.IsAnyInvalid()) bin = Enumerable.Empty<object>();
                    else bin = pinSpread[index] as IEnumerable;

                    // don't change if pin data equals the message data
                    if (!message.Fields.Contains(name))
                    {
                        message[name] = BinFactory.New(type);
                        hasCopied = true;
                    }

                    if (!message[name].Equals(bin))
                    {
                        message.AssignFrom(name, bin, type);
                        hasCopied = true;
                    }  
                }
            }
            return hasCopied;

        }

        protected IIOContainer CreatePin(FormularFieldDescriptor field)
        {
            IOAttribute attr = SetPinAttributes(field); // each implementation of DynamicPinsNode must create its own InputAttribute or OutputAttribute (

            Type pinType = typeof(ISpread<>).MakeGenericType((typeof(ISpread<>)).MakeGenericType(field.Type)); // the Pin is always a binsized one
            var pin = FPins[field.Name] = FIOFactory.CreateIOContainer(pinType, attr);

            DynPinCount += 2; // total pincount. always add two to account for data pin and binsize pin

            return pin;
        }

        protected bool RetryConfig()
        {
            if (RemovePinsFirst)
            {
                TryDefinePins(this, Formular);
            }

            if (RemovePinsFirst)
                throw new PinConnectionException("Manually remove unneeded links first!");
            else return true;
        }

        protected bool HasLink(IIOContainer pinContainer)
        {
            try
            {
                var connected = pinContainer.GetPluginIO().IsConnected;

                foreach (var associated in pinContainer.AssociatedContainers)
                {
                    connected |= associated.GetPluginIO().IsConnected;
                }
                return connected;
            }
            catch (Exception)
            {
                string nodePath = PluginHost.GetNodePath(false);
                FLogger.Log(LogType.Error, "Failed to protect a " + this.GetType().Name + " node: " + nodePath);
                return false;
            }
        }
        /// <summary>
        /// Detects link breaking action, that would occur when disposing a now unneeded pin, or resetting its type.
        /// </summary>
        /// <param name="newFormular">Includes the changes to be done.</param>
        /// <returns>True, if no linkbreaking conflict of type or linkbreaking pin elimination</returns>
        /// <remarks>Compares the existing dynamic pins with the required fields in the given Formular</remarks>
        protected bool HasEndangeredLinks(MessageFormular newFormular)
        {
            // pin removals
            var danger = from pinName in FPins.Keys
                         where HasLink(FPins[pinName])
                         where !(    newFormular.FieldNames.Contains(pinName) 
                                  && newFormular[pinName].IsRequired)
                         select pinName;

            // type changes - removal and recreate new
            var typeDanger = from pinName in FPins.Keys
                             where HasLink(FPins[pinName]) // first frame pin will not be initialized
                             where newFormular.FieldNames.Contains(pinName)
                             where newFormular[pinName].IsRequired
                             let innerType = FPins[pinName].GetInnerMostType() // ISpread<ISpread< ??? >>
                             where newFormular[pinName].Type != innerType // rather ask for conversion? see actual pin creation for more comments
                             select pinName;

            // ignore changes to binsize for now

            return danger.Count() > 0 || typeDanger.Count() > 0;
        }

        /// <summary>
        /// Will try to apply the formular to the current pin layout
        /// </summary>
        /// <param name="newFormular"></param>
        /// <remarks>
        /// If any pin in the new formular is already present, it will preserve the pin.
        /// If any current pin is not contained in the new Formular, but is linked in the patch, it will force the node to raise exceptions every frame, until the link is manually deleted.
        /// </remarks>
        protected void TryDefinePins(object sender, MessageFormular newFormular)
        {
            if (HasEndangeredLinks(newFormular))
            {
                RemovePinsFirst = true;
                return;
            }
            else RemovePinsFirst = false;

            List<string> invalidPins = FPins.Keys.ToList();
            foreach (FormularFieldDescriptor field in newFormular.FieldDescriptors.Where(field => field.IsRequired))
            {
                if (FPins.ContainsKey(field.Name) && FPins[field.Name] != null)
                {
                    invalidPins.Remove(field.Name);

                    // same name, but types don't match
                    // todo: in fact eg float does match double from vvvv side, conversion useful for patching speed?
                    if (FPins[field.Name].GetInnerMostType() != field.Type)
                    {
                        FPins[field.Name].Dispose();
                        FPins[field.Name] = CreatePin(field);
                    }
                }
                else
                {
                    FPins[field.Name] = CreatePin(field);
                }
            }

            // cleanup
            foreach (string name in invalidPins)
            {
                FPins[name].Dispose();
                FPins.Remove(name);
            }

            // reorder - does not work right now, sdk offers only read-only access
            //var names = Formular.FieldNames.ToArray();

            //int counter = 0;
            //foreach (var name in newFormular.FieldNames)
            //{
            //    if (FPins[name] != null)
            //        FPins[name].GetPluginIO().Order = counter * 2 + 5;
            //}
        }

        #endregion pin management

        #region window management

        private void UpdateWindow(MessageFormular newFormular, bool append = false)
        {
            if (newFormular == null) return;

            var old = LayoutPanel.Formular; // retrieve copy

            if (append)
            {
                foreach (var field in newFormular.FieldDescriptors)
                {
                    // keep all potential fields checked, old and new
                    //if (old.FieldNames.Contains(field.Name))
                    //    field.IsRequired |= old[field.Name].IsRequired;

                    old[field.Name] = field; // hard overwrite 
                }

                LayoutPanel.Changed -= ReadLayoutPanel;

                old.Name = newFormular.Name;
                LayoutPanel.Formular = old; // set to appended copy

                LayoutPanel.Changed += ReadLayoutPanel;

            }
            else
            {
                var require = from field in old.FieldDescriptors
                              where field.IsRequired
                              where newFormular.FieldNames.Contains(field.Name)
                              select field.Name;

                foreach (var name in newFormular.FieldNames) newFormular[name].IsRequired = false;
                foreach (var name in require) newFormular[name].IsRequired = true;

                LayoutPanel.Changed -= ReadLayoutPanel;
                LayoutPanel.Formular = newFormular;
                LayoutPanel.Changed += ReadLayoutPanel;

            }

            LayoutPanel.CanEditFields = newFormular.IsDynamic;
        }
        #endregion

        #region Window utils

        public IntPtr Handle
        {
            get { return LayoutPanel.Handle; }
        }

        public CustomQueryInterfaceResult GetInterface(ref Guid iid, out IntPtr ppv)
        {
            if (iid.Equals(Guid.Parse("00000112-0000-0000-c000-000000000046")))
            {
                ppv = Marshal.GetComInterfaceForObject(LayoutPanel, typeof(IOleObject));
                return CustomQueryInterfaceResult.Handled;
            }
            else if (iid.Equals(Guid.Parse("458AB8A2-A1EA-4d7b-8EBE-DEE5D3D9442C")))
            {
                ppv = Marshal.GetComInterfaceForObject(LayoutPanel, typeof(IWin32Window));
                return CustomQueryInterfaceResult.Handled;
            }
            else
            {
                FLogger.Log(LogType.Debug, "missing: " + iid.ToString());
                ppv = IntPtr.Zero;
                return CustomQueryInterfaceResult.NotHandled;
            }
        }
        #endregion window utils

        #region abstract methods
        protected abstract IOAttribute SetPinAttributes(FormularFieldDescriptor config);
        #endregion abstract methods
    }

}
