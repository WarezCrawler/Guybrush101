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

        [UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, controlEnabled = true, scene = UI_Scene.All,onFieldChanged = ]
        [KSPField(isPersistant = true)]
        public int selectedIntake = -1;
        private int _selectedIntakeOld = -1;


        //Availability of the functionality
        [KSPField]
        public bool availableInFlight = true;
        [KSPField]
        public bool availableInEditor = true;
        #endregion

        #region User_Interface
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
        #endregion

        [KSPAction("Next Intake")]
        public void nextIntakeAction(KSPActionParam param)
        {
            nextIntakeEvent();
        }
        [KSPAction("Previous Intake")]
        public void previousIntakeAction(KSPActionParam param)
        {
            previousIntakeEvent();
        }
    }
}
