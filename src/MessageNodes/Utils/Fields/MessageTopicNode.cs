﻿using System.Collections.Generic;
using System.Linq;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils;

namespace VVVV.Packs.Messaging.Nodes
{
    #region PluginInfo
    [PluginInfo(Name = "Topic", AutoEvaluate = true, Category = "Message", Help = "Change the topic of Messages",
        Tags = "velcrome")]
    #endregion PluginInfo
    public class MessageTopicNode : IPluginEvaluate
    {
#pragma warning disable 649, 169
        [Input("Input")]
        IDiffSpread<Message> FInput;

        [Input("Topic")]
        IDiffSpread<string> FTopic;

        [Input("Update", IsToggle = true, Order = int.MaxValue, DefaultBoolean = true)]
        IDiffSpread<bool> FUpdate;


        [Output("Output", AutoFlush = false)]
        private ISpread<Message> FOutput;

#pragma warning restore

        public void Evaluate(int SpreadMax)
        {
            if (!FInput.IsChanged && 
                !FTopic.IsChanged &&
                !(FUpdate.IsChanged && FUpdate.Any())
            ) return;

            SpreadMax = FInput.IsAnyInvalid() ? 0 : FInput.SliceCount;
            for (int i = 0; i < SpreadMax; i++)
            {
                // check if topic change needs to occur, because doing so will mark the message as dirty
                if (FUpdate[i] && !FInput[i].Topic.Equals(FTopic[i]))
                    FInput[i].Topic = FTopic[i];
            }

            FOutput.FlushResult(FInput);

        }
    }
}