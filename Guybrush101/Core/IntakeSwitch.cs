using System;
using System.Collections.Generic;
//using KSPAssets;
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
        public string resourceNames = string.Empty;
        [KSPField]
        public string checkForOxygen = string.Empty;
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

        [KSPField]
        public string debugMode = "false";

        #region Booleans for existence checks
        private bool resourceNamesEmpty = false;
        private bool resMaxAmountEmpty = false;
        private bool checkForOxygenEmpty = false;

        #endregion

        #region Other class level declarations
        private bool _settingsInitialized = false;

        private string[] arrIntakeNames, arrcheckForOxygen;

        private List<ModuleResourceIntake> ModuleIntakes;

        #endregion

        #region Events
        public override void OnStart(PartModule.StartState state)
        {
            //Debug.Log("Intake Switch OnStart()");
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
                //Debug.Log("Loading Settings for Intake Switcher");
                Utilities Util = new Utilities();

                #region Parse Arrays
                resourceNamesEmpty = Util.ArraySplitEvaluate(resourceNames, out arrIntakeNames, ';');
                checkForOxygenEmpty = Util.ArraySplitEvaluate(checkForOxygen, out arrcheckForOxygen, ';');
                #endregion
                #region Parse Strings
                resMaxAmountEmpty = Util.StringEvaluate(resMaxAmount, out resMaxAmount);
                
                //If the resMaxAmount is empty, try getting the maxAmount of first intake. If fails, then return a simple 2 Units.
                try
                { resMaxAmount = resMaxAmountEmpty ? ModuleIntakes[0].res.maxAmount.ToString() : resMaxAmount; }
                catch
                { resMaxAmount = "2"; }
                #endregion

                #region GUI Update
                if (!resourceNamesEmpty) { initializeGUI(); }
                else { Debug.LogError("GTI_IntakeSwitch is missing settings for intake names."); return; }
                #endregion

                ModuleIntakes = part.FindModulesImplementing<ModuleResourceIntake>();

                //GUIResourceName = ModuleIntakes[0].resourceName;        //Temporarily added

                //Debug.Log("debugMode before: " + debugMode);
                if (bool.Parse(debugMode)) { Events["DEBUG_INTAKESWITCH"].guiActive = true; Events["DEBUG_INTAKESWITCH"].guiActiveEditor = true; Events["DEBUG_INTAKESWITCH"].active = true; Debug.Log("Intake Switch debugMode activated"); }
                else { Events["DEBUG_INTAKESWITCH"].guiActive = false; Events["DEBUG_INTAKESWITCH"].guiActiveEditor = false; Events["DEBUG_INTAKESWITCH"].active = false; /*Debug.Log("debugMode deactivated");*/ }
                //Debug.Log(
                //    "\ndebugMode after: " + debugMode +
                //    "\nguiActive after: " + Events["DEBUG_INTAKESWITCH"].guiActive +
                //    "\nguiActiveEditor after: " + Events["DEBUG_INTAKESWITCH"].guiActiveEditor +
                //    "\nactive after: " + Events["DEBUG_INTAKESWITCH"].active
                //    );


                //set settings to initialized
                _settingsInitialized = true;
                //Debug.Log("Intake Switcher settings loaded: " + _settingsInitialized);
            }
        }
        #endregion

        #region UpdatePart Intake Module
        private void updateIntake(bool calledByPlayer, string callingFunction = "player")
        {
            ConfigNode newIntakeNode = new ConfigNode();
            Part currentPart = this.part;
            int resIniAmount = 0;
            bool removethis = false;


            //foreach (PartModule moduleIntake in ModuleIntakes)
            //Apparently the foreach does not work properly with Intakes. Don't know why.
            for (int i = 0; i < ModuleIntakes.Count; i++)
            {
                //*****************************
                //Debug.Log("Intake Switcher: Update Resource Intake");

                //Define the node object
                ConfigNode IntakeNode = newIntakeNode;          //.GetNode("ModuleResourceIntake");
                ConfigNode IntakeResource = newIntakeNode;

                //Debug.Log("Confignode defined");
                //Debug.Log("checkForOxygen: " + arrIntakeNames[selectedIntake]);

                //Set new setting values
                if (!resourceNamesEmpty) { IntakeNode.SetValue("resourceName", arrIntakeNames[selectedIntake], true); }
                if (!checkForOxygenEmpty) { IntakeNode.SetValue("checkForOxygen", arrcheckForOxygen[selectedIntake], true); } else { IntakeNode.RemoveNode("checkForOxygen"); }

                //Debug.Log("Confignode value added");

                //Load changes (nodeobject) into the moduleIntake
                ModuleIntakes[i].Load(IntakeNode);
                
                //Debug.Log("Cleanout old resources");
                
                #region Intake Resource
                //Clean out any previous resources in the intake
                currentPart.Resources.Clear();
                //PartResource[] partResources = currentPart.GetComponents<PartResource>();
                //foreach (PartResource resource in partResources)
                //List<PartResource> resourcesDeleteList = new List<PartResource>();

                //currentPart.symmetryCounterparts;
                removethis = false;
                foreach (PartResource resource in currentPart.Resources)
                {
                    //Check if the resource is part of the switching resources, so that we do not destroy resources which are not intended for this switching
                    foreach (string inIntakeResource in arrIntakeNames)
                    {
                        if (inIntakeResource == resource.resourceName)
                        {
                            removethis = true;
                            break;
                        }
                    }

                    //currentPart.Resources.list.Remove(resource);
                    //if (removethis == true) { DestroyImmediate(resource., false); }              //*********COMMENTED OUT BECAUSE OF ERROR
                    //if (removethis == true) { Destroy(resource); }                              //*********COMMENTED OUT BECAUSE OF ERROR
                    //if (removethis == true) { PartResourceList[0].Remove(resource); }
                    //PartResourceList[0].;
                    //resource.;
                    //bool a;
                    //a = PartResourceList.Remove(resource);
                    if (removethis == true) { currentPart.Resources.Remove(resource); }
                    removethis = false;
                }

                resIniAmount = HighLogic.LoadedSceneIsFlight ? 0 : int.Parse(resMaxAmount);

                //Create Resource node
                IntakeResource.AddNode("RESOURCE");
                IntakeResource.AddValue("name", arrIntakeNames[selectedIntake]);
                IntakeResource.AddValue("amount", resIniAmount);
                IntakeResource.AddValue("maxAmount", int.Parse(resMaxAmount));
                //Add the resources
                currentPart.AddResource(IntakeResource);

                //Update the part resources
                //currentPart.Resources.UpdateList();
                GUIResourceName = ModuleIntakes[0].resourceName;
                #endregion

                //added check for called by player, since the ResourceDisplay update fails when called from the OnStart function.
                try
                { if (HighLogic.LoadedSceneIsFlight && calledByPlayer) { KSP.UI.Screens.ResourceDisplay.Instance.Refresh(); } }
                catch { Debug.LogError("Update of resource panel failed" + "\ncallingFunction: " + callingFunction + "\ncalledByPlayer: " + calledByPlayer); }
                //Debug.Log("Confignode Loaded");

            }
        }
        #endregion

        #region --------------------------------Debugging---------------------------------------
        [KSPEvent(active = false, guiActive = false, guiActiveEditor = false, guiName = "DEBUG")]
        public void DEBUG_INTAKESWITCH()
        {
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
                    "\npart.Resources[0].maxAmount: " + part.Resources[i].maxAmount +
                    "\nresMaxAmountEmpty: " + resMaxAmountEmpty
                    );
            }
            

        }
        #endregion

        public override string GetInfo()
        {
            return "AirIntake Switcher:\n" + resourceNames;
        }

    }
}
