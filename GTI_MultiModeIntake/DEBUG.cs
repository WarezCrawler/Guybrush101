//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using GTI.GenericFunctions;

//namespace GTI
//{
//    partial class GTI_MultiModeIntakeOLD : PartModule
//    {
//        #region --------------------------------Debugging---------------------------------------
//        [KSPEvent(active = false, guiActive = false, guiActiveEditor = true, guiName = "DEBUG")]
//        public void DEBUG_INTAKESWITCH()
//        {
//            /* Debugging Area */

//            //Debug.Log("Update GUI");
//            //GUIResourceName = ModuleIntakes[0].resourceName;
//            //Debug.Log("GUI Updated");

//            Debug.Log(
//                "\nIntakeAir id: " + PartResourceLibrary.Instance.GetDefinition("IntakeAir").id +
//                "\nIntakeAir density: " + PartResourceLibrary.Instance.GetDefinition("IntakeAir").density +
//                "\nIntakeAtm id: " + PartResourceLibrary.Instance.GetDefinition("IntakeAtm").id +
//                "\nIntakeAtm density: " + PartResourceLibrary.Instance.GetDefinition("IntakeAtm").density
//            );

//            Debug.Log(
//                "\nModuleIntakes[0].resourceName: " + ModuleIntakes[0].resourceName +
//                "\nresourceId: " + ModuleIntakes[0].resourceId +
//                "\nresourceDef: " + ModuleIntakes[0].resourceDef +
//                "\nres: " + ModuleIntakes[0].res +
//                "\nresourceUnits: " + ModuleIntakes[0].resourceUnits +
//                "\ncheckForOxygen: " + ModuleIntakes[0].checkForOxygen +
//                "\narea: " + ModuleIntakes[0].area +
//                "\nairFlow: " + ModuleIntakes[0].airFlow +
//                "\nModuleIntakes.Count: " + ModuleIntakes.Count
//                );

//            for (int i = 0; i < part.Resources.Count; i++)
//            {
//                Debug.Log(
//                    "\npart.Resources[0].resourceName: " + part.Resources[i].resourceName +
//                    "\npart.Resources[0].amount: " + part.Resources[i].amount +
//                    "\npart.Resources[0].maxAmount: " + part.Resources[i].maxAmount +
//                    "\nresMaxAmountEmpty: " + resMaxAmountEmpty
//                    );
//            }

//            for (int i = 0; i < ModuleIntakes[0].Fields.Count; i++)
//            {
//                //moduleEngine.Fields[i].guiName;

//                if (ModuleIntakes[0].Fields[i].guiActive)
//                {
//                    Debug.Log(
//                        "\nmoduleEngine.Fields[" + i + "]" +
//                        "\nguiName: " + ModuleIntakes[0].Fields[i].guiName +
//                        "\nname: " + ModuleIntakes[0].Fields[i].name +
//                        "\noriginalValue: " + ModuleIntakes[0].Fields[i].originalValue +
//                        "\nisPersistant: " + ModuleIntakes[0].Fields[i].isPersistant +
//                        "\nguiActive: " + ModuleIntakes[0].Fields[i].guiActive +
//                        "\nguiActiveEditor: " + ModuleIntakes[0].Fields[i].guiActiveEditor +
//                        "\nguiFormat: " + ModuleIntakes[0].Fields[i].guiFormat +
//                        "\nguiUnits: " + ModuleIntakes[0].Fields[i].guiUnits +
//                        "\nHasInterface: " + ModuleIntakes[0].Fields[i].HasInterface +
//                        "\nhost: " + ModuleIntakes[0].Fields[i].host +
//                        "\nuiControlEditor: " + ModuleIntakes[0].Fields[i].uiControlEditor +
//                        "\nuiControlFlight: " + ModuleIntakes[0].Fields[i].uiControlFlight +
//                        "\nuiControlOnly: " + ModuleIntakes[0].Fields[i].uiControlOnly +
//                        "\n");
//                }
//            }

//        }
//        #endregion

//    }
//}
















