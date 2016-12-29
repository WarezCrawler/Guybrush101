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
        private float _startDelay = 1.5f;

        public override void OnStart(PartModule.StartState state)
        {
            _initializing = true;
            initializeSettings();
            initializeGUI();
            //updateConverter();
            //Workaround on an delayed update issue in the converter modules....
            Invoke("updateConverter", _startDelay);
        }
        private void initializeSettings()
        {
            if (!_settingsInitialized)
            {
                //Debug.Log("GTI_MultiModeConverter: initializeSettings() --> !_settingsInitialized");
                
                //Find all converster in the part, and order thier names into an array for reference
                MRC = part.FindModulesImplementing<ModuleResourceConverter>();
                converterNames = new string[MRC.Count];
                //Debug.Log("GTI_MultiModeConverter: MRC.Count --> " + MRC.Count);
                for (int i = 0; i < MRC.Count; i++)
                {
                    //Debug.Log("GTI_MultiModeConverter: int i --> " + i);
                    //Debug.Log("GTI_MultiModeConverter: MRC[i].name --> " + MRC[i].ConverterName);
                    converterNames[i] = MRC[i].ConverterName;
                    //Debug.Log("GTI_MultiModeConverter: converterNames[i] --> " + converterNames[i]);
                }


                //foreach (ModuleResourceConverter converter in ModuleResourceConverters)
                //{
                //    converterNames[i] = converter.name;
                //    i++;
                //}

                //Debug.Log("GTI_MultiModeConverter: _settingsInitialized = true");
                _settingsInitialized = true;
            }
        }
        private void updateConverter()
        {
            //Debug.Log("GTI_MultiModeConverter: updateConverter() --> Begin");

            //initializeSettings();
            FindSelectedConverter();
            if (_initializing == false) { writeScreenMessage(); } else { _initializing = false; }

            for (int i = 0; i < MRC.Count; i++)
            {
                //Debug.Log("GTI_MultiModeConverter: MRC[" + i + "].ConverterName --> " + MRC[i].ConverterName);

                if (i == selectedConverter)
                {
                    Debug.Log("GTI_MultiModeConverter: Activate Converter Module [" + i + "] --> " + MRC[i].ConverterName);

                    //Debug.Log("GTI_MultiModeConverter: .isEnabled --> " + MRC[i].isEnabled);
                    //Debug.Log("GTI_MultiModeConverter: .moduleIsEnabled --> " + MRC[i].moduleIsEnabled);

                    //Debug.Log("GTI_MultiModeConverter: .isEnabled --> " + MRC[i].isEnabled);
                    //Debug.Log("GTI_MultiModeConverter: .moduleIsEnabled --> " + MRC[i].moduleIsEnabled);
                    //MRC[i].isEnabled = true;

                    //Debug.Log("GTI_MultiModeConverter: .isEnabled (post) --> " + MRC[i].isEnabled);
                    //Debug.Log("GTI_MultiModeConverter: .moduleIsEnabled (post) --> " + MRC[i].moduleIsEnabled);


                    MRC[i].EnableModule(); 
                    //Debug.Log("GTI_MultiModeConverter: MRC[i].EnableModule()");
                    //MRC[i].enabled = true;
                    //MRC[i].moduleIsEnabled = true;
                }
                else
                {
                    Debug.Log("GTI_MultiModeConverter: Deactivate Converter Module [" + i + "] --> " + MRC[i].ConverterName);


                    //Deactivate the converter
                    //Debug.Log("GTI_MultiModeConverter: .isEnabled --> " + MRC[i].isEnabled);
                    //Debug.Log("GTI_MultiModeConverter: .moduleIsEnabled --> " + MRC[i].moduleIsEnabled);

                    //Debug.Log("GTI_MultiModeConverter: .isEnabled --> " + MRC[i].isEnabled);
                    //Debug.Log("GTI_MultiModeConverter: .moduleIsEnabled --> " + MRC[i].moduleIsEnabled);
                    //MRC[i].isEnabled = false;

                    //Debug.Log("GTI_MultiModeConverter: .isEnabled (post) --> " + MRC[i].isEnabled);
                    //Debug.Log("GTI_MultiModeConverter: .moduleIsEnabled (post) --> " + MRC[i].moduleIsEnabled);

                    MRC[i].DisableModule(); 
                    //Debug.Log("GTI_MultiModeConverter: MRC[i].DisableModule()");
                    //MRC[i].enabled = false;
                    //MRC[i].moduleIsEnabled = false;

                    //Stop the converter
                    MRC[i].StopResourceConverter();
                }

                //MRC[i].enabled = false;
                //MRC[i].EnableModule();
                //MRC[i].isEnabled = false;
                //MRC[i].ModuleIsActive();
                //MRC[i].moduleIsEnabled = false;
                //MRC[i].DisableModule();
            }
            //Debug.Log("GTI_MultiModeConverter: updateConverter() --> Finished");
        }

    } // End Class
}
