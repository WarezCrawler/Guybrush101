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
        [KSPField]
        public string resMaxAmount = string.Empty;

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
            Debug.Log("Intake Switch OnStart()");
            InitializeSettings();
            if (selectedIntake == -1)
            {
                selectedIntake = 0;
                //_selectedIntakeOld = 0;
            }
            //GameEvents.onVesselWasModified
            //GameEvents.onEditorShipModified
            //GameEvents.on
            //http://forum.kerbalspaceprogram.com/index.php?/topic/135891-ui_chooseoption-oddities-when-displaying-long-names/

            updateIntake(false, "OnStart");
        }

        private void InitializeSettings()
        {
            if (!_settingsInitialized)
            {
                Debug.Log("Loading Settings for Intake Switcher");
                Utilities Util = new Utilities();

                /*
                #region GUI Update
                var nextEvent = Events["nextIntakeEvent"];
                nextEvent.guiActive = availableInFlight;
                nextEvent.guiActiveEditor = availableInEditor;

                var previousEvent = Events["previousIntakeEvent"];
                previousEvent.guiActive = availableInFlight;
                previousEvent.guiActiveEditor = availableInEditor;
                #endregion
                */


                #region Parse Arrays
                resourceNamesEmpty = Util.ArraySplitEvaluate(resourceNames, out arrIntakeNames, ';');
                #endregion

                #region GUI Update
                if (!resourceNamesEmpty) { initializeGUI(); }
                else
                { Debug.LogError("GTI_IntakeSwitch is missing settings for intake names."); return; }
                #endregion

                ModuleIntakes = part.FindModulesImplementing<ModuleResourceIntake>();

                //set settings to initialized
                _settingsInitialized = true;
                Debug.Log("Intake Switcher settings loaded: " + _settingsInitialized);

            }
        }
        #endregion

        #region UpdatePart Intake Module
        private void updateIntake(bool calledByPlayer, string callingFunction = "player")
        {
            ConfigNode newIntakeNode = new ConfigNode();
            Part currentPart = this.part;


            //foreach (PartModule moduleIntake in ModuleIntakes)
            //Apparently the foreach does not work properly with Intakes. Don't know why.
            for (int i = 0; i < ModuleIntakes.Count; i++)
            {
                //*****************************
                Debug.Log("Update Resource Intake");

                //Define the node object
                ConfigNode IntakeNode = newIntakeNode;          //.GetNode("ModuleResourceIntake");
                ConfigNode IntakeResource = newIntakeNode;

                //Debug.Log("Confignode defined");

                //Set new setting values
                IntakeNode.AddValue("resourceName", arrIntakeNames[selectedIntake]);

                //Debug.Log("Confignode value added");

                //Load changes (nodeobject) into the moduleIntake
                ModuleIntakes[i].Load(IntakeNode);

                //Clean out any previous resources in the intake
                currentPart.Resources.list.Clear();
                PartResource[] partResources = currentPart.GetComponents<PartResource>();
                foreach (PartResource resource in partResources)
                {
                    //currentPart.Resources.list.Remove(resource);
                    DestroyImmediate(resource);
                }

                //Create Resource node
                IntakeResource.AddNode("RESOURCE");
                IntakeResource.AddValue("name", arrIntakeNames[selectedIntake]);
                IntakeResource.AddValue("amount", 0f);
                IntakeResource.AddValue("maxAmount", int.Parse(resMaxAmount));

                //Add the resources
                currentPart.AddResource(IntakeResource);

                //Update the part resources
                currentPart.Resources.UpdateList();

                if (HighLogic.LoadedSceneIsFlight) { KSP.UI.Screens.ResourceDisplay.Instance.Refresh(); }

                //Debug.Log("Confignode Loaded");

                /*
                //machCurves
                //for (int i = 0; i < IntakeNode.GetNode("machCurve").GetValues().Length; i++)
                //{
                //    Debug.Log("MachCurve: [" + i + "] --> " + IntakeNode.GetNode("machCurve").GetValues().GetValue(i));
                //    //IntakeNode.
                //}
                //Debug.Log("MachCurve: " + IntakeNode.GetNode("machCurve").GetValues().GetValue(0));
                //Debug.Log("MachCurve: " + IntakeNode.GetNode("machCurve").);
                string temp = string.Empty;
                foreach (var key in ModuleIntakes[i].machCurve.Curve.keys)
                {
                    //ModuleIntakes[i].machCurve.Curve.
                    //Debug.Log(
                        temp = temp + "\nmachKeys: " + key.time + " " + key.value + " " + key.inTangent + " " + key.outTangent;
                        //);
                }
                Debug.Log(temp);
                */

            }

            #region DEBUGGING
            /* Debugging Area */

            Debug.Log("Update GUI");
            GUIResourceName = ModuleIntakes[0].resourceName;
            Debug.Log("GUI Updated");

            Debug.Log(
                "\nIntakeAir id: " + PartResourceLibrary.Instance.GetDefinition("IntakeAir").id +
                "\nIntakeAir density: "     + PartResourceLibrary.Instance.GetDefinition("IntakeAir").density +
                "\nIntakeAtm id: "          + PartResourceLibrary.Instance.GetDefinition("IntakeAtm").id +
                "\nIntakeAtm density: "     + PartResourceLibrary.Instance.GetDefinition("IntakeAtm").density
            );

            Debug.Log(
                "\nModuleIntakes[0].resourceName: " + ModuleIntakes[0].resourceName +
                "\nresourceId: "                    + ModuleIntakes[0].resourceId +
                "\nresourceDef: "                   + ModuleIntakes[0].resourceDef +
                "\nres: "                           + ModuleIntakes[0].res +
                "\nresourceUnits: "                 + ModuleIntakes[0].resourceUnits +
                "\ncheckForOxygen: "                + ModuleIntakes[0].checkForOxygen +
                "\narea: "                          + ModuleIntakes[0].area +
                "\nairFlow: "                       + ModuleIntakes[0].airFlow +
                "\nModuleIntakes.Count: "           + ModuleIntakes.Count
                );
            
            for (int i = 0;i < part.Resources.Count; i++)
            {
                Debug.Log(
                    "\npart.Resources[0].resourceName: " + part.Resources[i].resourceName +
                    "\npart.Resources[0].amount: " + part.Resources[i].amount +
                    "\npart.Resources[0].maxAmount: " + part.Resources[i].maxAmount
                    );
            }
            #endregion

        }
        #endregion

        public override string GetInfo()
        {
            return "AirIntake Switcher:\n" + resourceNames;
        }

    }
}
