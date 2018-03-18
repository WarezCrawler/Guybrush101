using System;
using static GTI.GTIConfig;
using System.Collections.Generic;
using System.Text;

namespace GTI
{
    public class GTI_MultiModeHarvester : GTI_MultiMode<MultiMode>
    {
        protected List<ModuleResourceHarvester> MRH;

        protected override void initializeSettings()
        {
            if (!_settingsInitialized)
            {
                GTIDebug.Log("GTI_MultiModeHarvester --> initializeSettings()", iDebugLevel.DebugInfo);
                MRH = part.FindModulesImplementing<ModuleResourceHarvester>();

                modes = new List<MultiMode>(MRH.Count);
                for (int i = 0; i < MRH.Count; i++)
                {
                    modes.Add(new MultiMode()
                    {
                        moduleIndex = i,
                        ID          = i.ToString(),
                        Name        = MRH[i].ConverterName
                    });
                    GTIDebug.Log("mode[" + i + "].ID --> " + modes[i].ID, iDebugLevel.DebugInfo);
                    GTIDebug.Log("mode[" + i + "].Name --> " + modes[i].Name, iDebugLevel.DebugInfo);
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
                //updateMultiMode = update;         --> Delegate testing
            }
        }

        public override void updateMultiMode(bool silentUpdate = false)
        {
            GTIDebug.Log("GTI_MultiModeConverter: updateMultiMode() --> Begin", iDebugLevel.High);

            //FindSelectedMode();
            if (silentUpdate == false) writeScreenMessage();

            bool MAG_isDeployed = true;
            if (MAG != null && useModuleAnimationGroup == true)
                MAG_isDeployed = MAG.isDeployed;

            for (int i = 0; i < modes.Count; i++)
            {
                if (i == selectedMode && MAG_isDeployed)
                {
                    GTIDebug.Log("GTI_MultiModeConverter (" + (silentUpdate ? "silent" : "non-silent") + "): Activate Converter Module [" + modes[i].moduleIndex + "] --> " + MRH[modes[i].moduleIndex].ConverterName, iDebugLevel.High);
                    MRH[modes[i].moduleIndex].EnableModule();
                }
                else
                {
                    GTIDebug.Log("GTI_MultiModeConverter (" + (silentUpdate ? "silent" : "non-silent") + "): Deactivate Converter Module [" + modes[i].moduleIndex + "] --> " + MRH[modes[i].moduleIndex].ConverterName, iDebugLevel.High);

                    //Deactivate the converter
                    MRH[modes[i].moduleIndex].DisableModule();
                    //Stop the converter
                    MRH[modes[i].moduleIndex].StopResourceConverter();
                }
            }
            MonoUtilities.RefreshContextWindows(part);
            GTIDebug.Log("GTI_MultiModeHarvester: updateMultiMode() --> Finished", iDebugLevel.DebugInfo);
        }


        protected override void writeScreenMessage()
        {
            //string strOutInfo = string.Empty;
            StringBuilder strOutInfo = new StringBuilder();

            strOutInfo.AppendLine("Converter mode changed to " + modes[selectedMode].Name);
            strOutInfo.AppendLine("Inputs:");
            foreach (ResourceRatio input in MRH[modes[selectedMode].moduleIndex].inputList)
            {
                strOutInfo.AppendLine(input.ResourceName + " (" + input.Ratio + ")");
            }
            strOutInfo.AppendLine("Outputs:");
            strOutInfo.AppendLine(MRH[selectedMode].ResourceName);

            GTIDebug.Log("\nGTI_MultiModeHarvester:\n" + strOutInfo.ToString(), iDebugLevel.DebugInfo);

            writeScreenMessage(
                Message: strOutInfo.ToString(),
                messagePosition: messagePosition,
                duration: 3f
                );
        }

        protected override void ModuleAnimationGroupEvent_DisableModules()
        {
            //for (int i = 0; i < mode.Count; i++)
            for (int i = 0; i < MRH.Count; i++)     //Disable all modules
            {
                GTIDebug.Log("GTI_MultiModeHarvester: Deactivate Converter Module [" + modes[i].moduleIndex + "] --> " + MRH[modes[i].moduleIndex].ConverterName, iDebugLevel.High);
                //Deactivate the converter
                MRH[modes[i].moduleIndex].DirtyFlag = false;    //Fix for KSP1.3.1
                MRH[modes[i].moduleIndex].DisableModule();
                //Stop the converter
                MRH[modes[i].moduleIndex].StopResourceConverter();
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