//using System;
//using System.Collections.Generic;
//using GTI.Config;
//using static GTI.GTIConfig;
//using GTI.GenericFunctions;
//using static GTI.GenericFunctions.Utilities;
////using static GTI.GenericFunctions.PhysicsUtilities;

//namespace GTI
//{
//    public class IntakeModes : MultiMode
//    {
//        //public int moduleIndex { get; set; }
//        //public string ID;
//        //public string Name;
//        //public bool modeDisabled = false;
//        public string resourceName;
//        public bool checkForOxygen;
//        public float areas;
//        public float intakeSpeed;
//        public string intakeTransformNames;
//        public ConfigNode machCurves;
//        public float resMaxAmount;

//    }
//    class GTI_MultiModeIntake : GTI_MultiMode<IntakeModes>
//    {
//        /* Example on an Intake Module
//        name = ModuleResourceIntake
//        resourceName = IntakeAir

//        checkForOxygen = true
//        area = 0.0031
//        intakeSpeed = 15
//        intakeTransformName = Intake
//        machCurve
//        {
//            key = 1 1 0 0
//            key = 1.5 0.9 -0.4312553 -0.4312553
//            key = 2.5 0.45 -0.5275364 -0.5275364
//            key = 3.5 0.1 0 0
//        }
//        */

//        [KSPField]
//        public string resourceNames = string.Empty;
//        [KSPField]
//        public string GUINames = string.Empty;
//        [KSPField]
//        public string checkForOxygen = string.Empty;
//        //[KSPField]
//        //public string areas = string.Empty;                      //"0.0031, 0.0031";
//        //[KSPField]
//        //public string intakeSpeeds = string.Empty;              //"15, 15";
//        //[KSPField]
//        //public string intakeTransformNames = string.Empty;       //"Intake, Intake";
//        //[KSPField]
//        //public string machCurves = string.Empty;
//        [KSPField]
//        public string resMaxAmount = string.Empty;

//        #region Booleans for existence checks
//        private bool resourceNamesEmpty = false;
//        //private bool resMaxAmountEmpty = false;
//        private bool checkForOxygenEmpty = false;

//        #endregion

//        //private string[] arrIntakeNames, arrcheckForOxygen;

//        public List<ModuleResourceIntake> ModuleIntakes;

//        protected override void initializeSettings()
//        {
//            GTIDebug.Log("GTI_MultiModeIntake --> initializeSettings()", iDebugLevel.DebugInfo);
//            GTIDebug.Log(GetPartModuleConfig(this.part, "MODULE", "name", "GTI_MultiModeIntake").ToString(), iDebugLevel.DebugInfo);
//            //GetPartModuleConfig(this.part, "MODULE", "name", "GTI_MultiModeEngine")

//            string[] arrIntakeNames, arrcheckForOxygen;

//            //Find resourceIntake modules
//            ModuleIntakes = part.FindModulesImplementing<ModuleResourceIntake>();

//            //Define arrays for data structures
//            resourceNamesEmpty = Utilities.ArraySplitEvaluate(resourceNames, out arrIntakeNames, ';');
//            checkForOxygenEmpty = Utilities.ArraySplitEvaluate(checkForOxygen, out arrcheckForOxygen, ';');

//            //Consistency checks
//            if (ModuleIntakes.Count != 1) { GTIDebug.LogError("GTI_MultiModeIntake does not support more than one ModuleResourceIntake to be defined in part. Initialization terminated."); }
//            if (resourceNamesEmpty) { GTIDebug.LogError("GTI_MultiModeIntake initialization failed because no resourceNames where defined and inconsistent"); }
//            if (!checkForOxygenEmpty && arrcheckForOxygen.Length != arrIntakeNames.Length) checkForOxygenEmpty = true;  //Lenght was wrong, so we ignore the contents


