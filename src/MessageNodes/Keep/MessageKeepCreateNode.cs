using System;
using System.Collections.Generic;
using System.Linq;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils;

namespace VVVV.Packs.Messaging.Nodes
{
    #region PluginInfo
    [PluginInfo(Name = "Create", AutoEvaluate = true, Category = "Message.Keep", Help = "Joins a permanent Message from custom dynamic pins", Version = "Formular", Tags = "Dynamic, Bin", Author = "velcrome")]
    #endregion PluginInfo
    public class MessageKeepCreateNode : DynamicPinsNode
    {
#pragma warning disable 649, 169
        [Input("Topic", DefaultString = "State", Order = 1)]
        public IDiffSpread<string> FTopic;

        [Input("AutoSense", Order = -1, IsSingle = true, IsToggle = true, DefaultBoolean = false, Visibility = PinVisibility.OnlyInspector)]
        IDiffSpread<bool> FDetectChange;

        [Input("Update", IsToggle = true, Order = int.MaxValue-1, DefaultBoolean = true)]
        IDiffSpread<bool> FUpdate;

        [Input("Count", IsSingle = true, DefaultValue = 1, Order = int.MaxValue)]
        public IDiffSpread<int> FSpreadCount;

        [Output("Output", AutoFlush = false)]
        public Pin<Message> FOutput;

        [Output("Changed Message", Order = int.MaxValue - 2, AutoFlush = false, Visibility = PinVisibility.Hidden)]
        public Pin<int> FChangeIndexOut;

        [Output("Message Diff", Order = int.MaxValue - 1, AutoFlush = false)]
        public Pin<Message> FChangeOut;

        [Output("Internal Count", Order = int.MaxValue, AutoFlush = false)]
        public ISpread<int> FCountOut;
#pragma warning restore

        protected MessageKeep Keep = new MessageKeep(false); // same as in AbstractStorageNode. 

        protected virtual bool CheckReset()
        {
            if (FConfig.IsChanged)
//                if ((!FReset.IsAnyInvalid() && FReset[0]) || FConfig.IsChanged)
            {
                Keep.Clear();
                return true;
            }
            return false;

        }

        protected virtual bool UpKeep(bool force = false)
        {
            if (!force && !Keep.IsChanged)
            {
                if (!Keep.QuickMode && FChangeIndexOut.SliceCount != 0)
                {
                    FChangeIndexOut.SliceCount = 0;
                    FChangeIndexOut.Flush();
                    FChangeOut.SliceCount = 0;
                    FChangeOut.Flush();
                }
                return false;
            }

            if (Keep.QuickMode)
            {
                Keep.Sync();
            }
            else
            {
                IEnumerable<Message> changes;
                IEnumerable<int> indexes;
                changes = Keep.Sync(out indexes);

                FChangeIndexOut.SliceCount = 0;
                FChangeIndexOut.AssignFrom(indexes);
                FChangeIndexOut.Flush();

                FChangeOut.SliceCount = 0;
                FChangeOut.AssignFrom(changes);
                FChangeOut.Flush();
            }

            FOutput.SliceCount = 0;
            FOutput.AssignFrom(Keep);
            FOutput.Flush();

            FCountOut[0] = Keep.Count;
            FCountOut.Flush();

            return true;
        }



        protected override IOAttribute DefinePin(FormularFieldDescriptor configuration)
        {
            var attr = new InputAttribute(configuration.Name); 
            attr.BinVisibility = PinVisibility.Hidden;
            attr.BinSize = configuration.DefaultSize;
            attr.Order = DynPinCount;
            attr.BinOrder = DynPinCount + 1;
            attr.CheckIfChanged = true;

            return attr;
        }

        public override void Evaluate(int SpreadMax)
        {
            SpreadMax = FSpreadCount.IsAnyInvalid() || FTopic.IsAnyInvalid() ? 0 : FSpreadCount[0];
            SpreadMax = Math.Max(SpreadMax, 0); // safeguard against negative binsizes

//          Reset?
            var anyUpdate = CheckReset();

            var forceUpdate = !FDetectChange[0] || FDetectChange.IsChanged;
          
            var newData = FPins.Any(x => x.Value.ToISpread().IsChanged); // changed pins
            newData |= forceUpdate; // if update is forced, then predent it is new Data
            
            var newTopic = FTopic.IsChanged;
            newTopic |= forceUpdate; // if update is forced, then pretend it is a new Topic

//          remove superfluous entries
            if (SpreadMax < Keep.Count) 
            {
                anyUpdate = true;
                Keep.RemoveRange(SpreadMax, Keep.Count - SpreadMax);
            }

//          add new entries
            for (int i = Keep.Count; i < SpreadMax; i++)
            {
                anyUpdate = true; // new entry in Keep will require data

                newData = true;
                newTopic = true;

                var message = new Message(Formular);
                message.Topic = FTopic[i];
                Keep.Add(message);
            }

//          check update pin
            anyUpdate |= FUpdate.Any(x => x);

            if (anyUpdate && (newData || newTopic))
            {
                // ...and start filling messages
                int messageIndex = 0;
                foreach (var message in Keep)
                {
                    // only copy, when Update is true for this message
                    if (newData && FUpdate[messageIndex] && CopyFromPins(message, messageIndex, !forceUpdate)) 
                        anyUpdate = true;
                    
                    if (newTopic && FUpdate[messageIndex] && message.Topic != FTopic[messageIndex]) 
                    {
                        message.Topic = FTopic[messageIndex];
                        anyUpdate = true;
                    }
                    messageIndex++;
                }
            }

            if (Keep.IsChanged)
            {
                UpKeep(anyUpdate);
            }
            else  // no change, so make sure, none is reported
            {
                if (FChangeOut.SliceCount > 0)
                {
                    FChangeOut.SliceCount = 0;
                    FChangeIndexOut.SliceCount = 0;
                }
            }

        }



    }
}