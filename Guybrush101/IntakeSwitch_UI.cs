using System;
using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    partial class GTI_IntakeSwitch : PartModule
    {
        #region KSPFields and supporting settings

        //Availability of the functionality
        [KSPField]
        public bool availableInFlight = false;
        [KSPField]
        public bool availableInEditor = true;
        #endregion

        #region User_Interface
        //START - Events for selection of propellants
        //NEXT
        [KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "Next propellant setup")]
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
        [KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "Previous propellant setup")]
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
