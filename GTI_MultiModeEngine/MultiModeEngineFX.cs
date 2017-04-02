﻿using GTI.Config;
using static GTI.Config.GTIConfig;
using static GTI.GenericFunctions.Utilities;
using System.Collections.Generic;
using System;

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

    partial class GTI_MultiModeEngineFX : GTI_MultiMode<MultiMode>        //, IPartCostModifier
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
                string[] arrEngineID, arrGUIengineID;

                #region Split into Arrays
                arrEngineID = engineID.Trim().Split(';');
                GUIengineIDEmpty = ArraySplitEvaluate(GUIengineID, out arrGUIengineID, ';');
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

                    GTIDebug.Log(ModuleEngines[i].engineID + " - Collect module engines index's", iDebugLevel.DebugInfo);
                    foreach (MultiMode m in modes)
                    {
                        //Update index's of the engine modules
                        if (ModuleEngines[i].engineID == m.ID) m.moduleIndex = i;
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

            currentEngineState = currentModuleEngine.getIgnitionState;

            //FindSelectedMode();       //irrelevant when inheritting from the base class
            writeScreenMessage();

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

        protected override void ModuleAnimationGroupEvent_DisableModules()
        {
            throw new NotImplementedException();
        }

        #region VAB Information
        public override string GetInfo()
        {
            try
            {
                return "GTI MultiMode Engine FX";
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