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





        #region Booleans for existence checks
        private bool resourceNamesEmpty = false;

        #endregion

        #region Other class level declarations
        private bool _settingsInitialized = false;

        private string[] arrIntakeNames;

        private List<ModuleResourceIntake> ModuleIntakes;
        //private ConfigNode node;

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
        /*
        public override void OnLoad(ConfigNode inNode)
        {
            //base.OnLoad(node);
            ConfigNode node = inNode;
            //Debug.Log(node.);

            foreach (var n in node.GetNodes())
            {
                Debug.Log("node.GetNodes():\n " + n);
            }
            foreach (var n in node.GetValues())
            {
                Debug.Log("node.GetValues():\n" + n);
            }

            //node.


        }
        */
        private void InitializeSettings()
        {
            if (!_settingsInitialized)
            {
                Debug.Log("Loading Settings for Intake Switcher");
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
                Debug.Log("InitializeSettings() --> ModuleIntakes[0].resourceName " + ModuleIntakes[0].resourceName);
                //ModuleIntakes[1].

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

            //foreach (PartModule moduleIntake in ModuleIntakes)
            for (int i = 0; i < ModuleIntakes.Count; i++)
            {
                //*****************************
                Debug.Log("Update Resource Intake");
                //moduleIntake.Fields.SetValue("resourceName", "IntakeAtm");

                //ConfigNode IntakeNode = newIntakeNode.AddNode("PROPELLANT");

                //Define the node object
                ConfigNode IntakeNode = newIntakeNode;          //.GetNode("ModuleResourceIntake");

                Debug.Log("Confignode defined");

                //Set new setting values
                IntakeNode.AddValue("resourceName", arrIntakeNames[selectedIntake]);

                Debug.Log("Confignode value added");

                //Load changes (nodeobject) into the moduleIntake
                ModuleIntakes[i].Load(IntakeNode);

                Debug.Log("Confignode Loaded");

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


            }
            //ModuleIntakes[0].resourceName = arrIntakeNames[selectedIntake];
            

            //ConfigNode propNode = ModuleIntakes[0].GetComponents<>;
            //propNode.AddValue("name", arrtargetPropellants[i]);

            Debug.Log("Update GUI");
            GUIResourceName = ModuleIntakes[0].resourceName;
            Debug.Log("GUI Updated");

            Debug.Log(
                "IntakeAir id: "            + PartResourceLibrary.Instance.GetDefinition("IntakeAir").id +
                "\nIntakeAir density: "     + PartResourceLibrary.Instance.GetDefinition("IntakeAir").density +
                "\nIntakeAtm id: "          + PartResourceLibrary.Instance.GetDefinition("IntakeAtm").id +
                "\nIntakeAtm density: "     + PartResourceLibrary.Instance.GetDefinition("IntakeAtm").density
            );

            Debug.Log(
                "ModuleIntakes[0].resourceName: "   + ModuleIntakes[0].resourceName +
                "\nresourceId: "                    + ModuleIntakes[0].resourceId +
                "\nresourceDef: "                   + ModuleIntakes[0].resourceDef +
                "\nres: "                           + ModuleIntakes[0].res +
                "\nresourceUnits: "                 + ModuleIntakes[0].resourceUnits +
                "\ncheckForOxygen: "                + ModuleIntakes[0].checkForOxygen +
                "\narea: "                          + ModuleIntakes[0].area +
                "\nairFlow: "                       + ModuleIntakes[0].airFlow +
                "\nModuleIntakes.Count: "           + ModuleIntakes.Count
                );
            //ModuleIntakes[0].machCurve.
            //ModuleIntakes.Count;
        }
        #endregion

        public override string GetInfo()
        {
            return "AirIntake Switcher:\n" + resourceNames;
        }

    }
}
