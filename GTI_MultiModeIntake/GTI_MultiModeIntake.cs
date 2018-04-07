using System;
using System.Collections.Generic;
using static GTI.GTIConfig;
//using GTI.GenericFunctions;
using static GTI.Utilities;
//using static GTI.GenericFunctions.PhysicsUtilities;

namespace GTI
{
    public class IntakeModes : IMultiMode
    {

        public int moduleIndex { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }

        //public bool modeDisabled = false;
        public string resourceName;


        public override string ToString()
        {
            return base.ToString() + "\t" + resourceName;
        }
    }
    class GTI_MultiModeIntake : GTI_MultiMode<IntakeModes>
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
        public string GUINames = string.Empty;

        [KSPField(isPersistant = true)]
        public bool selectedModeStatus = true;

        [KSPField]
        public string resMaxAmount = string.Empty;

        [KSPField]
        public bool preserveResourceNodes = false;

        public List<ModuleResourceIntake> ModuleIntakes;

        protected override void initializeSettings()
        {
            GTIDebug.Log("GTI_MultiModeIntake --> initializeSettings()", iDebugLevel.DebugInfo);
            GTIDebug.Log(part.GetPartModuleConfig("MODULE", "name", "GTI_MultiModeIntake").ToString(), iDebugLevel.DebugInfo);

            ConfigNode[] ResourceNodes = part.GetPartModuleConfigs("RESOURCE");
            GTIDebug.Log(ResourceNodes.ToStringExt(), iDebugLevel.DebugInfo);
            //foreach(ConfigNode n in ResourceNodes)
            //{
            //    GTIDebug.Log(n.ToString(), iDebugLevel.DebugInfo);
            //    GTIDebug.Log(n.values.GetValue("name"), iDebugLevel.DebugInfo);
            //}

            //GetPartModuleConfig(this.part, "MODULE", "name", "GTI_MultiModeEngine")

            //Find resourceIntake modules
            ModuleIntakes = part.FindModulesImplementing<ModuleResourceIntake>();

            modes = new List<IntakeModes>(ModuleIntakes.Count);
            GTIDebug.Log(this.GetType().Name + " --> ModuleIntakes.Count; " + ModuleIntakes.Count, iDebugLevel.DebugInfo);
            for (int i = 0; i < ModuleIntakes.Count; i++)
            {
                GTIDebug.Log("for (int i = 0; i < ModuleIntakes.Count; i++): " + i, iDebugLevel.DebugInfo);
                modes.Add(new IntakeModes()
                {
                    moduleIndex = i,
                    ID = i.ToString(),
                    Name = ModuleIntakes[i].resourceName,
                    resourceName = ModuleIntakes[i].resourceName
                });
                GTIDebug.Log("modes[" + i + "].moduleIndex --> " + modes[i].moduleIndex, iDebugLevel.DebugInfo);
                GTIDebug.Log("modes[" + i + "].ID --> " + modes[i].ID, iDebugLevel.DebugInfo);
                GTIDebug.Log("modes[" + i + "].Name --> " + modes[i].Name, iDebugLevel.DebugInfo);
            }

            //Disable Events, as these should be handled by the multimode module
            for (int i = 0; i < ModuleIntakes.Count; i++)
            {
                ModuleIntakes[i].Events["Deactivate"].guiActive = false;
                ModuleIntakes[i].Events["Deactivate"].guiActiveEditor = false;
                ModuleIntakes[i].Events["Activate"].guiActive = false;
                ModuleIntakes[i].Events["Activate"].guiActiveEditor = false;

                ModuleIntakes[i].Actions["ToggleAction"].active = false;
            }
            
            //force no use af animation groups (it's not imlpemented, I don't believe it's relevant)
            useModuleAnimationGroup = false;
        }
        