//            modes = new List<IntakeModes>(arrIntakeNames.Length);
//            GTIDebug.Log(this.GetType().Name + " --> arrIntakeNames.Length; " + arrIntakeNames.Length, iDebugLevel.DebugInfo);
//            for (int i = 0; i < arrIntakeNames.Length; i++)
//            {
//                GTIDebug.Log("for (int i = 0; i < ModuleIntakes.Count; i++): " + i, iDebugLevel.DebugInfo);
//                modes.Add(new IntakeModes()
//                {
//                    moduleIndex = i,
//                    ID = i.ToString(),
//                    Name = arrIntakeNames[i],
//                    resourceName = arrIntakeNames[i],
//                    checkForOxygen = checkForOxygenEmpty ? ModuleIntakes[0].checkForOxygen : bool.Parse(arrcheckForOxygen[i])
//                });
//                GTIDebug.Log("modes[" + i + "].ID --> " + modes[i].ID, iDebugLevel.DebugInfo);
//                GTIDebug.Log("modes[" + i + "].Name --> " + modes[i].Name, iDebugLevel.DebugInfo);
//            }

//            //force no use af animation groups (it's not imlpemented, I don't believe it's relevant)
//            useModuleAnimationGroup = false;
//        }


//        public override void updateMultiMode(bool silentUpdate = false)
//        {
//            GTIDebug.Log(this.GetType().Name + " --> GTI_MultiModeIntake: updateMultiMode() --> Begin", iDebugLevel.High);
//            Part currentPart = this.part;
//            ConfigNode IntakeNode = new ConfigNode();

//            if (silentUpdate == false) writeScreenMessage();

//            //ModuleIntakes[0].resourceName = modes[selectedMode].resourceName;
//            //ModuleIntakes[0].checkForOxygen = modes[selectedMode].checkForOxygen;
//            //ModuleIntakes[0].


//            GTIDebug.Log("TEMP status " + ModuleIntakes[0].status);
//            GTIDebug.Log("TEMP intakeEnabled " + ModuleIntakes[0].intakeEnabled);
//            GTIDebug.Log("TEMP isActiveAndEnabled " + ModuleIntakes[0].isActiveAndEnabled);
//            GTIDebug.Log("TEMP isEnabled " + ModuleIntakes[0].isEnabled);
//            GTIDebug.Log("TEMP enabled " + ModuleIntakes[0].enabled);
//            GTIDebug.Log("TEMP resourceName --> Deactivate()");
//            ModuleIntakes[0].Deactivate();
//            GTIDebug.Log("TEMP status " + ModuleIntakes[0].status);
//            GTIDebug.Log("TEMP intakeEnabled " + ModuleIntakes[0].intakeEnabled);
//            GTIDebug.Log("TEMP isActiveAndEnabled " + ModuleIntakes[0].isActiveAndEnabled);
//            GTIDebug.Log("TEMP isEnabled " + ModuleIntakes[0].isEnabled);
//            GTIDebug.Log("TEMP enabled " + ModuleIntakes[0].enabled);
//            GTIDebug.Log("TEMP resourceName --> Activate()");
//            ModuleIntakes[0].Activate();
//            GTIDebug.Log("TEMP status " + ModuleIntakes[0].status);
//            GTIDebug.Log("TEMP intakeEnabled " + ModuleIntakes[0].intakeEnabled);
//            GTIDebug.Log("TEMP isActiveAndEnabled " + ModuleIntakes[0].isActiveAndEnabled);
//            GTIDebug.Log("TEMP isEnabled " + ModuleIntakes[0].isEnabled);
//            GTIDebug.Log("TEMP enabled " + ModuleIntakes[0].enabled);
//            //GTIDebug.Log("TEMP resourceName " + ModuleIntakes[0].resourceName);
//            //GTIDebug.Log("TEMP resourceId " + ModuleIntakes[0].resourceId);
//            //GTIDebug.Log("TEMP resourceDef " + ModuleIntakes[0].resourceDef);
//            //GTIDebug.Log("TEMP resourceUnits " + ModuleIntakes[0].resourceUnits);
//            //GTIDebug.Log("TEMP area " + ModuleIntakes[0].area);
//            //GTIDebug.Log("TEMP intakeSpeed " + ModuleIntakes[0].intakeSpeed);

//            if (!resourceNamesEmpty) { IntakeNode.SetValue("resourceName", modes[selectedMode].resourceName, true); }
//            if (!checkForOxygenEmpty) { IntakeNode.SetValue("checkForOxygen", modes[selectedMode].checkForOxygen, true); } else { IntakeNode.RemoveNode("checkForOxygen"); }
//            ModuleIntakes[0].Load(IntakeNode);

