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

        public int selectedPropulsion = 0;

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
        private const int numberOfSpecificActions = 4;

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

            //Update Actions GUI texts and hide the ones not applicable
            for (int i = 1; i <= numberOfSpecificActions; i++)
            {
                if (arrEngineID.Length < i) { this.Actions["ActionPropulsion_" + i].active = false; }
                if (!(arrEngineID.Length < i)) { this.Actions["ActionPropulsion_" + i].guiName = GUIengineIDEmpty ? "Set " + arrEngineID[i-1]: "Set " + arrGUIengineID[i-1]; }
            }

        }

        private void selectPropulsion(BaseField field, object oldValueObj)
        {
            //selectedPropulsion = ChooseOption;
            ScreenMessages.PostScreenMessage("Changing Propultion to: " + ChooseOption, 1.5f, ScreenMessageStyle.UPPER_CENTER);
            updatePropulsion();
        }

        #region Actions Engine
        [KSPAction("Toggle Engine")]
        public void ActionToggle(KSPActionParam param)
        {
            if (currentModuleEngine.getIgnitionState) { currentModuleEngine.Shutdown(); }
            else { currentModuleEngine.Activate(); }

            currentEngineState = currentModuleEngine.getIgnitionState;
            Debug.Log("Action ActionToggle: " + ChooseOption + " new state is: " + currentEngineState);
        }
        [KSPAction("Activate Engine")]
        public void ActionActivate(KSPActionParam param)
        {
            if (!currentModuleEngine.getIgnitionState) { currentModuleEngine.Activate(); }

            currentEngineState = currentModuleEngine.getIgnitionState;
            Debug.Log("Action currentModuleEngine.Activate(): " + ChooseOption + " new state is: " + currentEngineState);
        }
        [KSPAction("Shutdown Engine")]
        public void ActionShutdownAction(KSPActionParam param)
        {
            if (currentModuleEngine.getIgnitionState) { currentModuleEngine.Shutdown(); }

            currentEngineState = currentModuleEngine.getIgnitionState;
            Debug.Log("Action currentModuleEngine.Shutdown(): " + ChooseOption + " new state is: " + currentEngineState);
        }
        #endregion

        #region Actions Engine configuration selections
        [KSPAction("Next Propulsion")]
        public void ActionNextPropulsion(KSPActionParam param)
        {
            selectedPropulsion++;
            if (selectedPropulsion > arrEngineID.Length - 1) { selectedPropulsion = 0; }
            ChooseOption = arrEngineID[selectedPropulsion];
            updatePropulsion();
        }
        [KSPAction("Previous Propulsion")]
        public void ActionPreviousPropulsion(KSPActionParam param)
        {
            selectedPropulsion--;
            //Check if selected proplusion was the first one, and return the last one instead
            if (selectedPropulsion < 0) { selectedPropulsion = arrEngineID.Length - 1; }
            ChooseOption = arrEngineID[selectedPropulsion];
            updatePropulsion();
        }

        [KSPAction("Set Propulsion #1")]
        public void ActionPropulsion_1(KSPActionParam param)
        {
            //Set the constant for this action
            const int ActionSelect = 0;

            Debug.Log("Action ActionPropulsion_1 (before): " + ChooseOption);
            
            //Check if the selected Propulsion is possible
            if (ActionSelect < arrEngineID.Length)
            {
                selectedPropulsion = ActionSelect;



                ChooseOption = arrEngineID[selectedPropulsion];
                Debug.Log("ActionPropulsion_1 Executed");
                updatePropulsion();
            }
            
            Debug.Log("Action ActionPropulsion_1 (after): " + ChooseOption);
        }

        [KSPAction("Set Propulsion #2")]
        public void ActionPropulsion_2(KSPActionParam param)
        {
            //Set the constant for this action
            const int ActionSelect = 1;

            Debug.Log("Action ActionPropulsion_2 (before): " + ChooseOption);

            //Check if the selected Propulsion is possible
            if (ActionSelect < arrEngineID.Length)
            {
                selectedPropulsion = ActionSelect;



                ChooseOption = arrEngineID[selectedPropulsion];
                Debug.Log("ActionPropulsion_2 Executed");
                updatePropulsion();
            }

            Debug.Log("Action ActionPropulsion_2 (after): " + ChooseOption);
        }

        [KSPAction("Set Propulsion #3")]
        public void ActionPropulsion_3(KSPActionParam param)
        {
            //Set the constant for this action
            const int ActionSelect = 2;

            Debug.Log("Action ActionPropulsion_3 (before): " + ChooseOption);

            //Check if the selected Propulsion is possible
            if (ActionSelect < arrEngineID.Length)
            {
                selectedPropulsion = ActionSelect;



                ChooseOption = arrEngineID[selectedPropulsion];
                Debug.Log("ActionPropulsion_3 Executed");
                updatePropulsion();
            }

            Debug.Log("Action ActionPropulsion_3 (after): " + ChooseOption);
        }

        [KSPAction("Set Propulsion #4")]
        public void ActionPropulsion_4(KSPActionParam param)
        {
            //Set the constant for this action
            const int ActionSelect = 3;

            Debug.Log("Action ActionPropulsion_4 (before): " + ChooseOption);

            //Check if the selected Propulsion is possible
            if (ActionSelect < arrEngineID.Length)
            {
                selectedPropulsion = ActionSelect;



                ChooseOption = arrEngineID[selectedPropulsion];
                Debug.Log("ActionPropulsion_4 Executed");
                updatePropulsion();
            }

            Debug.Log("Action ActionPropulsion_4 (after): " + ChooseOption);
        }
        #endregion



        //moduleEngine.Actions["OnAction"].active = false;
        //moduleEngine.Actions["ShutdownAction"].active = false;
        //moduleEngine.Actions["ActivateAction"].active = false;
    }
}