        public override void updateMultiMode(bool silentUpdate = false)
        {
            GTIDebug.Log(this.GetType().Name + " --> GTI_MultiModeIntake: updateMultiMode() --> Begin", iDebugLevel.High);
            Part currentPart = this.part;

            if (silentUpdate == false) writeScreenMessage();

            for (int i = 0; i < ModuleIntakes.Count; i++)
            {
                if (i == modes[selectedMode].moduleIndex)
                {
                    GTIDebug.Log("GTI_MultiMode (" + (silentUpdate ? "silent" : "non-silent") + "): Activate Module [" + modes[i].moduleIndex + "] --> " + ModuleIntakes[modes[i].moduleIndex].resourceName, iDebugLevel.High);
                    GTIDebug.Log("selectedMode Intake: " + selectedMode + "\tselectedModeStatus: " + selectedModeStatus);
                    if (selectedModeStatus)
                    {
                        GTIDebug.Log("Activating Intake Mode: " + modes[selectedMode].resourceName);
                        ModuleIntakes[i].intakeEnabled = true;
                    }
                    else
                    {
                        ModuleIntakes[i].intakeEnabled = false;
                    }
                    ModuleIntakes[i].enabled = true;
                    ModuleIntakes[i].isEnabled = true;
                }
                else
                {
                    GTIDebug.Log("GTI_MultiMode (" + (silentUpdate ? "silent" : "non-silent") + "): Deactivate Module [" + modes[i].moduleIndex + "] --> " + ModuleIntakes[modes[i].moduleIndex].resourceName, iDebugLevel.High);
                    ModuleIntakes[i].enabled = false;
                    ModuleIntakes[i].isEnabled = false;         //=> FixedUpdate() will update status = "Closed" and exit
                    ModuleIntakes[i].intakeEnabled = false;
                }
            }
            this.Events["IntakeActivate"].active = !selectedModeStatus;
            this.Events["IntakeDeactivate"].active = selectedModeStatus;

            #region Create new Resource node
            //Only handle resources if preservation is not activated
            if (!preserveResourceNodes)
            {
                //List<ConfigNode> IntakeResources = new List<ConfigNode>();
                ConfigNode IntakeResource = new ConfigNode("RESOURCE");
                float resMaxAmount = (float)ModuleIntakes[modes[selectedMode].moduleIndex].res.maxAmount;
                if (resMaxAmount <= 0) resMaxAmount = 1f;
                float resIniAmount = HighLogic.LoadedSceneIsFlight ? 0f : resMaxAmount;

                //Create Resource node
                IntakeResource.AddValue("name", modes[selectedMode].resourceName);
                IntakeResource.AddValue("amount", resIniAmount);
                IntakeResource.AddValue("maxAmount", resMaxAmount);

                //All resource properties
                //partResource.amount = amount;
                //partResource.maxAmount = maxAmount;
                //partResource.flowState = flowState;
                //partResource.isTweakable = isTweakable;
                //partResource.hideFlow = hideFlow;
                //partResource.isVisible = isVisible;
                //partResource.flowMode = flow;


                //Clear all resources since I get null ref error when I do not do this
                //currentPart.Resources.Clear();
                bool preserveResource;
                // remove all target resources
                List<PartResource> resourcesDeleteList = new List<PartResource>();
                foreach (PartResource resource in currentPart.Resources)
                {
                    preserveResource = true;
                    GTIDebug.Log("Check for resource removal: " + resource.resourceName, iDebugLevel.DebugInfo);
                    for (int j = 0; j < modes.Count; j++)
                    {
                        if (modes[j].resourceName == resource.resourceName)
                        {
                            //Remove resources managed by this mod
                            preserveResource = false;
                            break;
                        }
                    }
                    if (!preserveResource)
                    {
                        GTIDebug.Log("Removing Resource: " + resource.resourceName, iDebugLevel.DebugInfo);
                        //if (currentPart.Resources.Remove(resource)) GTIDebug.Log("Resource removed: " + GetResourceID(resource.resourceName), iDebugLevel.DebugInfo);
                        resourcesDeleteList.Add(resource);
                    }
                }
                foreach (var resource in resourcesDeleteList)
                {
                    if (currentPart.Resources.Remove(resource)) GTIDebug.Log("Resource removed: " + GetResourceID(resource.resourceName), iDebugLevel.DebugInfo);
                }
                resourcesDeleteList = null;

                //Add the resources
                GTIDebug.Log("MultiModeIntake: Add Resource\n" + IntakeResource.ToString(), iDebugLevel.DebugInfo);
                currentPart.AddResource(IntakeResource);
                //IntakeResource.ClearNodes();
                //IntakeResource.ClearValues();

                GTIDebug.Log("Listing resources defined in part", iDebugLevel.DebugInfo);
                if(DebugLevel == iDebugLevel.DebugInfo)
                {
                    for (int i = 0; i < currentPart.Resources.Count; i++)
                    {
                        GTIDebug.Log("currentPart.Resources[" + i + "].resourceName: " + currentPart.Resources[i].resourceName + " \t" + currentPart.Resources[i].maxAmount, iDebugLevel.DebugInfo);
                    }
                }
            }
            #endregion

            try
            { if (HighLogic.LoadedSceneIsFlight && !silentUpdate) { KSP.UI.Screens.ResourceDisplay.Instance.Refresh(); } }
            catch { GTIDebug.LogError("Update resource panel failed."); }
        }
        
        protected override void writeScreenMessage()
        {
            writeScreenMessage(
                Message: "Intake mode: " + modes[selectedMode].Name,
                messagePosition: messagePosition,
                duration: 3f
                );
        }

        protected override void ModuleAnimationGroupEvent_DisableModules() { throw new NotImplementedException(); }

        public List<GTI_MultiModeIntake> GetCounterPartModules(Part thispart)
        {
            List<Part> CounterParts = thispart.symmetryCounterparts;
            List<GTI_MultiModeIntake> modules = new List<GTI_MultiModeIntake>(CounterParts.Count);

            foreach (Part part in CounterParts)
            {
                modules.Add(part.FindModuleImplementing<GTI_MultiModeIntake>());
            }

            return modules;
        }

