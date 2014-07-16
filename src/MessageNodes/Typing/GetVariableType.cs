using VVVV.Packs.Message.Core;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Packs.Message.Nodes
{

    #region PluginInfo
    [PluginInfo(Name = "GetVariableType", AutoEvaluate = true, Category = "Message", Help = "Outputs the type of a given variable", Tags = "Dynamic, Bin, velcrome")]
    #endregion PluginInfo
    public class GetVariableTypeNode : IPluginEvaluate
    {
        #pragma warning disable 649, 169
        [Input("Type", DefaultString = "Event", IsSingle = true)]
        public ISpread<string> FType;

        [Input("Variable Name", DefaultString = "VariableName")]
        public ISpread<string> FVariableName;

        [Output("Variable Type")]
        ISpread<string> FOutput;
        #pragma warning restore

        public void Evaluate(int SpreadMax)
        {
            SpreadMax = FVariableName.SliceCount;
            FOutput.SliceCount = SpreadMax;
            var dict = TypeDictionary.Instance;
            for (int i = 0; i < SpreadMax; i++)
            {
                if (dict.ContainsKey(FType[0]))
                {
                    string[] config = dict[FType[0]].Trim().Split(',');
                    if (dict[FType[0]].Contains(FVariableName[i]))
                    {
                        foreach (string pinConfig in config)
                        {
                            string[] pinData = pinConfig.Trim().Split(' ');
                            string name = pinData[1];
                            if (name == FVariableName[i])
                            {
                                FOutput[i] = pinData[0].ToLower();
                                break;
                            }
                        }

                    }
                    else FOutput[i] = "";
                }
                else FOutput.SliceCount = 0;
            }
        }
    }
}