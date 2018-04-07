using static GTI.GTIConfig;
using static GTI.Utilities;
using System.Collections.Generic;
using System;
using System.Text;

namespace GTI
{
    class GTI_MultiModeRCS : GTI_MultiMode<MultiMode>
    {
        [KSPField]
        public string RCSID = string.Empty;
        [KSPField]
        public string GUIRCSID = string.Empty;

        protected bool GUIRCSIDEmpty = true;

        protected List<ModuleRCS> ModuleRCSs;

        protected ModuleRCS currentModuleRCS;
        protected int currentModuleRCSindex;

        protected override void initializeSettings()
        {
            if (!_settingsInitialized)
            {
                GTIDebug.Log("GTI_MultiModeRCS() --> initializeSettings()", iDebugLevel.DebugInfo);

                string[] arrGUIRCSID;

                #region Split into Arrays
                //arrRCSID = RCSID.Trim().Split(';');
                GUIRCSIDEmpty = ArraySplitEvaluate(GUIRCSID, out arrGUIRCSID, ';');
                #endregion

                #region Identify ModuleRCS in Scope
                GTIDebug.Log("Find modules RCS from part", iDebugLevel.DebugInfo);
                ModuleRCSs = part.FindModulesImplementing<ModuleRCS>();

                modes = new List<MultiMode>(ModuleRCSs.Count);
                if (ModuleRCSs.Count != arrGUIRCSID.Length || GUIRCSIDEmpty)
                    GTIDebug.LogError("GTI_MultiModeRCS Error in CFG configuration detected");


                GTIDebug.Log("Create list of modes", iDebugLevel.DebugInfo);
                for (int i = 0; i < ModuleRCSs.Count; i++)
                {
                    modes.Add(new MultiMode()
                    {
                        moduleIndex = i,
                        ID = i.ToString(),
                        Name = GUIRCSIDEmpty ? ModuleRCSs[i].resourceName : arrGUIRCSID[i]
                    });
                }

                //Remove the is interactions buttons of the engines, so that it is controlled by this mod instead
                //foreach (var moduleEngine in ModuleEngines)
                for (int i = 0; i < ModuleRCSs.Count; i++)
                {
                    //Deactivate stock RCS actions
                    ModuleRCSs[i].Actions["ToggleAction"].active = false;
                }

                useModuleAnimationGroup = false;    //ModuleAnimationGroup handling, can be added if needed later 
                #endregion
            }
        }

        public override void updateMultiMode(bool silentUpdate = false)
        {
            GTIDebug.Log("GTI_MultiModeRCS: updateMultiMode() --> Begin", iDebugLevel.High);
            if (!silentUpdate) writeScreenMessage();

            for (int i = 0; i < modes.Count; i++)
            {
                if (i == modes[currentModuleRCSindex].moduleIndex)
                {
                    currentModuleRCS = ModuleRCSs[i];
                    currentModuleRCSindex = i;
                    ModuleRCSs[i].rcsEnabled = true;
                }
                else
                {

                }
            }


            throw new NotImplementedException();
        }

        protected override void ModuleAnimationGroupEvent_DisableModules()
        {
            throw new NotImplementedException();
        }

        protected override void writeScreenMessage()
        {
            if (messagePosition == string.Empty)
                messagePosition = "UPPER_CENTER";

            writeScreenMessage(
                Message: "Changing PRCS propulsion to: " + modes[selectedMode].Name,
                messagePosition: messagePosition
                );
        }
    }
}
