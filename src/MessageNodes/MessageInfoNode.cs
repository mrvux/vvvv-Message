using System.ComponentModel.Composition;
using System.Globalization;
using System.Text;
using VVVV.Core.Logging;
using VVVV.Packs.Messaging.Core;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Pack.Messaging.Nodes
{
    #region PluginInfo
    [PluginInfo(Name = "Info", Category = "Message", Help = "Help to Debug Messages", Tags = "TTY", Author = "velcrome")]
    #endregion PluginInfo
    public class MessageInfoNode : IPluginEvaluate
    {
#pragma warning disable 649, 169

        [Input("Input")] 
        private IDiffSpread<Message> FInput;

        //[Input("Output Bin Length", IsSingle = true, IsToggle = true)]
        //private IDiffSpread<bool> FLength;

        [Input("Print Message", IsBang = true)]
        private IDiffSpread<bool> FPrint;

        [Output("Address", AutoFlush = false)] private ISpread<string> FAddress;

        [Output("Timestamp", AutoFlush = false)] private ISpread<string> FTimeStamp;

        [Output("Output", AutoFlush = false)] private ISpread<string> FOutput;

        [Output("Configuration", AutoFlush = false)] private ISpread<string> FConfigOut;

        [Import()] protected ILogger FLogger;

#pragma warning restore

        public void Evaluate(int SpreadMax)
        {
            if (FInput.SliceCount <= 0 || FInput[0] == null) SpreadMax = 0;
            else SpreadMax = FInput.SliceCount;

            if (!FInput.IsChanged) return;

            FOutput.SliceCount = SpreadMax;
            FTimeStamp.SliceCount = SpreadMax;
            FAddress.SliceCount = SpreadMax;
            FConfigOut.SliceCount = SpreadMax;

            for (int i = 0; i < SpreadMax; i++)
            {
                Message m = FInput[i];
                FOutput[i] = m.ToString();
                FAddress[i] = m.Address;
                FTimeStamp[i] = m.TimeStamp.UniversalTime.ToString(CultureInfo.InvariantCulture);
//                FConfigOut[i] = FInput[i].GetConfig(FLength[0]);
                FConfigOut[i] = FInput[i].GetConfig();

                if (FPrint[i])
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("== Message " + i + " ==");
                    sb.AppendLine();

                    sb.AppendLine(FInput[i].ToString());
                    sb.Append("====\n");
                    FLogger.Log(LogType.Debug, sb.ToString());
                }
            }
            FAddress.Flush();
            FTimeStamp.Flush();
            FOutput.Flush();
            FConfigOut.Flush();
        }
    }
}