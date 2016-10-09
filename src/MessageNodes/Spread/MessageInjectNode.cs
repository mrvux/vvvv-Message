﻿using System.ComponentModel.Composition;
using System.Linq;
using VVVV.Core.Logging;
using VVVV.Packs.Messaging;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils;

namespace VVVV.Messaging.Nodes
{

    #region PluginInfo
    [PluginInfo(Name = "Inject", Category = "Message", Help = "When matching topics, this node injects some Modifications into existing Messages", Tags = "Message", Author = "velcrome")]
    #endregion PluginInfo
    public class MessageSiftMessageNode : IPluginEvaluate
    {
#pragma warning disable 649, 169
        [Input("Input")]
        private IDiffSpread<Message> FInput;

        [Input("Modificator Input")]
        private IDiffSpread<Message> FModifier;

        [Input("Only modify when changed")]
        private IDiffSpread<bool> FDeepInspection;

        [Output("Output", AutoFlush = false)]
        private ISpread<Message> FOutput;

        [Output("Edits", AutoFlush = false)]
        private ISpread<int> FEditCount;

        [Import()]
        protected ILogger FLogger;

#pragma warning restore

        public void Evaluate(int SpreadMax)
        {
            if (FInput.IsAnyInvalid()) SpreadMax = 0;
            else SpreadMax = FInput.SliceCount;

            if (SpreadMax == 0)
            {
                if (FOutput.SliceCount != 0)
                {
                    FOutput.FlushNil();
                    FEditCount.FlushNil();
                }
                return;
            }

            if (!FInput.IsChanged && !FModifier.IsChanged) return;

            FEditCount.SliceCount = SpreadMax;

            for (int i = 0; i < SpreadMax; i++)
            {
                FEditCount[i] = 0;
                var message = FInput[i];

                var targets =
                    from mod in FModifier
                    where mod != null
                    where mod.Topic == message.Topic
                    select mod;

                foreach(var mod in targets)
                {
                    message.InjectWith(mod, FDeepInspection[0]);
                    FEditCount[i]++;
                }
            }

            FOutput.FlushResult(FInput);
            FEditCount.Flush();
        }
    }
}
