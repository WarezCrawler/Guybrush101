using static GTI.GTIConfig;
using static GTI.Utilities;
using System.Collections.Generic;
using System;
using System.Text;

//using System;

/*
This module targets "ModuleEnginesFX" modules for engine switching
*/

namespace GTI
{
    //public class EngineMultiMode : MultiMode
    //{
    //    //public int moduleIndex;
    //    //public string ID;
    //    //public string Name;

    //    //public string engineID { get; set; } = string.Empty;        //The propellants
    //    //public string GUIengineID { get; set; } = string.Empty;

    //}

    public class GTI_MultiModeEngineFX : GTI_MultiMode<MultiMode>        //, IPartCostModifier
    {
        //private string _thismoduleName = "GTI_MultiModeEngineFX";

        [KSPField]
        public string engineID = string.Empty;
        [KSPField]
        public string GUIengineID = string.Empty;

        #region Empty value indicators (boolean)
        //private bool minReqTechEmpty = true, maxReqTechEmpty = true, engineAvailableEmpty = true, 
        private bool GUIengineIDEmpty = true;
        #endregion


        #region Arrays and Lists
        //For the engines modules
        //new public List<EngineMultiMode> mode { get; protected set; }     //Override the list type
        protected List<ModuleEnginesFX> ModuleEngines;

        private ModuleEnginesFX currentModuleEngine;
        private bool currentEngineState;

        //public new List<EngineMultiMode> mode;
        //private int i = 0;
        #endregion

        /// <summary>
        /// Initialize settings for GTI_MultiModeEngine
        /// </summary>
        protected override void initializeSettings()
        {
            if (!_settingsInitialized)
            {
                GTIDebug.Log("GTI_MultiModeEngineFX() --> initializeSettings()", iDebugLevel.DebugInfo);
                //Utilities Util = new Utilities();
                //string[] arrEngineID;    //, arrGUIengineID;

                #region Split into Arrays
                string[] arrEngineID = engineID.Trim().Split(';');
                GUIengineIDEmpty = ArraySplitEvaluate(GUIengineID, out string[] arrGUIengineID, ';');
                #endregion

                #region Identify ModuleEngines in Scope
                //mode = new List<EngineMultiMode>(arrEngineID.Length);
                modes = new List<MultiMode>(arrEngineID.Length);
                GTIDebug.Log("Create list of modes", iDebugLevel.DebugInfo);
                for (int i = 0; i < arrEngineID.Length; i++)
                {
                    modes.Add(new MultiMode()
                    {
                        ID = arrEngineID[i],
                        Name = GUIengineIDEmpty ? arrEngineID[i] : arrGUIengineID[i]
                    });
                }

                //If there is an engine, and none is currently selected, then set the active one to be the first one
                GTIDebug.Log("Find module engines from part", iDebugLevel.DebugInfo);
                ModuleEngines = part.FindModulesImplementing<ModuleEnginesFX>();

                //GTIDebug.Log("Find selected mode from ChooseOption", iDebugLevel.DebugInfo);
                //if (ModuleEngines.Count > 0) { selModeFromChooseOption(); }
                //Find modules which is to be manipulated
                
                //Remove the inteactions buttons of the engines, so that it is controlled by this mod instead
                //foreach (var moduleEngine in ModuleEngines)
                for (int i = 0; i < ModuleEngines.Count; i++)
                {
                    //Deactivate stock engine actions
                    ModuleEngines[i].Actions["OnAction"].active = false;
                    ModuleEngines[i].Actions["ShutdownAction"].active = false;
                    ModuleEngines[i].Actions["ActivateAction"].active = false;

                    GTIDebug.Log(ModuleEngines[i].engineID + " - Collect module engines index's", this.GetType().Name, iDebugLevel.DebugInfo);
                    //foreach (MultiMode m in modes)
                    for (int j = 0; j < modes.Count; j++)
                    {
                        //Update index's of the engine modules
                        if (ModuleEngines[i].engineID == modes[j].ID)
                        {
                            GTIDebug.Log("Engine index found: " + i, this.GetType().Name, iDebugLevel.DebugInfo);
                            modes[j].moduleIndex = i;
                        }
                    }

                    //Get currently activated engine module
                    //GTIDebug.Log("Get currently activated engine module", iDebugLevel.DebugInfo);
                    //if (ModuleEngines[i].engineID == ChooseOption) currentModuleEngine = ModuleEngines[i];
                }
                #endregion
            }
        }


        /// <summary>
        /// Update propulsion
        /// </summary>
        public override void updateMultiMode(bool silentUpdate = false)
        {
            //initializeSettings();
            GTIDebug.Log("GTI_MultiModeEngine: updatePropulsion() --> ChooseOption = " + ChooseOption, iDebugLevel.High);

            if (currentModuleEngine != null)
            {
                currentEngineState = currentModuleEngine.getIgnitionState;
            } else GTIDebug.Log("updateMultiMode() --> currentModuleEngine is null", iDebugLevel.Low);


            //FindSelectedMode();       //irrelevant when inheritting from the base class
            writeScreenMessage();

            if (ModuleEngines == null)
                GTIDebug.Log("updateMultiMode() --> ModuleEngines is null", iDebugLevel.Low);

            foreach (ModuleEnginesFX moduleEngine in ModuleEngines)
            {
                #region NOTES
                /* Stock GUI Elements
                fuelFlowGui
                finalThrust
                realIsp
                status
                thrustPercentage
                */
                #endregion

                if (moduleEngine.engineID == ChooseOption)
                {
                    GTIDebug.Log("GTI_MultiModeEngine: Set currentModuleEngine " + moduleEngine.engineID, iDebugLevel.High);
                    currentModuleEngine = moduleEngine;
                    //Reactivate engine if it was active
                    if (currentEngineState)
                    {
                        GTIDebug.Log("GTI_MultiModeEngine: Activate() " + moduleEngine.engineID, iDebugLevel.High);
                        moduleEngine.Activate();
                    }
                    moduleEngine.manuallyOverridden = false;
                    moduleEngine.isEnabled = true;
                }
                else
                {
                    GTIDebug.Log("GTI_MultiModeEngine: Shutdown() " + moduleEngine.engineID, iDebugLevel.High);
                    moduleEngine.Shutdown();
                    moduleEngine.manuallyOverridden = true;
                    moduleEngine.isEnabled = false;
                }
            }


        } //END OF updatePropulsion()

