using System;
using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    partial class GTI_MultiModeIntake : PartModule
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
            //InitializeSettings();
            InitializeSettings();
            if (selectedIntake == -1)
            {
                selectedIntake = 0;
                ChooseOption = selectedIntake.ToString();
                //_selectedIntakeOld = 0;
            }
            //http://forum.kerbalspaceprogram.com/index.php?/topic/135891-ui_chooseoption-oddities-when-displaying-long-names/

            initializeGUI();

            //updateIntake(false, "OnStart");
            updateIntake(false, "OnStart");
        }

        //private void InitializeSettings()
        //{
        //    if (!_settingsInitialized)
        //    {
        //        Debug.Log("MultiModeIntake: Loading Settings");
        //        Utilities Util = new Utilities();
        //        ModuleIntakes = part.FindModulesImplementing<ModuleResourceIntake>();

        //        #region Parse Arrays
        //        resourceNamesEmpty = Utilities.ArraySplitEvaluate(resourceNames, out arrIntakeNames, ';');
        //        checkForOxygenEmpty = Utilities.ArraySplitEvaluate(checkForOxygen, out arrcheckForOxygen, ';');
        //        #endregion
        //        #region Parse Strings
        //        resMaxAmountEmpty = Utilities.StringEvaluate(resMaxAmount, out resMaxAmount);

        //        //If the resMaxAmount is empty, try getting the maxAmount of first intake. If fails, then return a simple 2 Units.
        //        Debug.Log("MultiModeIntake: resMaxAmount");
        //        try
        //        { resMaxAmount = resMaxAmountEmpty ? ModuleIntakes[0].res.maxAmount.ToString() : resMaxAmount; }
        //        catch
        //        { resMaxAmount = "2"; }
        //        #endregion

        //        #region GUI Update
        //        Debug.Log("MultiModeIntake: Decide to initializeGUI");
        //        if (!resourceNamesEmpty) { initializeGUI(); }
        //        else { Debug.LogError("GTI_IntakeSwitch is missing settings for intake names."); return; }
        //        #endregion

        //        //Debug.Log("debugMode before: " + debugMode);
        //        Debug.Log("MultiModeIntake: Debugging?");
        //        if (bool.Parse(debugMode)) { Events["DEBUG_INTAKESWITCH"].guiActive = true; Events["DEBUG_INTAKESWITCH"].guiActiveEditor = true; Events["DEBUG_INTAKESWITCH"].active = true; Debug.Log("Intake Switch debugMode activated"); }
        //        else { Events["DEBUG_INTAKESWITCH"].guiActive = false; Events["DEBUG_INTAKESWITCH"].guiActiveEditor = false; Events["DEBUG_INTAKESWITCH"].active = false; /*Debug.Log("debugMode deactivated");*/ }

        //        //set settings to initialized
        //        _settingsInitialized = true;
        //        Debug.Log("MultiModeIntake: _settingsInitialized");
        //    }
        //}

        private void InitializeSettings()
        {
            if (!_settingsInitialized)
            {
                //Debug.Log("MultiModeIntake: Loading Settings (2)");
                //Utilities Util = new Utilities();
                ModuleIntakes = part.FindModulesImplementing<ModuleResourceIntake>();

                #region Parse Arrays
                //Debug.Log("MultiModeIntake: Parse Arrays");
                resourceNamesEmpty = Utilities.ArraySplitEvaluate(resourceNames, out arrIntakeNames, ';');
                checkForOxygenEmpty = Utilities.ArraySplitEvaluate(checkForOxygen, out arrcheckForOxygen, ';');
                #endregion
                #region Parse Strings
                resMaxAmountEmpty = Utilities.StringEvaluate(resMaxAmount, out resMaxAmount);

                //If the resMaxAmount is empty, try getting the maxAmount of first intake. If fails, then return a simple 2 Units.
                //Debug.Log("MultiModeIntake: resMaxAmount");
                try
                { resMaxAmount = resMaxAmountEmpty ? ModuleIntakes[0].res.maxAmount.ToString() : resMaxAmount; }
                catch
                { resMaxAmount = "2"; }
                #endregion

                #region GUI Update
                //Debug.Log("MultiModeIntake: Decide to initializeGUI");
                if (!resourceNamesEmpty) { initializeGUI(); }
                else { Debug.LogError("GTI_IntakeSwitch is missing settings for intake names."); return; }
                #endregion


                //Debug.Log("debugMode before: " + debugMode);
                //Debug.Log("MultiModeIntake: Debugging?");
                if (bool.Parse(debugMode)) { Events["DEBUG_INTAKESWITCH"].guiActive = true; Events["DEBUG_INTAKESWITCH"].guiActiveEditor = true; Events["DEBUG_INTAKESWITCH"].active = true; Debug.Log("Intake Switch debugMode activated"); }
                else { Events["DEBUG_INTAKESWITCH"].guiActive = false; Events["DEBUG_INTAKESWITCH"].guiActiveEditor = false; Events["DEBUG_INTAKESWITCH"].active = false; /*Debug.Log("debugMode deactivated");*/ }

                //set settings to initialized
                _settingsInitialized = true;
                //Debug.Log("MultiModeIntake: _settingsInitialized");
            }
        }
        #endregion

        #region UpdatePart Intake Module
        private void updateIntake(bool calledByPlayer, string callingFunction = "player")
        {
            ConfigNode newIntakeNode = new ConfigNode();
            Part currentPart = this.part;
            float resIniAmount = 0f;
            bool removethis = false;
            

            //foreach (PartModule moduleIntake in ModuleIntakes)
            //Apparently the foreach does not work properly with Intakes. Don't know why.
            //Debug.Log("MultiModeIntake: Before ModuleIntakes count");
            //Debug.Log("MultiModeIntake: ModuleIntakes count: " + ModuleIntakes.Count);

            //Debug.Log("MultiModeIntake: Cycle ModuleIntakes");
            //for (int i = 0; i < ModuleIntakes.Count; i++)
            for (int i = ModuleIntakes.Count - 1; i == 0; i--)
            {
                if (i == 0)
                {
                    //*****************************
                    //Debug.Log("Intake Switcher: Update Resource Intake");

                    //Define the node object
                    ConfigNode IntakeNode = newIntakeNode;          //.GetNode("ModuleResourceIntake");

                    //Debug.Log("MultiModeIntake: Confignode defined " + i);
                    //Debug.Log("checkForOxygen: " + arrIntakeNames[selectedIntake]);

                    //Set new setting values
                    if (!resourceNamesEmpty) { IntakeNode.SetValue("resourceName", arrIntakeNames[selectedIntake], true); }
                    if (!checkForOxygenEmpty) { IntakeNode.SetValue("checkForOxygen", arrcheckForOxygen[selectedIntake], true); } else { IntakeNode.RemoveNode("checkForOxygen"); }

                    //Debug.Log("Confignode value added");

                    //Load changes (nodeobject) into the moduleIntake
                    //Debug.Log("MultiModeIntake: Load IntakeNode " + i);
                    ModuleIntakes[i].Load(IntakeNode);
                }
            }

            #region Intake Resource
            ConfigNode IntakeResource = newIntakeNode;

            //Clean out any previous resources in the intake
            //Debug.Log("MultiModeIntake: currentPart.Resources.Clear()");
            currentPart.Resources.Clear();
            //Debug.Log("Cleanout old resources");

            //PartResource[] partResources = currentPart.GetComponents<PartResource>();
            //foreach (PartResource resource in partResources)
            //List<PartResource> resourcesDeleteList = new List<PartResource>();

            //currentPart.symmetryCounterparts;
            //Debug.Log("MultiModeIntake: Update Resources");
            removethis = false;
            foreach (PartResource resource in currentPart.Resources)
            {
                //Check if the resource is part of the switching resources, so that we do not destroy resources which are not intended for this switching
                //Debug.Log("MultiModeIntake: foreach (string inIntakeResource in arrIntakeNames)");
                foreach (string inIntakeResource in arrIntakeNames)
                {
                    //Debug.Log("MultiModeIntake: inIntakeResource - " + inIntakeResource);
                    if (inIntakeResource == resource.resourceName)
                    {
                        removethis = true;
                        break;
                    }
                }
                //If the resource belongs to the intake, then remove it
                //Debug.Log("MultiModeIntake: currentPart.Resources.Remove - " + removethis);
                if (removethis == true) { currentPart.Resources.Remove(resource); }
                removethis = false;
            }

            resIniAmount = HighLogic.LoadedSceneIsFlight ? 0f : Single.Parse(resMaxAmount);

            //Create Resource node
            IntakeResource.AddNode("RESOURCE");
            IntakeResource.AddValue("name", arrIntakeNames[selectedIntake]);
            IntakeResource.AddValue("amount", resIniAmount);
            IntakeResource.AddValue("maxAmount", Single.Parse(resMaxAmount));

            //Add the resources
            //Debug.Log("MultiModeIntake: Add Resource");
            currentPart.AddResource(IntakeResource);
            IntakeResource.ClearNodes();
            IntakeResource.ClearValues();


            //Update the part resources
            //currentPart.Resources.UpdateList();
            //GUIResourceName = ModuleIntakes[0].resourceName;
            #endregion

            //added check for called by player, since the ResourceDisplay update fails when called from the OnStart function.
            //Debug.Log("MultiModeIntake: ResourceDisplay update");
            try
            { if (HighLogic.LoadedSceneIsFlight && calledByPlayer) { KSP.UI.Screens.ResourceDisplay.Instance.Refresh(); } }
            catch { Debug.LogError("[GTI] Update of resource panel failed" + "\tcallingFunction: " + callingFunction + "\tcalledByPlayer: " + calledByPlayer); }
            //Debug.Log("Confignode Loaded");
        }


        //private void updateIntake(bool calledByPlayer, string callingFunction = "player")
        //{
        //    ConfigNode newIntakeNode = new ConfigNode();
        //    Part currentPart = this.part;
        //    int resIniAmount = 0;
        //    bool removethis = false;


        //    //foreach (PartModule moduleIntake in ModuleIntakes)
        //    //Apparently the foreach does not work properly with Intakes. Don't know why.
        //    for (int i = 0; i < ModuleIntakes.Count; i++)
        //    {
        //        //*****************************
        //        //Debug.Log("Intake Switcher: Update Resource Intake");

        //        //Define the node object
        //        ConfigNode IntakeNode = newIntakeNode;          //.GetNode("ModuleResourceIntake");
        //        ConfigNode IntakeResource = newIntakeNode;

        //        //Debug.Log("Confignode defined");
        //        //Debug.Log("checkForOxygen: " + arrIntakeNames[selectedIntake]);

        //        //Set new setting values
        //        if (!resourceNamesEmpty) { IntakeNode.SetValue("resourceName", arrIntakeNames[selectedIntake], true); }
        //        if (!checkForOxygenEmpty) { IntakeNode.SetValue("checkForOxygen", arrcheckForOxygen[selectedIntake], true); } else { IntakeNode.RemoveNode("checkForOxygen"); }

        //        //Debug.Log("Confignode value added");

        //        //Load changes (nodeobject) into the moduleIntake
        //        ModuleIntakes[i].Load(IntakeNode);

        //        //Debug.Log("Cleanout old resources");

        //        #region Intake Resource
        //        //Clean out any previous resources in the intake
        //        currentPart.Resources.Clear();
        //        //PartResource[] partResources = currentPart.GetComponents<PartResource>();
        //        //foreach (PartResource resource in partResources)
        //        //List<PartResource> resourcesDeleteList = new List<PartResource>();

        //        //currentPart.symmetryCounterparts;
        //        removethis = false;
        //        foreach (PartResource resource in currentPart.Resources)
        //        {
        //            //Check if the resource is part of the switching resources, so that we do not destroy resources which are not intended for this switching
        //            foreach (string inIntakeResource in arrIntakeNames)
        //            {
        //                if (inIntakeResource == resource.resourceName)
        //                {
        //                    removethis = true;
        //                    break;
        //                }
        //            }
        //            //If the resource belongs to the intake, then remove it
        //            if (removethis == true) { currentPart.Resources.Remove(resource); }
        //            removethis = false;
        //        }

        //        resIniAmount = HighLogic.LoadedSceneIsFlight ? 0 : int.Parse(resMaxAmount);

        //        //Create Resource node
        //        IntakeResource.AddNode("RESOURCE");
        //        IntakeResource.AddValue("name", arrIntakeNames[selectedIntake]);
        //        IntakeResource.AddValue("amount", resIniAmount);
        //        IntakeResource.AddValue("maxAmount", int.Parse(resMaxAmount));
        //        //Add the resources
        //        currentPart.AddResource(IntakeResource);

        //        //Update the part resources
        //        //currentPart.Resources.UpdateList();
        //        GUIResourceName = ModuleIntakes[0].resourceName;
        //        #endregion

        //        //added check for called by player, since the ResourceDisplay update fails when called from the OnStart function.
        //        try
        //        { if (HighLogic.LoadedSceneIsFlight && calledByPlayer) { KSP.UI.Screens.ResourceDisplay.Instance.Refresh(); } }
        //        catch { Debug.LogError("Update of resource panel failed" + "\ncallingFunction: " + callingFunction + "\ncalledByPlayer: " + calledByPlayer); }
        //        //Debug.Log("Confignode Loaded");

        //    }
        //}
        #endregion


        public override string GetInfo()
        {
            return "GTI Multi Mode Intakes:\n" + resourceNames;
        }

    }
}
