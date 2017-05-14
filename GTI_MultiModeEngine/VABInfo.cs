using System;
using System.Text;
using UnityEngine;

namespace GTI
{
    partial class GTI_MultiModeEngineOLD : PartModule
    {
        #region VAB Information
        public override string GetInfo()
        {
            StringBuilder Info = new StringBuilder();
            try
            {
                //we need to run the InitializeSettings here, because the OnStart does not run before this.
                //initializeSettings(false);

                Info.AppendLine("<color=yellow>Engine Modes Available:</color>");

                //for (int i = 0; i < engineModeList.Count; i++)
                //{
                    Info.Append("<b><color=yellow>Engine Modes: </color></b>");
                    Info.Append(GUIengineModeNames);
                    Info.AppendLine();

                    Info.Append("<b><color=yellow>Propellants: </color></b>");
                    Info.Append(propellantNames);
                    Info.AppendLine();

                    Info.Append("<b><color=yellow>Propellant ratios: </color></b>");
                    Info.Append(propellantRatios);
                    Info.AppendLine();

                    Info.Append("<b><color=yellow>Max thrust: </color></b>");
                    Info.Append(maxThrust);
                    Info.AppendLine();
                //}
                
                Info.AppendLine("\nIn Flight switching is <color=yellow>" + (availableInFlight ? "available" : "not available") + "</color>");

                //str.AppendFormat("Maximal force: {0:0.0}iN\n", maxGeneratorForce);
                //str.AppendFormat("Maximal charge time: {0:0.0}s\n\n", maxChargeTime);
                //str.AppendFormat("Requires\n");
                //str.AppendFormat("- Electric charge: {0:0.00}/s\n\n", requiredElectricalCharge);
                //str.Append("Navigational computer\n");
                //str.Append("- Required force\n");
                //str.Append("- Success probability\n");

                //return "GTI Multi Mode Engine";
                return Info.ToString();
            }
            catch (Exception e)
            {
                Debug.LogError("GTI_MultiModeEngine GetInfo Error " + e.Message);
                throw;
            }
        }
        #endregion
    }
}