        protected override void initializeGUI()
        {
            GTIDebug.Log("GTI_MultiModeEngine() --> override initializeGUI()", iDebugLevel.High);
            base.initializeGUI();
            //Get currently activated engine module
            GTIDebug.Log("override initializeGUI() --> Get currently activated engine module", iDebugLevel.High);
            for (int i = 0; i < ModuleEngines.Count; i++)
            {
                if (ModuleEngines[i].engineID == ChooseOption) currentModuleEngine = ModuleEngines[i];
            }
        }

        protected override void writeScreenMessage()
        {
            writeScreenMessage(
                Message: "Changing Propulsion to: " + modes[selectedMode].ID,
                messagePosition: ScreenMessageStyle.UPPER_CENTER
                );
        }

        [KSPAction("Activate Engine")]
        public void ActionActivate(KSPActionParam param)
        {
            if (!currentModuleEngine.getIgnitionState) { currentModuleEngine.Activate(); }

            currentEngineState = currentModuleEngine.getIgnitionState;
            //Debug.Log("Action currentModuleEngine.Activate(): " + ChooseOption + " new state is: " + currentEngineState);
        }
        [KSPAction("Shutdown Engine")]
        public void ActionShutdown(KSPActionParam param)
        {
            if (currentModuleEngine.getIgnitionState) { currentModuleEngine.Shutdown(); }

            currentEngineState = currentModuleEngine.getIgnitionState;
            //Debug.Log("Action currentModuleEngine.Shutdown(): " + ChooseOption + " new state is: " + currentEngineState);
        }
        public void ActionToggle(KSPActionParam param)
        {

            if (currentModuleEngine.getIgnitionState)
            {
                currentModuleEngine.Shutdown();
            }
            else
            {
                currentModuleEngine.Activate();
            }

            currentEngineState = currentModuleEngine.getIgnitionState;
            //Debug.Log("Action currentModuleEngine.Shutdown(): " + ChooseOption + " new state is: " + currentEngineState);
        }

        protected override void ModuleAnimationGroupEvent_DisableModules()
        {
            throw new NotImplementedException();
        }

        #region VAB Information
        public override string GetInfo()
        {
            StringBuilder Info = new StringBuilder();
            GTIDebug.Log("GTI_MultiModeEngineFX GetInfo");
            try
            {
                if (!_settingsInitialized)
                {
                    initializeSettings();
                    //ModuleEngines = part.FindModulesImplementing<ModuleEnginesFX>();
                }


                //Info.AppendLine("GTI MultiMode Engine FX");
                Info.AppendLine("<color=yellow>Engine Modes Available:</color>");
                Info.AppendLine(GUIengineID);
                Info.AppendLine("\nIn Flight switching is <color=yellow>" + (availableInFlight ? "available" : "not available") + "</color>");
                //foreach (ModuleEnginesFX e in ModuleEngines)
                //foreach (MultiMode m in modes)
                //{

                //    //ModuleEngines[m.moduleIndex].GetMaxThrust();

                //    Info.AppendLine("Max Trust " + ModuleEngines[m.moduleIndex].GetMaxThrust());
                //}


                //str.AppendFormat("Maximal force: {0:0.0}iN\n", maxGeneratorForce);
                //str.AppendFormat("Maximal charge time: {0:0.0}s\n\n", maxChargeTime);
                //str.AppendFormat("Requires\n");
                //str.AppendFormat("- Electric charge: {0:0.00}/s\n\n", requiredElectricalCharge);
                //str.Append("Navigational computer\n");
                //str.Append("- Required force\n");
                //str.Append("- Success probability\n");


                //return "GTI MultiMode Engine FX";
                return Info.ToString();
            }
            catch (Exception e)
            {
                GTIDebug.LogError("GTI_MultiModeEngineFX GetInfo Error " + e.Message);
                throw;
            }
        }
        #endregion
        //partial class GTI_MultiModeEngineFX : GTI_MultiMode<MultiMode>     //PartModule
        //{
        //    #region VAB Information
        //    public override string GetInfo()
        //    {
        //        try
        //        {
        //            //we need to run the InitializeSettings here, because the OnStart does not run before this.
        //            //initializeSettings();

        //            //string strOutInfo = string.Empty;
        //            //System.Text.StringBuilder strOutInfo = new System.Text.StringBuilder();
        //            //string[] _propellants, _propratios;

        //            //strOutInfo.AppendLine("Propellants available");
        //            //foreach (CustomTypes.PropellantList item in propList)
        //            //{
        //            //    strOutInfo.AppendLine(item.Propellants.Replace(",",", "));
        //            //}
        //            //strOutInfo.AppendLine(propellantNames.Replace(";", "; "));
        //            return "GTI Multi Mode Engine FX";
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.LogError("GTI_MultiModeEngineFX GetInfo Error " + e.Message);
        //            throw;
        //        }
        //    }
        //    #endregion
        //}
    }
}