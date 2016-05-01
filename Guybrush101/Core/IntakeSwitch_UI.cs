using System;
using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    partial class GTI_IntakeSwitch : PartModule
    {
        #region KSPFields and supporting settings

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Intake Resource")]
        private string GUIResourceName = String.Empty;

        [KSPField(isPersistant = true)]
        public int selectedIntake = -1;

        //Availability of the functionality
        [KSPField]
        public bool availableInFlight = true;
        [KSPField]
        public bool availableInEditor = true;
        #endregion

        #region User_Interface

        [KSPField(guiActive = false, guiActiveEditor = false, isPersistant = true, guiName = "IntakeSwitcher")]
        [UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, scene = UI_Scene.All, suppressEditorShipModified = false, options = new[] { "None" })]
        public string ChooseOption = "0";
        private string[] Options;
        BaseField chooseField;

        private void initializeGUI()
        {
            //Update the gui
            chooseField = Fields[nameof(ChooseOption)];
            chooseField.guiName = "Intake";     //Dummy name until updated
            chooseField.guiActiveEditor = availableInEditor;
            chooseField.guiActive = availableInFlight;

            //Create array Options that are simple ref's to the propellant list
            Options = new string[arrIntakeNames.Length];
            for (int i = 0; i < arrIntakeNames.Length; i++)
            {
                Options[i] = i.ToString();
            }

            //Set which function run's when changing selection, which options, and the text to display
            //var chooseOption = chooseField.uiControlEditor as UI_ChooseOption;
            UI_ChooseOption chooseOption = HighLogic.LoadedSceneIsFlight ? chooseField.uiControlFlight as UI_ChooseOption : chooseField.uiControlEditor as UI_ChooseOption;
            chooseOption.options = Options;
            chooseOption.display = arrIntakeNames;        //Should be GUInames array
            chooseOption.onFieldChanged = selectIntake;
        }

        //onFieldChanged action
        private void selectIntake(BaseField field, object oldValueObj)
        {
            selectedIntake = int.Parse(ChooseOption);
            updateIntake(true);
        }

        //UI_ChooseOption, UI_ScaleEdit, UI_FloatEdit

        #endregion

        #region Actions
        [KSPAction("Next Intake")]
        public void nextIntakeAction(KSPActionParam param)
        {
            //nextIntakeEvent();
            selectedIntake++;
            if (selectedIntake > arrIntakeNames.GetUpperBound(0))
            {
                //if we move from last propellant, then the next one is the first one - aka 0
                selectedIntake = 0;
            }
            ChooseOption = selectedIntake.ToString();
            updateIntake(true);
        }
        [KSPAction("Previous Intake")]
        public void previousIntakeAction(KSPActionParam param)
        {
            //previousIntakeEvent();
            selectedIntake--;
            if (selectedIntake < 0)
            {
                //if we move from the first propellant, then the previous is the last - aka the upperbound
                selectedIntake = arrIntakeNames.GetUpperBound(0);
            }
            ChooseOption = selectedIntake.ToString();
            updateIntake(true);
        }
        #endregion

    }
}
