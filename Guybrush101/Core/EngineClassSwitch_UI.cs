using System;
using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    partial class GTI_EngineClassSwitch : PartModule
    {
        #region KSPFields and supporting settings
        //GUI fields for information
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Propellants")]
        private string GUIpropellantNames = String.Empty;
        [KSPField]
        public string iniGUIpropellantNames = string.Empty;

        [KSPField(isPersistant = true)]
        public int selectedPropellant = 0;                                             //holds the selected propellant setup.

        //Availability of the functionality
        [KSPField]
        public bool availableInFlight = false;
        [KSPField]
        public bool availableInEditor = true;

        //private bool RightClickUI_onoff = false;
        #endregion

        #region OLD User_Interface
        /*
        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "EngineSwitcher")]
        [UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, scene = UI_Scene.All, suppressEditorShipModified = false, options = new[] { "None" }, display = new[] { "None" })]
        public string ChooseOption = "0";

        private void initializeGUI()
        {
            BaseField chooseField;
            string[] Options;
            string[] OptionsDisplay;

            //Debug.Log("initializeGUI(): START");
            chooseField                     = Fields[nameof(ChooseOption)];
            chooseField.guiName             = "Propellants";
            chooseField.guiActiveEditor     = availableInEditor;
            chooseField.guiActive           = availableInFlight;

            //Debug.Log("initializeGUI() | arrPropellantNames.Length: " + arrPropellantNames.Length);

            //Create array Options that are simple ref's to the propellant list
            //Debug.Log(
            //    "propList.Count: " + propList.Count
            //    );
            Options = new string[propList.Count];        //Options = new string[arrPropellantNames.Length];
            OptionsDisplay = new string[propList.Count];
            for (int i = 0; (i < propList.Count) ; i++)
            {
                //Debug.Log(
                //    "\ni: " + i +
                //    "\npropList[i].Propellants: " + propList[i].Propellants
                //    );

                //Debug.Log("Tech basicRocketry: " + ResearchAndDevelopment.GetTechnologyState("basicRocketry"));
                //if (ResearchAndDevelopment.GetTechnologyState(propList[i].requiredTech) == RDTech.State.Available)
                //if (propList[i].EngineConfigAvailable)
                //{
                    //Debug.Log("Add " + propList[i].Propellants + " (" + i + ") to UI");
                    Options[i] = i.ToString();
                    OptionsDisplay[i] = propList[i].Propellants;
                //}
            }
            
            
            //Set which function run's when changing selection, which options, and the text to display
            //var chooseOptionEditor = chooseField.uiControlEditor as UI_ChooseOption;
            UI_ChooseOption chooseOption = HighLogic.LoadedSceneIsFlight ? chooseField.uiControlFlight as UI_ChooseOption : chooseField.uiControlEditor as UI_ChooseOption;
            chooseOption.options = Options;
            chooseOption.display = OptionsDisplay;              //arrPropellantNames;        //Should be GUInames array
            chooseOption.onFieldChanged = selectPropellant;
        }

        //onFieldChanged action
        private void selectPropellant(BaseField field, object oldValueObj)
        {
            selectedPropellant = int.Parse(ChooseOption);
            //chooseField.guiName = propList[selectedPropellant].Propellants;
            updateEngineModule(true);
        }
        


        [KSPAction("Next propellant")]
        public void nextPropellantAction(KSPActionParam param)
        {
            //nextPropellantEvent();

            selectedPropellant++;
            if (selectedPropellant > (propList.Count - 1))                           //arrPropellantNames.GetUpperBound(0))
            {
                //if we move from last propellant, then the next one is the first one - aka 0
                selectedPropellant = 0;
            }
            ChooseOption = selectedPropellant.ToString();
            updateEngineModule(true);

        }
        [KSPAction("Previous propellant")]
        public void previousPropellantAction(KSPActionParam param)
        {
            //previousPropellantEvent();
            selectedPropellant--;
            if (selectedPropellant < 0)
            {
                //if we move from the first propellant, then the previous is the last - aka the upperbound
                selectedPropellant = (propList.Count-1);                           //arrPropellantNames.GetUpperBound(0))
            }
            ChooseOption = selectedPropellant.ToString();
            updateEngineModule(true);
        }
        */
        #endregion OLD User_Interface

        #region ###### UPDATED GUI ######

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "EngineSwitcher")]
        [UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, scene = UI_Scene.All, suppressEditorShipModified = false, options = new[] { "None" }, display = new[] { "None" })]
        public string ChooseOption = string.Empty;

        private void initializeGUI_2()
        {
            BaseField chooseField;
            string[] Options;
            string[] OptionsDisplay;

            chooseField = Fields[nameof(ChooseOption)];
            chooseField.guiName = "Propulsion";
            chooseField.guiActiveEditor = availableInEditor;
            chooseField.guiActive = availableInFlight;

            //Extract options from the propellantlist
            Options = new string[propList.Count];
            OptionsDisplay = new string[propList.Count];
            for (int i = 0; (i < propList.Count); i++)
            {
                Options[i] = propList[i].Propellants;
                OptionsDisplay[i] = iniGUIpropellantNamesEmpty ? propList[i].Propellants : propList[i].GUIpropellantNames;
            }
            //If there is only one engine available, then hide the selector menu --> It yields null ref errors if used in flight!!!
            Debug.Log("engineList.Count: " + propList.Count);
            if (propList.Count < 2)
            {
                chooseField.guiActive = false;
                chooseField.guiActiveEditor = false;
            }

            //Configurate the ChooseOption menu
            UI_ChooseOption chooseOption = HighLogic.LoadedSceneIsFlight ? chooseField.uiControlFlight as UI_ChooseOption : chooseField.uiControlEditor as UI_ChooseOption;
            chooseOption.options = Options;
            chooseOption.display = OptionsDisplay;
            chooseOption.onFieldChanged = selectPropulsion;

            //Update Actions GUI texts and hide the ones not applicable
            for (int i = 1; i <= numberOfSpecificActions; i++)
            {
                if (propList.Count < i) { this.Actions["ActionPropulsion_" + i].active = false; }
                if (!(propList.Count < i)) { this.Actions["ActionPropulsion_" + i].guiName = iniGUIpropellantNamesEmpty ? "Set " + propList[i - 1].Propellants : "Set " + propList[i - 1].GUIpropellantNames; }
            }



        }

        private void selectPropulsion(BaseField field, object oldValueObj)
        {
            //selectedPropulsion = ChooseOption;
            //ScreenMessages.PostScreenMessage("Changing Propultion to: " + ChooseOption, 1.5f, ScreenMessageStyle.UPPER_CENTER);
            //writeScreenMessage();
            updateEngineModule(true);                    //updatePropulsion();
        }
        private void FindSelectedPropulsion()
        {
            for (int i = 0; i < propList.Count; i++)
            {
                if (propList[i].Propellants == ChooseOption) { selectedPropellant = i; }
            }
        }
        private void writeScreenMessage()
        {
            //ScreenMessages.PostScreenMessage("Changing Propultion to: " + engineList[selectedPropulsion].engineID, 1.5f, ScreenMessageStyle.UPPER_CENTER);
            writeScreenMessage(
                Message: "Changing Propultion to: " + propList[selectedPropellant].GUIpropellantNames,
                position: ScreenMessageStyle.UPPER_CENTER
                );
        }
        private void writeScreenMessage(ScreenMessageStyle position, string Message, float duration = 1.5f)
        {
            ScreenMessages.PostScreenMessage(Message, duration, position);
        }

        #region Actions Engine configuration selections
        [KSPAction("Next Propulsion")]
        public void ActionNextPropulsion(KSPActionParam param)
        {
            selectedPropellant++;
            if (selectedPropellant > propList.Count - 1) { selectedPropellant = 0; }
            ChooseOption = propList[selectedPropellant].Propellants;
            updateEngineModule(true);
        }
        [KSPAction("Previous Propulsion")]
        public void ActionPreviousPropulsion(KSPActionParam param)
        {
            selectedPropellant--;
            //Check if selected proplusion was the first one, and return the last one instead
            if (selectedPropellant < 0) { selectedPropellant = propList.Count - 1; }
            ChooseOption = propList[selectedPropellant].Propellants;

            updateEngineModule(true);
        }

        //Specific actions
        private const int numberOfSpecificActions = 8;
        [KSPAction("Set Propulsion #1")]
        public void ActionPropulsion_1(KSPActionParam param) { ActionPropulsion(0); }

        [KSPAction("Set Propulsion #2")]
        public void ActionPropulsion_2(KSPActionParam param) { ActionPropulsion(1); }

        [KSPAction("Set Propulsion #3")]
        public void ActionPropulsion_3(KSPActionParam param) { ActionPropulsion(2); }

        [KSPAction("Set Propulsion #4")]
        public void ActionPropulsion_4(KSPActionParam param) { ActionPropulsion(3); }

        [KSPAction("Set Propulsion #5")]
        public void ActionPropulsion_5(KSPActionParam param) { ActionPropulsion(4); }

        [KSPAction("Set Propulsion #6")]
        public void ActionPropulsion_6(KSPActionParam param) { ActionPropulsion(5); }

        [KSPAction("Set Propulsion #7")]
        public void ActionPropulsion_7(KSPActionParam param) { ActionPropulsion(6); }

        [KSPAction("Set Propulsion #8")]
        public void ActionPropulsion_8(KSPActionParam param) { ActionPropulsion(7); }

        private void ActionPropulsion(int inActionSelect)
        {
            //Debug.Log("Action ActionPropulsion_" + inActionSelect + " (before): " + ChooseOption);

            //Check if the selected Propulsion is possible
            if (inActionSelect < propList.Count)
            {
                if (!(selectedPropellant == inActionSelect))
                {
                    selectedPropellant = inActionSelect;

                    ChooseOption = propList[selectedPropellant].Propellants;
                    //Debug.Log("ActionPropulsion_" + inActionSelect + " Executed");
                    updateEngineModule(true);
                }
            }
            //Debug.Log("Action ActionPropulsion_" + inActionSelect + " (after): " + ChooseOption);
        }

        #endregion Actions Engine configuration selections
        #endregion ###### UPDATED GUI ######













    }
}
