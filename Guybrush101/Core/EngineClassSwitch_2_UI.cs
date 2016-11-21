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

            //Extract options from the engineList
            Options = new string[engineList.Count];
            OptionsDisplay = new string[engineList.Count];
            for (int i = 0; (i < engineList.Count); i++)
            {
                Options[i] = engineList[i].engineID;
                OptionsDisplay[i] = GUIengineIDEmpty ? engineList[i].engineID : engineList[i].GUIengineID;
            }
            //If there is only one engine available, then hide the selector menu --> It yields null ref errors if used in flight!!!
            Debug.Log("engineList.Count: " + engineList.Count);
            if (engineList.Count < 2)
            {
                chooseField.guiActive = false;
                chooseField.guiActiveEditor = false;
            }

            UI_ChooseOption chooseOption = HighLogic.LoadedSceneIsFlight ? chooseField.uiControlFlight as UI_ChooseOption : chooseField.uiControlEditor as UI_ChooseOption;
            chooseOption.options = Options;
            chooseOption.display = OptionsDisplay;
            chooseOption.onFieldChanged = selectPropulsion;

            //Update Actions GUI texts and hide the ones not applicable
            for (int i = 1; i <= numberOfSpecificActions; i++)
            {
                if (engineList.Count < i) { this.Actions["ActionPropulsion_" + i].active = false; }
                if (!(engineList.Count < i)) { this.Actions["ActionPropulsion_" + i].guiName = GUIengineIDEmpty ? "Set " + engineList[i-1].engineID : "Set " + engineList[i-1].GUIengineID; }
            }

        }

        private void selectPropulsion(BaseField field, object oldValueObj)
        {
            //selectedPropulsion = ChooseOption;
            //ScreenMessages.PostScreenMessage("Changing Propultion to: " + ChooseOption, 1.5f, ScreenMessageStyle.UPPER_CENTER);
            //writeScreenMessage();
            updatePropulsion();
        }
        private void FindSelectedPropulsion()
        {
            for (int i = 0; i < engineList.Count; i++)
            {
                if (engineList[i].engineID == ChooseOption) { selectedPropulsion = i; }
            }
        }
        private void writeScreenMessage()
        {
            //ScreenMessages.PostScreenMessage("Changing Propultion to: " + engineList[selectedPropulsion].engineID, 1.5f, ScreenMessageStyle.UPPER_CENTER);
            writeScreenMessage(
                Message: "Changing Propulsion to: " + engineList[selectedPropulsion].GUIengineID,
                position: ScreenMessageStyle.UPPER_CENTER
                );
        }
        private void writeScreenMessage(ScreenMessageStyle position, string Message, float duration = 1.5f)
        {
            ScreenMessages.PostScreenMessage(Message, duration, position);
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
            if (selectedPropulsion > engineList.Count - 1) { selectedPropulsion = 0; }
            ChooseOption = engineList[selectedPropulsion].engineID;
            updatePropulsion();
        }
        [KSPAction("Previous Propulsion")]
        public void ActionPreviousPropulsion(KSPActionParam param)
        {
            selectedPropulsion--;
            //Check if selected proplusion was the first one, and return the last one instead
            if (selectedPropulsion < 0) { selectedPropulsion = engineList.Count - 1; }
            ChooseOption = engineList[selectedPropulsion].engineID;

            updatePropulsion();
        }

        //Specific actions
        private const int numberOfSpecificActions = 8;
        [KSPAction("Set Propulsion #1")]
        public void ActionPropulsion_1(KSPActionParam param)    { ActionPropulsion(0); }

        [KSPAction("Set Propulsion #2")]
        public void ActionPropulsion_2(KSPActionParam param)    { ActionPropulsion(1); }

        [KSPAction("Set Propulsion #3")]
        public void ActionPropulsion_3(KSPActionParam param)    { ActionPropulsion(2); }

        [KSPAction("Set Propulsion #4")]
        public void ActionPropulsion_4(KSPActionParam param)    { ActionPropulsion(3); }

        [KSPAction("Set Propulsion #5")]
        public void ActionPropulsion_5(KSPActionParam param)    { ActionPropulsion(4); }

        [KSPAction("Set Propulsion #6")]
        public void ActionPropulsion_6(KSPActionParam param)    { ActionPropulsion(5); }

        [KSPAction("Set Propulsion #7")]
        public void ActionPropulsion_7(KSPActionParam param)    { ActionPropulsion(6); }

        [KSPAction("Set Propulsion #8")]
        public void ActionPropulsion_8(KSPActionParam param)    { ActionPropulsion(7); }

        private void ActionPropulsion(int inActionSelect)
        {
            //Debug.Log("Action ActionPropulsion_" + inActionSelect + " (before): " + ChooseOption);

            //Check if the selected Propulsion is possible
            if (inActionSelect < engineList.Count)
            {
                if (!(selectedPropulsion == inActionSelect))
                {
                    selectedPropulsion = inActionSelect;

                    ChooseOption = engineList[selectedPropulsion].engineID;
                    //Debug.Log("ActionPropulsion_" + inActionSelect + " Executed");
                    updatePropulsion();
                }
            }
            //Debug.Log("Action ActionPropulsion_" + inActionSelect + " (after): " + ChooseOption);
        }
        #endregion



        //moduleEngine.Actions["OnAction"].active = false;
        //moduleEngine.Actions["ShutdownAction"].active = false;
        //moduleEngine.Actions["ActivateAction"].active = false;
    }
}


