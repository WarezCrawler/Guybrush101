using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;
using System.Text;

/*
This module targets "ModuleEngines" modules for engine switching
*/

namespace GTI
{
    partial class GTI_MultiModeEngine : PartModule
    {

        //Availability of the functionality
        [KSPField]
        public bool availableInFlight = false;
        [KSPField]
        public bool availableInEditor = true;

        [KSPField]
        public string messagePosition = string.Empty;

        #region User_Interface
        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "EngineSwitcher")]
        [UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, scene = UI_Scene.All, suppressEditorShipModified = false, options = new[] { "None" }, display = new[] { "None" })]
        public string ChooseOption = string.Empty;
        private int selectedMode;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "Thrust Limiter [GTI]")]
        [UI_FloatRange(affectSymCounterparts = UI_Scene.Editor, scene = UI_Scene.All, suppressEditorShipModified = false, minValue = 0, maxValue = 100, stepIncrement = 0.5f)]
        float thrustPercentage = 100;

        #endregion

        private void initializeGUI()
        {
            BaseField chooseField;
            string[] Options = new string[engineModeList.Count];
            string[] OptionsDisplay = new string[engineModeList.Count];

            chooseField = Fields[nameof(ChooseOption)];
            chooseField.guiName = "Propulsion";
            chooseField.guiActiveEditor = availableInEditor;
            chooseField.guiActive = availableInFlight;

            for (int i = 0; (i < engineModeList.Count); i++)
            {
                Options[i] = i.ToString();
                OptionsDisplay[i] = GUIengineModeNamesEmpty ? engineModeList[i].Propellants : engineModeList[i].GUIengineModeNames;
            }
            //if (ChooseOption == string.Empty) { ChooseOption == Options[0]; }
            

            //If there is only one engine available, then hide the selector menu --> It yields null ref errors if used in flight!!!
            //Debug.Log("engineList.Count: " + engineList.Count);
            if (engineModeList.Count < 2)
            {
                chooseField.guiActive = false;
                chooseField.guiActiveEditor = false;
            }

            //Update Actions GUI texts and hide the ones not applicable
            initializeActions();

            UI_ChooseOption chooseOption = HighLogic.LoadedSceneIsFlight ? chooseField.uiControlFlight as UI_ChooseOption : chooseField.uiControlEditor as UI_ChooseOption;
            chooseOption.options = Options;
            chooseOption.display = OptionsDisplay;
            chooseOption.onFieldChanged = selectPropulsion;

            #region thrustPercentage of moduleEngines
            BaseField thrustPercentageField = ModuleEngines.Fields[fieldName: "thrustPercentage"];
            thrustPercentageField.guiActive = false;
            thrustPercentageField.guiActiveEditor = false;
            thrustPercentage = ModuleEngines.thrustPercentage;
            Fields[nameof(thrustPercentage)].OnValueModified += thrustPercentage_callback;
            //Fields[nameof(thrustPercentage)].OnValueModified -= thrustPercentage_callback;
            #endregion
        } // END OF private void initializeGUI()

        private void selectPropulsion(BaseField field, object oldValueObj)
        {
            updatePropulsion();
        }
        private void thrustPercentage_callback(object oldValueObj)
        {
            //Debug.Log("[GTI] Setting thrustPercentage");
            ModuleEngines.thrustPercentage = thrustPercentage;

            onThrottleChange(); //Recalc ISP when limiter is changed, same as for throttle
        }
        private void FindSelectedPropulsion()
        {
            selectedMode = int.Parse(ChooseOption);
        }
        /// <summary>
        /// selPropFromChooseOption set selectedPropulsion from ChooseOption.
        /// If ChooseOption is empty, then the first engine in engineList is returned.
        /// Dependent on: ChooseOption, selectedMode
        /// </summary>
        private void selPropFromChooseOption()
        {
            if (ChooseOption == string.Empty)
            {
                ChooseOption = "0";
                selectedMode = 0;
            }
            else
            {
                FindSelectedPropulsion();
            }
        }
        private void writeScreenMessage()
        {
            //string strOutInfo = string.Empty;
            StringBuilder strOutInfo = new StringBuilder();
            
            strOutInfo.AppendLine("Engine mode changed to " + engineModeList[selectedMode].GUIengineModeNames);
            strOutInfo.AppendLine("Propellants:");
            strOutInfo.AppendLine(engineModeList[selectedMode].Propellants);
            
            //Debug.Log("\nGTI_MultiModeEngine:\n" + strOutInfo.ToString());

            //Default position and switch to user defined position
            ScreenMessageStyle position = ScreenMessageStyle.UPPER_CENTER;
            switch (messagePosition)
            {
                case "UPPER_CENTER":
                    position = ScreenMessageStyle.UPPER_CENTER;
                    break;
                case "UPPER_RIGHT":
                    position = ScreenMessageStyle.UPPER_RIGHT;
                    break;
                case "UPPER_LEFT":
                    position = ScreenMessageStyle.UPPER_LEFT;
                    break;
                case "LOWER_CENTER":
                    position = ScreenMessageStyle.LOWER_CENTER;
                    break;
            }

            writeScreenMessage(
                Message: strOutInfo.ToString(),
                position: position,
                duration: 3f
                );
        }
        private void writeScreenMessage(ScreenMessageStyle position, string Message, float duration = 1.5f)
        {
            ScreenMessages.PostScreenMessage(Message, duration, position);
        }


        #region ACTION

        private void initializeActions()
        {
            //Update Actions GUI texts and hide the ones not applicable
            for (int i = 1; i <= numberOfSpecificActions; i++)
            {
                if (engineModeList.Count < i) { this.Actions["ActionPropulsion_" + i].active = false; }
                else { this.Actions["ActionPropulsion_" + i].guiName = GUIengineModeNamesEmpty ? "Set " + engineModeList[i - 1].Propellants : "Set " + engineModeList[i - 1].GUIengineModeNames; }
            }
        }

        //Specific actions
        private const int numberOfSpecificActions = 12;
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

        [KSPAction("Set Propulsion #9")]
        public void ActionPropulsion_9(KSPActionParam param) { ActionPropulsion(8); }

        [KSPAction("Set Propulsion #10")]
        public void ActionPropulsion_10(KSPActionParam param) { ActionPropulsion(9); }

        [KSPAction("Set Propulsion #11")]
        public void ActionPropulsion_11(KSPActionParam param) { ActionPropulsion(10); }

        [KSPAction("Set Propulsion #12")]
        public void ActionPropulsion_12(KSPActionParam param) { ActionPropulsion(11); }

        private void ActionPropulsion(int inActionSelect)
        {
            //Debug.Log("Action ActionPropulsion_" + inActionSelect + " (before): " + ChooseOption);

            //Check if the selected Propulsion is possible
            if (inActionSelect < engineModeList.Count)
            {
                if (!(selectedMode == inActionSelect))
                {
                    selectedMode = inActionSelect;      //This is also handled in updatePropulsion
                    ChooseOption = selectedMode.ToString();
                    updatePropulsion();
                }
            }
            //Debug.Log("Action ActionPropulsion_" + inActionSelect + " (after): " + ChooseOption);
        }

        [KSPAction("Next Propulsion")]
        public void ActionNextPropulsion(KSPActionParam param)
        {
            selectedMode++;
            if (selectedMode > engineModeList.Count - 1) { selectedMode = 0; }
            ChooseOption = selectedMode.ToString();
            updatePropulsion();
        }
        [KSPAction("Previous Propulsion")]
        public void ActionPreviousPropulsion(KSPActionParam param)
        {
            selectedMode--;
            //Check if selected proplusion was the first one, and return the last one instead
            if (selectedMode < 0) { selectedMode = engineModeList.Count - 1; }
            ChooseOption = selectedMode.ToString();

            updatePropulsion();
        }

        #endregion

        
    }
}
