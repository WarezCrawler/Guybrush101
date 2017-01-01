using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;
using System;

namespace GTI
{
    partial class GTI_MultiModeConverter : PartModule
    {
        private List<ModuleResourceConverter> MRC;
        private string[] converterNames;

        private bool _settingsInitialized = false;
        private bool _initializing = true;


        /// <summary>
        /// Loads the module at scene startup
        /// </summary>
        /// <param name="state"></param>
        public override void OnStart(PartModule.StartState state)
        {
            _initializing = true;
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
            updateConverter();
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

                _settingsInitialized = true;
            }
        }

        /// <summary>
        /// Updates the converters with new selections. Deactivating the inactive ones, and activating the selected one.
        /// </summary>
        private void updateConverter()
        {
            //Debug.Log("GTI_MultiModeConverter: updateConverter() --> Begin");

            //initializeSettings();
            FindSelectedConverter();
            if (_initializing == false) { writeScreenMessage(); } else { _initializing = false; }

            for (int i = 0; i < MRC.Count; i++)
            {
                if (i == selectedConverter)
                {
                    Debug.Log("GTI_MultiModeConverter: Activate Converter Module [" + i + "] --> " + MRC[i].ConverterName);
                    MRC[i].EnableModule(); 
                }
                else
                {
                    Debug.Log("GTI_MultiModeConverter: Deactivate Converter Module [" + i + "] --> " + MRC[i].ConverterName);


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
