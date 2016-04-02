using System;
using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    partial class GTI_IntakeSwitch : PartModule
    {
        /* Example on an Intake Module
        name = ModuleResourceIntake
        resourceName = IntakeAir

        checkForOxygen = true
		area = 0.0031
		intakeSpeed = 15
		intakeTransformName = Intake
        machCurve
        {
            key = 1 1 0 0
			key = 1.5 0.9 -0.4312553 -0.4312553
			key = 2.5 0.45 -0.5275364 -0.5275364
			key = 3.5 0.1 0 0
		}
        */

        [KSPField]
        public string resourceNames = "IntakeAir, IntakeAtm";
        [KSPField]
        public string areas = string.Empty;                      //"0.0031, 0.0031";
        [KSPField]
        public string intakeSpeeds = string.Empty;              //"15, 15";
        [KSPField]
        public string intakeTransformNames = string.Empty;       //"Intake, Intake";
        [KSPField]
        public string machCurves = string.Empty;


        [KSPField(isPersistant = true)]
        public int selectedIntake = -1;


        #region Booleans for existence checks
        private bool resourceNamesEmpty = false;

        #endregion

        #region Other class level declarations
        private bool _settingsInitialized = false;

        private string[] arrIntakeNames;

        private List<ModuleResourceIntake> ModuleIntakes;

        #endregion

        #region Events
        public override void OnStart(PartModule.StartState state)
        {
            InitializeSettings();
            if (selectedIntake == -1)
            {
                selectedIntake = 0;
            }
            updateIntake(false, "OnStart");
        }

        private void InitializeSettings()
        {
            if (!_settingsInitialized)
            {
                #region GUI Update
                var nextEvent = Events["nextIntakeEvent"];
                nextEvent.guiActive = availableInFlight;
                nextEvent.guiActiveEditor = availableInEditor;

                var previousEvent = Events["previousIntakeEvent"];
                previousEvent.guiActive = availableInFlight;
                previousEvent.guiActiveEditor = availableInEditor;
                #endregion

                #region Parse Arrays
                arrIntakeNames = resourceNames.Trim().Split(';');



                #endregion

                if (string.IsNullOrEmpty(resourceNames) || resourceNames.Trim().Length == 0)
                {
                    Debug.LogError("GTI_IntakeSwitch is missing settings for intake names.");
                    return;
                }


                #region Check Input existance etc.
                resourceNamesEmpty = ((string.IsNullOrEmpty(resourceNames) || resourceNames.Trim().Length == 0));
                #endregion

                #region Check and split into arrays
                if (!resourceNamesEmpty)
                {
                    arrIntakeNames = resourceNames.Trim().Split(';');
                }
                #endregion

                ModuleIntakes = part.FindModulesImplementing<ModuleResourceIntake>();
                //foreach (PartModule moduleIntake in ModuleIntakes)
                //{
                //}

                //set settings to initialized
                _settingsInitialized = true;

            }
        }
        #endregion

        #region UpdatePart Intake Module
        private void updateIntake(bool calledByPlayer, string callingFunction = "player")
        {
            foreach (PartModule moduleIntake in ModuleIntakes)
            {
                //*****************************
            }
        }
        #endregion



    }
}
