using System.Collections.Generic;
using UnityEngine;
//using GTI.GenericFunctions;
//using System;

namespace GTI
{
    partial class GTI_MultiModeConverter : PartModule
    {
        private List<ModuleResourceConverter> MRC;
        private string[] converterNames;

        private bool _settingsInitialized = false;

        [KSPField]
        public bool useModuleAnimationGroup = false;

        private Animation Anim = new Animation();
        //private List<ModuleAnimationGroup> MAG = new List<ModuleAnimationGroup>();
        private ModuleAnimationGroup MAG = new ModuleAnimationGroup();

        /// <summary>
        /// Loads the module at scene startup
        /// </summary>
        /// <param name="state"></param>
        public override void OnStart(PartModule.StartState state)
        {
            initializeSettings();
            initializeGUI();

            //Show Debug GUI?
            debugActivator();
        }

        /// <summary>
        /// After onstart, the converters can be updated. This cannot be done sooner, since all settings does not seem to be finished loading at that point.
        /// </summary>
        /// <param name="state"></param>
        public override void OnStartFinished(StartState state)
        {
            updateConverter(silentUpdate: true);
        }

        /// <summary>
        /// initializes settings for the module
        /// </summary>
        private void initializeSettings()
        {
            if (!_settingsInitialized)
            {
                //Debug.Log("GTI_MultiModeConverter: initializeSettings() --> !_settingsInitialized");
                
                //Find all converters in the part, and order thier names into an array for reference
                MRC = part.FindModulesImplementing<ModuleResourceConverter>();
                converterNames = new string[MRC.Count];
                for (int i = 0; i < MRC.Count; i++)
                {
                    converterNames[i] = MRC[i].ConverterName;
                }

                //Disable converter actions, as these should be handled by the multimodeconverter module
                for (int i = 0; i < MRC.Count; i++)
                {
                    MRC[i].Actions["ToggleResourceConverterAction"].active = false;
                    MRC[i].Actions["StartResourceConverterAction"].active = false;
                    MRC[i].Actions["StopResourceConverterAction"].active = false;
                    //for (int j = 0; j < MRC[i].Actions.Count; j++)
                    //{
                    //    Debug.Log("MRC[" + i + "].Actions[" + j + "].active " + MRC[i].Actions[j].active);
                    //    MRC[i].Actions[j].active = false;
                    //}
                }

                #region ModuleAnimationGroup
                //Debug.Log("useModuleAnimationGroup: " + useModuleAnimationGroup);
                if (useModuleAnimationGroup == true)
                {
                    //Debug.Log("useModuleAnimationGroup: Evaluated as true");
                    
                    MAG = part.FindModuleImplementing<ModuleAnimationGroup>();
                    //Debug.Log("MAG.DeployAnimation " + MAG.activeAnimationName);
                    //Debug.Log("MAG.DeployAnimation " + MAG.deployAnimationName);

                    Anim = part.FindModelAnimator(MAG.deployAnimationName);

                    foreach (BaseEvent e in MAG.Events)
                    {
                        e.active = false;
                        e.guiActive = false;
                        e.guiActiveEditor = false;
                    }
                    foreach (BaseAction a in MAG.Actions)
                    {
                        a.active = false;
                    }

                    //Debug.Log("Activate 'ModuleAnimationGroupEvent'");
                    
                    //Activate event in this module to trigger animations instead
                    this.Events["ModuleAnimationGroupEvent"].active = true;
                    this.Events["ModuleAnimationGroupEvent"].guiActive = true;
                    this.Events["ModuleAnimationGroupEvent"].guiActiveEditor = true;
                }
                #endregion

                _settingsInitialized = true;
            }
        }

        /// <summary>
        /// Updates the converters with new selections. Deactivating the inactive ones, and activating the selected one.
        /// </summary>
        private void updateConverter(bool silentUpdate = false)
        {
            //Debug.Log("GTI_MultiModeConverter: updateConverter() --> Begin");

            //initializeSettings();
            FindSelectedConverter();
            if (silentUpdate == false) writeScreenMessage(); 

            for (int i = 0; i < MRC.Count; i++)
            {
                if (i == selectedConverter)
                {
                    if (silentUpdate == false) Debug.Log("GTI_MultiModeConverter: Activate Converter Module [" + i + "] --> " + MRC[i].ConverterName);
                    MRC[i].EnableModule(); 
                }
                else
                {
                    if (silentUpdate == false) Debug.Log("GTI_MultiModeConverter: Deactivate Converter Module [" + i + "] --> " + MRC[i].ConverterName);

                    //Deactivate the converter
                    MRC[i].DisableModule();
                    //Stop the converter
                    MRC[i].StopResourceConverter();
                }
            }
            //Debug.Log("GTI_MultiModeConverter: updateConverter() --> Finished");
        }

    } // End Class
}
