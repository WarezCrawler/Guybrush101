using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    partial class GTI_EngineClassSwitch_2 : PartModule
    {
        #region KSPFields and supporting settings
        //GUI fields for information
        //[KSPField(guiActive = true, guiActiveEditor = true, guiName = "Propellants")]
        //private string GUIpropellantNames = String.Empty;
        //[KSPField]
        //public string iniGUIpropellantNames = string.Empty;

        //public string selectedPropulsion = string.Empty;

        //Availability of the functionality
        [KSPField]
        public bool availableInFlight = false;
        [KSPField]
        public bool availableInEditor = true;
        #endregion

        #region User_Interface
        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "EngineSwitcher")]
        [UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, scene = UI_Scene.All, suppressEditorShipModified = false, options = new[] { "None" }, display = new[] { "None" })]
        public string ChooseOption = string.Empty;
        //[KSPField(isPersistant = true)]
        //public int selectedEngine = 0;                                            //holds the selected propellant setup.


        #endregion

        private void initializeGUI()
        {
            BaseField chooseField;
            string[] Options;
            string[] OptionsDisplay;

            chooseField = Fields[nameof(ChooseOption)];
            chooseField.guiName = "Propulsion";
            chooseField.guiActiveEditor = availableInEditor;
            chooseField.guiActive = availableInFlight;

            Options = arrEngineID;
            OptionsDisplay = GUIengineIDEmpty ? arrEngineID : arrGUIengineID;

            UI_ChooseOption chooseOption = HighLogic.LoadedSceneIsFlight ? chooseField.uiControlFlight as UI_ChooseOption : chooseField.uiControlEditor as UI_ChooseOption;
            chooseOption.options = Options;
            chooseOption.display = OptionsDisplay;
            chooseOption.onFieldChanged = selectPropulsion;
        }

        private void selectPropulsion(BaseField field, object oldValueObj)
        {
            //selectedPropulsion = ChooseOption;
            ScreenMessages.PostScreenMessage("Changing Propultion to: " + ChooseOption, 1.5f, ScreenMessageStyle.UPPER_CENTER);
            updatePropulsion();

        }
    }
}
