//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

using System;
using GTI.Config;
using static GTI.Config.GTIConfig;
//using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace GTI
{
    public class GTI_MultiModeHarvester : GTI_MultiMode
    {
        protected List<ModuleResourceHarvester> MRH;

        protected override void initializeSettings()
        {
            if (!_settingsInitialized)
            {
                GTIDebug.Log("GTI_MultiModeHarvester --> initializeSettings()", iDebugLevel.DebugInfo);
                MRH = part.FindModulesImplementing<ModuleResourceHarvester>();

                mode = new List<MultiMode>();
                for (int i = 0; i < MRH.Count; i++)
                {
                    mode.Add(new MultiMode()
                    {
                        moduleIndex = i,
                        ID          = i.ToString(),
                        Name        = MRH[i].ConverterName
                    });
                    GTIDebug.Log("mode[" + i + "].ID --> " + mode[i].ID, iDebugLevel.DebugInfo);
                    GTIDebug.Log("mode[" + i + "].Name --> " + mode[i].Name, iDebugLevel.DebugInfo);
                }

                //Disable converter actions, as these should be handled by the multimodeconverter module
                for (int i = 0; i < MRH.Count; i++)
                {
                    MRH[i].Actions["ToggleResourceConverterAction"].active = false;
                    MRH[i].Actions["StartResourceConverterAction"].active = false;
                    MRH[i].Actions["StopResourceConverterAction"].active = false;
                }

                //initializeGUI();

                //_settingsInitialized = true;
            }
        }

        public override void updateMultiMode(bool silentUpdate = false)
        {
            GTIDebug.Log("GTI_MultiModeConverter: updateConverter() --> Begin", iDebugLevel.High);

            FindSelectedMode();
            if (silentUpdate == false) writeScreenMessage();

            for (int i = 0; i < mode.Count; i++)
            {
                if (i == selectedMode)
                {
                    GTIDebug.Log("GTI_MultiModeConverter (" + (silentUpdate ? "silent" : "non-silent") + "): Activate Converter Module [" + mode[i].moduleIndex + "] --> " + MRH[mode[i].moduleIndex].ConverterName, iDebugLevel.High);
                    MRH[mode[i].moduleIndex].EnableModule();
                }
                else
                {
                    GTIDebug.Log("GTI_MultiModeConverter (" + (silentUpdate ? "silent" : "non-silent") + "): Deactivate Converter Module [" + mode[i].moduleIndex + "] --> " + MRH[mode[i].moduleIndex].ConverterName, iDebugLevel.High);

                    //Deactivate the converter
                    MRH[mode[i].moduleIndex].DisableModule();
                    //Stop the converter
                    MRH[mode[i].moduleIndex].StopResourceConverter();
                }
            }
            MonoUtilities.RefreshContextWindows(part);
            //Debug.Log("GTI_MultiModeConverter: updateConverter() --> Finished");
        }


        protected override void writeScreenMessage()
        {
            //string strOutInfo = string.Empty;
            StringBuilder strOutInfo = new StringBuilder();

            strOutInfo.AppendLine("Converter mode changed to " + mode[selectedMode].Name);
            strOutInfo.AppendLine("Inputs:");
            foreach (ResourceRatio input in MRH[mode[selectedMode].moduleIndex].inputList)
            {
                strOutInfo.AppendLine(input.ResourceName + " (" + input.Ratio + ")");
            }
            strOutInfo.AppendLine("Outputs:");
            strOutInfo.AppendLine(MRH[selectedMode].ResourceName);
            //foreach (ResourceRatio output in MRH[selectedMode].outputList)
            //{
            //    strOutInfo.AppendLine(output.ResourceName + " (" + output.Ratio + ")");
            //}

            GTIDebug.Log("\nGTI_MultiModeConverter:\n" + strOutInfo.ToString(), iDebugLevel.DebugInfo);

            writeScreenMessage(
                Message: strOutInfo.ToString(),
                messagePosition: messagePosition,
                duration: 3f
                );
            writeScreenMessage(
                Message: strOutInfo.ToString(),
                messagePosition: ScreenMessageStyle.UPPER_RIGHT,
                duration: 3f
                );
        }

        protected override void ModuleAnimationGroupEvent_DisableModules()
        {
            //for (int i = 0; i < mode.Count; i++)
            for (int i = 0; i < MRH.Count; i++)     //Disable all modules
            {
                GTIDebug.Log("GTI_MultiModeHarvester: Deactivate Converter Module [" + mode[i].moduleIndex + "] --> " + MRH[mode[i].moduleIndex].ConverterName, iDebugLevel.High);
                //Deactivate the converter
                MRH[mode[i].moduleIndex].DisableModule();
                //Stop the converter
                MRH[mode[i].moduleIndex].StopResourceConverter();
            }
        }

        #region VAB Information
        public override string GetInfo()
        {
            try
            {
                return "GTI Multi Mode Harvester";
            }
            catch (Exception e)
            {
                GTIDebug.LogError(this.GetType().Name + " GetInfo() Error " + e.Message);
                throw;
            }
        }
        #endregion
    }
}