//            //GTIDebug.Log("TEMP resourceName " + ModuleIntakes[0].resourceName);
//            //GTIDebug.Log("TEMP resourceId " + ModuleIntakes[0].resourceId);
//            //GTIDebug.Log("TEMP resourceDef " + ModuleIntakes[0].resourceDef);
//            //GTIDebug.Log("TEMP resourceUnits " + ModuleIntakes[0].resourceUnits);
//            //GTIDebug.Log("TEMP area " + ModuleIntakes[0].area);
//            //GTIDebug.Log("TEMP intakeSpeed " + ModuleIntakes[0].intakeSpeed);

//            //part.Resources[0].resourceName = modes[selectedMode].resourceName;
//            //part.Resources[0].amount = 0;

//            //bool removethis = false;
//            //foreach (PartResource resource in currentPart.Resources)
//            //{
//            //    //Check if the resource is part of the switching resources, so that we do not destroy resources which are not intended for this switching
//            //    //Debug.Log("MultiModeIntake: foreach (string inIntakeResource in arrIntakeNames)");
//            //    //foreach (string inIntakeResource in arrIntakeNames)
//            //    for (int i = 0; i < modes.Count; i++)
//            //    {
//            //        if (modes[i].resourceName == resource.resourceName)
//            //        {
//            //            GTIDebug.Log("Remove Resource: " + resource.resourceName, iDebugLevel.High);
//            //            removethis = true;
//            //            break;
//            //        }
//            //    }
//            //    //If the resource belongs to the intake, then remove it
//            //    //Debug.Log("MultiModeIntake: currentPart.Resources.Remove - " + removethis);
//            //    if (removethis == true) { currentPart.Resources.Remove(resource); }
//            //    removethis = false;
//            //}

//            #region Create new Resource node
//            //List<ConfigNode> IntakeResources = new List<ConfigNode>();
//            ConfigNode IntakeResource = new ConfigNode("RESOURCE");
//            float resMaxAmount = (float)ModuleIntakes[0].res.maxAmount;
//            float resIniAmount = HighLogic.LoadedSceneIsFlight ? 0f : resMaxAmount;

//            //Create Resource node
//            //IntakeResource.AddNode("RESOURCE");
//            IntakeResource.AddValue("name", modes[selectedMode].resourceName);
//            IntakeResource.AddValue("amount", resIniAmount);
//            IntakeResource.AddValue("maxAmount", resMaxAmount);

//            //Add the resources
//            GTIDebug.Log("MultiModeIntake: Add Resource\n" + IntakeResource.ToString(), iDebugLevel.DebugInfo);
//            //Clear all resources since I get null ref error when I do not do this
//            currentPart.Resources.Clear();
//            currentPart.AddResource(IntakeResource);
//            //IntakeResource.ClearNodes();
//            //IntakeResource.ClearValues();
//            #endregion

//            try
//            { if (HighLogic.LoadedSceneIsFlight && !silentUpdate) { KSP.UI.Screens.ResourceDisplay.Instance.Refresh(); } }
//            catch { GTIDebug.LogError("Update resource panel failed."); }

//            //KSP.UI.Screens.ResourceDisplay.Instance.;
//        }

//        protected override void writeScreenMessage()
//        {
//            writeScreenMessage(
//                Message: "Intake mode: " + modes[selectedMode].Name,
//                messagePosition: messagePosition,
//                duration: 3f
//                );
//        }

//        //public override void OnDestroy()
//        //{
//        //    base.OnDestroy();
//        //}

//        protected override void ModuleAnimationGroupEvent_DisableModules() { throw new NotImplementedException(); }

