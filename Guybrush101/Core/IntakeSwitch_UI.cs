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
        //private int _selectedIntakeOld = -1;

        //Availability of the functionality
        [KSPField]
        public bool availableInFlight = true;
        [KSPField]
        public bool availableInEditor = true;
        #endregion

        #region User_Interface

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "IntakeSwitcher")]
        [UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, scene = UI_Scene.All, suppressEditorShipModified = false, options = new[] { "None" })]
        public string ChooseOption = "0";
        private string[] Options;
        BaseField chooseField;

        private void initializeGUI()
        {
            //Debug.Log("initializeGUI(): START");
            chooseField = Fields[nameof(ChooseOption)];
            chooseField.guiName = "Intake";     //Dummy name until updated
            chooseField.guiActiveEditor = availableInEditor;
            chooseField.guiActive = availableInFlight;

            //Debug.Log("initializeGUI() | arrPropellantNames.Length: " + arrIntakeNames.Length);

            //Create array Options that are simple ref's to the propellant list
            Options = new string[arrIntakeNames.Length];
            for (int i = 0; i < arrIntakeNames.Length; i++)
            {
                //Debug.Log(
                //    "\ni: " + i +
                //    "\n arrIntakeNames: " + arrIntakeNames[i]
                //    );
                Options[i] = i.ToString();
            }
            //Set which function run's when changing selection, which options, and the text to display
            //var chooseOption = chooseField.uiControlEditor as UI_ChooseOption;
            UI_ChooseOption chooseOption = HighLogic.LoadedSceneIsFlight ? chooseField.uiControlFlight as UI_ChooseOption : chooseField.uiControlEditor as UI_ChooseOption;
            chooseOption.options = Options;
            chooseOption.display = arrIntakeNames;        //Should be GUInames array
            chooseOption.onFieldChanged = selectIntake;

            //var chooseOptionFlight = chooseField.uiControlFlight as UI_ChooseOption;
            //chooseOptionFlight.options = Options;
            //chooseOptionFlight.display = arrIntakeNames;
            //chooseOptionFlight.onFieldChanged = selectIntake;
        }

        //onFieldChanged action
        private void selectIntake(BaseField field, object oldValueObj)
        {
            selectedIntake = int.Parse(ChooseOption);
            updateIntake(true);
        }









        /*
        //START - Events for selection of propellants

        //UI_ChooseOption, UI_ScaleEdit, UI_FloatEdit

        //NEXT
        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Next Intake")]
        public void nextIntakeEvent()
        {
            //InitializeSettings();
            selectedIntake++;
            if (selectedIntake > arrIntakeNames.GetUpperBound(0))
            {
                //if we move from last propellant, then the next one is the first one - aka 0
                selectedIntake = 0;
            }
            updateIntake(true);
        }
        //PREVIOUS
        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Previous Intake")]
        public void previousIntakeEvent()
        {
            //InitializeSettings();
            selectedIntake--;
            if (selectedIntake < 0)
            {
                //if we move from the first propellant, then the previous is the last - aka the upperbound
                selectedIntake = arrIntakeNames.GetUpperBound(0);
            }
            updateIntake(true);
        }
        //END - Events for selection of propellants
        */
        #endregion
        
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
        
    }
}
