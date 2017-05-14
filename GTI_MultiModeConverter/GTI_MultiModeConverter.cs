using System;
using static GTI.GTIConfig;
using System.Collections.Generic;
using System.Text;

namespace GTI
{
    class GTI_MultiModeConverter : GTI_MultiMode<MultiMode>
    {
        protected List<ModuleResourceConverter> MRC;

        protected override void initializeSettings()
        {
            if (!_settingsInitialized)
            {
                GTIDebug.Log("GTI_MultiModeConverter --> initializeSettings()", iDebugLevel.DebugInfo);

                //Find all converters in the part, and order thier names into an array for reference
                MRC = part.FindModulesImplementing<ModuleResourceConverter>();

                modes = new List<MultiMode>(MRC.Count);
                for (int i = 0; i < MRC.Count; i++)
                {
                    //converterNames[i] = MRC[i].ConverterName;
                    modes.Add(new MultiMode()
                    {
                        moduleIndex = i,
                        ID = i.ToString(),
                        Name = MRC[i].ConverterName
                    });
                    GTIDebug.Log("mode[" + i + "].ID --> " + modes[i].ID, iDebugLevel.DebugInfo);
                    GTIDebug.Log("mode[" + i + "].Name --> " + modes[i].Name, iDebugLevel.DebugInfo);
                }

                //Disable converter actions, as these should be handled by the multimodeconverter module
                for (int i = 0; i < MRC.Count; i++)
                {
                    MRC[i].Actions["ToggleResourceConverterAction"].active = false;
                    MRC[i].Actions["StartResourceConverterAction"].active = false;
                    MRC[i].Actions["StopResourceConverterAction"].active = false;
                }
            }
        }

        public override void updateMultiMode(bool silentUpdate = false)
        {
            GTIDebug.Log("GTI_MultiModeConverter: updateMultiMode() --> Begin", iDebugLevel.High);

            //FindSelectedMode();
            if (silentUpdate == false) writeScreenMessage();

            for (int i = 0; i < modes.Count; i++)
            {
                if (i == selectedMode)
                {
                    GTIDebug.Log("GTI_MultiMode (" + (silentUpdate ? "silent" : "non-silent") + "): Activate Module [" + modes[i].moduleIndex + "] --> " + MRC[modes[i].moduleIndex].ConverterName, iDebugLevel.High);
                    MRC[modes[i].moduleIndex].EnableModule();
                }
                else
                {
                    GTIDebug.Log("GTI_MultiMode (" + (silentUpdate ? "silent" : "non-silent") + "): Deactivate Module [" + modes[i].moduleIndex + "] --> " + MRC[modes[i].moduleIndex].ConverterName, iDebugLevel.High);

                    //Deactivate the converter
                    MRC[modes[i].moduleIndex].DisableModule();
                    //Stop the converter
                    MRC[modes[i].moduleIndex].StopResourceConverter();
                }
            }
            MonoUtilities.RefreshContextWindows(part);
            GTIDebug.Log("GTI_MultiModeConverter: updateMultiMode() --> Finished", iDebugLevel.DebugInfo);
        }

        protected override void ModuleAnimationGroupEvent_DisableModules()
        {
            //for (int i = 0; i < mode.Count; i++)
            for (int i = 0; i < MRC.Count; i++)     //Disable all modules
            {
                GTIDebug.Log("GTI_MultiModeConverter: Deactivate Converter Module [" + modes[i].moduleIndex + "] --> " + MRC[modes[i].moduleIndex].ConverterName, iDebugLevel.High);
                //Deactivate the converter
                MRC[modes[i].moduleIndex].DisableModule();
                //Stop the converter
                MRC[modes[i].moduleIndex].StopResourceConverter();
            }
        }

        protected override void writeScreenMessage()
        {
            //string strOutInfo = string.Empty;
            StringBuilder strOutInfo = new StringBuilder();

            strOutInfo.AppendLine("Converter mode changed to " + modes[selectedMode]);
            strOutInfo.AppendLine("Inputs:");
            foreach (ResourceRatio input in MRC[selectedMode].Recipe.Inputs)
            {
                strOutInfo.AppendLine(input.ResourceName + " (" + input.Ratio + ")");
            }
            strOutInfo.AppendLine("Outputs:");
            foreach (ResourceRatio output in MRC[selectedMode].Recipe.Outputs)
            {
                strOutInfo.AppendLine(output.ResourceName + " (" + output.Ratio + ")");
            }

            GTIDebug.Log("\nGTI_MultiModeConverter:\n" + strOutInfo.ToString(), iDebugLevel.DebugInfo);

            writeScreenMessage(
                Message: strOutInfo.ToString(),
                messagePosition: messagePosition,
                duration: 3f
                );
        }

        #region VAB Information
        public override string GetInfo()
        {
            try
            {
                return "GTI Multi Mode Converter";
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