//        [KSPEvent(name = "IntakeActivate", guiName = "Open Intake (GTI)", active = true, externalToEVAOnly = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActive = true, guiActiveEditor = true)]
//        public void IntakeActivate()
//        {
//            GTIDebug.Log("TEMP Before --> Activate()");
//            GTIDebug.Log("TEMP status " + ModuleIntakes[0].status);
//            GTIDebug.Log("TEMP intakeEnabled " + ModuleIntakes[0].intakeEnabled);
//            GTIDebug.Log("TEMP isActiveAndEnabled " + ModuleIntakes[0].isActiveAndEnabled);
//            GTIDebug.Log("TEMP isEnabled " + ModuleIntakes[0].isEnabled);
//            GTIDebug.Log("TEMP enabled " + ModuleIntakes[0].enabled);
//            GTIDebug.Log("TEMP resourceName --> Activate()");
//            ModuleIntakes[0].Activate();
//            ModuleIntakes[0].enabled = true;
//            ModuleIntakes[0].isEnabled = true;
//            ModuleIntakes[0].status = "Nominal";
//            GTIDebug.Log("TEMP status " + ModuleIntakes[0].status);
//            GTIDebug.Log("TEMP intakeEnabled " + ModuleIntakes[0].intakeEnabled);
//            GTIDebug.Log("TEMP isActiveAndEnabled " + ModuleIntakes[0].isActiveAndEnabled);
//            GTIDebug.Log("TEMP isEnabled " + ModuleIntakes[0].isEnabled);
//            GTIDebug.Log("TEMP enabled " + ModuleIntakes[0].enabled);
//        }
//        [KSPEvent(name = "IntakeDeactivate", guiName = "Open Intake (GTI)", active = true, externalToEVAOnly = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActive = true, guiActiveEditor = true)]
//        public void IntakeDeactivate()
//        {
//            GTIDebug.Log("TEMP Before --> Deactivate()");
//            GTIDebug.Log("TEMP status " + ModuleIntakes[0].status);
//            GTIDebug.Log("TEMP intakeEnabled " + ModuleIntakes[0].intakeEnabled);
//            GTIDebug.Log("TEMP isActiveAndEnabled " + ModuleIntakes[0].isActiveAndEnabled);
//            GTIDebug.Log("TEMP isEnabled " + ModuleIntakes[0].isEnabled);
//            GTIDebug.Log("TEMP enabled " + ModuleIntakes[0].enabled);
//            GTIDebug.Log("TEMP --> Deactivate()");
//            ModuleIntakes[0].Deactivate();
//            ModuleIntakes[0].enabled = false;
//            //ModuleIntakes[0].isEnabled = false;
//            ModuleIntakes[0].status = "Closed";
//            GTIDebug.Log("TEMP status " + ModuleIntakes[0].status);
//            GTIDebug.Log("TEMP intakeEnabled " + ModuleIntakes[0].intakeEnabled);
//            GTIDebug.Log("TEMP isActiveAndEnabled " + ModuleIntakes[0].isActiveAndEnabled);
//            GTIDebug.Log("TEMP isEnabled " + ModuleIntakes[0].isEnabled);
//            GTIDebug.Log("TEMP enabled " + ModuleIntakes[0].enabled);
//        }
//    }
//}
///* NOTES
//.Events
//GUIName: Close Intake
//id: -3298156
//name: Deactivate
//active: True
//assigned: False
//category: 
//externalToEVAOnly: True
//guiActive: True
//guiActiveEditor: True
//guiActiveUncommand: False
//guiActiveUnfocused: False
//guiIcon: Close Intake
//unfocusedRange: 2

//[LOG 14:39:45.346]
//[GTI] 
//.Events
//GUIName: Open Intake
//id: -1591330541
//name: Activate
//active: False
//assigned: False
//category: 
//externalToEVAOnly: True
//guiActive: True
//guiActiveEditor: True
//guiActiveUncommand: False
//guiActiveUnfocused: False
//guiIcon: Open Intake
//unfocusedRange: 2

//[LOG 14:39:45.346]
//[GTI] 
//.Events
//GUIName: Disable Staging
//id: 1164541479
//name: ToggleStaging
//active: True
//assigned: False
//category: 
//externalToEVAOnly: True
//guiActive: False
//guiActiveEditor: False
//guiActiveUncommand: False
//guiActiveUnfocused: False
//guiIcon: Disable Staging
//unfocusedRange: 2
//*/