        [KSPEvent(name = "IntakeActivate", guiName = "Open Intake (GTI)", active = true, externalToEVAOnly = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActive = true, guiActiveEditor = true)]
        public void IntakeActivate()
        {
            selectedModeStatus = true;

            this.Events["IntakeActivate"].active = false;
            this.Events["IntakeDeactivate"].active = true;

            InvokeOnUpdateMultiMode(true);

            ////Sync up all counterparts modules
            if (affectSymCounterpartsInFlight)
            {
                List<GTI_MultiModeIntake> CounterModules = GetCounterPartModules(this.part);
                foreach (GTI_MultiModeIntake module in CounterModules)
                {
                    module.selectedModeStatus = true;

                    module.Events["IntakeActivate"].active = false;
                    module.Events["IntakeDeactivate"].active = true;

                    module.InvokeOnUpdateMultiMode(true);
                }
            }
        }
        [KSPEvent(name = "IntakeDeactivate", guiName = "Close Intake (GTI)", active = true, externalToEVAOnly = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActive = true, guiActiveEditor = true)]
        public void IntakeDeactivate()
        {
            selectedModeStatus = false;

            this.Events["IntakeActivate"].active = true;
            this.Events["IntakeDeactivate"].active = false;

            InvokeOnUpdateMultiMode(true);

            ////Sync up all counterparts modules
            if (affectSymCounterpartsInFlight)
            {
                List<GTI_MultiModeIntake> CounterModules = GetCounterPartModules(this.part);
                foreach (GTI_MultiModeIntake module in CounterModules)
                {
                    module.selectedModeStatus = false;

                    module.Events["IntakeActivate"].active = true;
                    module.Events["IntakeDeactivate"].active = false;

                    module.InvokeOnUpdateMultiMode(true);
                }
            }
        }

        [KSPAction("Toggle Intake")]
        public void ToggleAction(KSPActionParam param)
        {
            if (selectedModeStatus)
            {
                IntakeDeactivate();
            }
            else
            {
                IntakeActivate();
            }
        }

        [KSPEvent(name = "GetStatus", guiName = "GetStatus (GTI)", active = false, externalToEVAOnly = false, guiActiveUnfocused = false, unfocusedRange = 5f, guiActive = true, guiActiveEditor = true)]
        public void GetStatus()
        {
            GTIDebug.Log("\nGetStatus() of all ModuleResourceIntakes", iDebugLevel.DebugInfo);
            foreach (ModuleResourceIntake I in ModuleIntakes)
            {
                GTIDebug.Log("\nresourceName " + I.resourceName, iDebugLevel.DebugInfo);
                GTIDebug.Log("status " + I.status, iDebugLevel.DebugInfo);
                GTIDebug.Log("intakeEnabled " + I.intakeEnabled, iDebugLevel.DebugInfo);
                GTIDebug.Log("isActiveAndEnabled " + I.isActiveAndEnabled, iDebugLevel.DebugInfo);
                GTIDebug.Log("isEnabled " + I.isEnabled, iDebugLevel.DebugInfo);
                GTIDebug.Log("enabled " + I.enabled, iDebugLevel.DebugInfo);
                GTIDebug.Log("airFlow " + I.airFlow, iDebugLevel.DebugInfo);
                GTIDebug.Log("airSpeedGui " + I.airSpeedGui, iDebugLevel.DebugInfo);
                GTIDebug.Log("area " + I.area, iDebugLevel.DebugInfo);
                GTIDebug.Log("checkForOxygen " + I.checkForOxygen, iDebugLevel.DebugInfo);
                GTIDebug.Log("kPaThreshold " + I.kPaThreshold, iDebugLevel.DebugInfo);
                GTIDebug.Log("moduleIsEnabled " + I.moduleIsEnabled, iDebugLevel.DebugInfo);
            }
        }
    }
}
/* NOTES
.Events
GUIName: Close Intake
id: -3298156
name: Deactivate
active: True
assigned: False
category: 
externalToEVAOnly: True
guiActive: True
guiActiveEditor: True
guiActiveUncommand: False
guiActiveUnfocused: False
guiIcon: Close Intake
unfocusedRange: 2

[LOG 14:39:45.346]
[GTI] 
.Events
GUIName: Open Intake
id: -1591330541
name: Activate
active: False
assigned: False
category: 
externalToEVAOnly: True
guiActive: True
guiActiveEditor: True
guiActiveUncommand: False
guiActiveUnfocused: False
guiIcon: Open Intake
unfocusedRange: 2

[LOG 14:39:45.346]
[GTI] 
.Events
GUIName: Disable Staging
id: 1164541479
name: ToggleStaging
active: True
assigned: False
category: 
externalToEVAOnly: True
guiActive: False
guiActiveEditor: False
guiActiveUncommand: False
guiActiveUnfocused: False
guiIcon: Disable Staging
unfocusedRange: 2
*/