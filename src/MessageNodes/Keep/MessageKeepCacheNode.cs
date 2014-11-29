﻿#region usings
using System;
using System.Collections.Generic;
using VVVV.PluginInterfaces.V2;
using System.Linq;
using VVVV.Packs.Messaging.Core;
#endregion usings

namespace VVVV.Packs.Messaging.Nodes
{
    [PluginInfo(Name = "Cache",
        Category = "Message.Keep",
        AutoEvaluate = true,
        Help = "Stores Messages and removes them, if no change was detected for a certain time",
        Author = "velcrome")]
    public class MessageCacheNode : AbstractMessageKeepNode
    {
        #region fields & pins

        [Input("Retain Time", IsSingle = true, DefaultValue = -1.0, Order = 3)]
        public ISpread<double> FTime;

        [Output("Removed Messages", AutoFlush = false, Order = 2)]
        public ISpread<Message> FRemovedMessages;

        #endregion fields & pins

        //called when data for any output pin is requested
        public override void Evaluate(int SpreadMax)
        {
            if (FReset[0])
            {
                MessageKeep.Clear();
            }

            var changed = (
                    from message in FInput
                    where message != null
                    let keep = MatchOrInsert(message, FUseAsID[0].Name)
                    select keep
                ).ToList();


            if (FTime[0] > 0)
            {
                var validTime = Time.Time.CurrentTime() -
                                new TimeSpan(0, 0, 0, (int) Math.Floor(FTime[0]), (int) Math.Floor((FTime[0]*1000)%1000));

                var clear = (from message in MessageKeep
                            where message.TimeStamp < validTime
                            select message).ToArray();

                foreach (var m in clear)
                {
                    var index = MessageKeep.IndexOf(m);
                    MessageKeep.RemoveAt(index);
                    changed.Remove(m);
                }

                if (FRemovedMessages.SliceCount > 0 || clear.Length != 0)
                {
                    FRemovedMessages.SliceCount = 0;
                    FRemovedMessages.AssignFrom(clear);
                    FRemovedMessages.Flush();
                }
                else
                {
                    // output still empty, no need to flush
                }

            }
            
            SpreadMax = MessageKeep.Count;
            FChanged.SliceCount = FOutput.SliceCount = SpreadMax;
         
  
            for (int i = 0; i < SpreadMax;i++ )
            {
                var message = MessageKeep[i];
                FOutput[i] = message;
                FChanged[i] = changed.Contains(message);

            }
  


        }

    }


}