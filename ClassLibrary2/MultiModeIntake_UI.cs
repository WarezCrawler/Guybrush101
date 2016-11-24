using System;
using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    partial class GTI_MultiModeIntake : PartModule
    {
        #region KSPFields and supporting settings

        //[KSPField(guiActive = true, guiActiveEditor = true, guiName = "Intake Resource")]
        //private string GUIResourceName = String.Empty;

        [KSPField(isPersistant = true)]
        public int selectedIntake = -1;

        //Availability of the functionality
        [KSPField]
        public bool availableInFlight = true;
        [KSPField]
        public bool availableInEditor = true;
        #endregion

        #region User_Interface

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "IntakeSwitcher")]
        [UI_ChooseOption(affectSymCounterparts = UI_Scene.None, scene = UI_Scene.All, suppressEditorShipModified = false, options = new[] { "None" }, display = new[] { "None" })]
        public string ChooseOption = "0";
        //public string ChooseOption = string.Empty;
        //private string[] Options;
        //BaseField chooseField;

        private void initializeGUI()
        {
            BaseField chooseField;
            string[] Options;
            string[] OptionsDisplay;

            //Update the gui
            chooseField = Fields[nameof(ChooseOption)];
            chooseField.guiName = "Intake";     //Dummy name until updated
            chooseField.guiActiveEditor = availableInEditor;
            chooseField.guiActive = availableInFlight;

            //Create array Options that are simple ref's to the propellant list
            Options = new string[arrIntakeNames.Length];
            OptionsDisplay = new string[arrIntakeNames.Length];
            Debug.Log("MultiModeIntake: Populate chooseField Options");
            for (int i = 0; i < arrIntakeNames.Length; i++)
            {
                Options[i] = i.ToString();
                OptionsDisplay[i] = arrIntakeNames[i];
            }
            Debug.Log("MultiModeIntake: Set chooseField to inactive");
            //If there is only one intake available, then hide the selector menu --> It yields null ref errors if used in flight!!!
            if (arrIntakeNames.Length < 2)
            {
                chooseField.guiActive = false;
                chooseField.guiActiveEditor = false;
            }

            //Set which function run's when changing selection, which options, and the text to display
            //var chooseOption = chooseField.uiControlEditor as UI_ChooseOption;
            UI_ChooseOption chooseOption = HighLogic.LoadedSceneIsFlight ? chooseField.uiControlFlight as UI_ChooseOption : chooseField.uiControlEditor as UI_ChooseOption;
            chooseOption.options = Options;
            chooseOption.display = OptionsDisplay;
            chooseOption.onFieldChanged = selectIntake;
        }

        //onFieldChanged action
        private void selectIntake(BaseField field, object oldValueObj)
        {
            Debug.Log("MultiModeIntake: selectIntake");
            selectedIntake = int.Parse(ChooseOption);
            //updateIntake(true);
            updateIntake(true);
        }

        //UI_ChooseOption, UI_ScaleEdit, UI_FloatEdit

        #endregion

        #region Actions
        [KSPAction("Next Intake")]
        public void nextIntakeAction(KSPActionParam param)
        {
            //nextIntakeEvent();
            Debug.Log("MultiModeIntake: nextIntakeAction");
            selectedIntake++;
            if (selectedIntake > arrIntakeNames.GetUpperBound(0))
            {
                //if we move from last propellant, then the next one is the first one - aka 0
                selectedIntake = 0;
            }
            ChooseOption = selectedIntake.ToString();
            //updateIntake(true);
            updateIntake(true);
        }
        [KSPAction("Previous Intake")]
        public void previousIntakeAction(KSPActionParam param)
        {
            Debug.Log("MultiModeIntake: previousIntakeAction");
            //previousIntakeEvent();
            selectedIntake--;
            if (selectedIntake < 0)
            {
                //if we move from the first propellant, then the previous is the last - aka the upperbound
                selectedIntake = arrIntakeNames.GetUpperBound(0);
            }
            ChooseOption = selectedIntake.ToString();
            //updateIntake(true);
            updateIntake(true);
        }
        #endregion

    }
}
