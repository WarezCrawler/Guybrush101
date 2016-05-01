using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    partial class GTI_EngineClassSwitch_2 : PartModule
    {
        [KSPField]
        public string engineID = string.Empty;
        [KSPField]
        public string GUIengineID = string.Empty;
        [KSPField]
        public string minReqTech = string.Empty;
        [KSPField]
        public string maxReqTech = string.Empty;

        #region Empty value indicators (boolean)
        private bool minReqTechEmpty = true, maxReqTechEmpty = true, GUIengineIDEmpty = true;
        #endregion


        #region Arrays and Lists
        private string[] arrEngineID, arrGUIengineID, arrMinReqTech, arrMaxReqTech; 

        //For the engines modules
        private List<ModuleEngines> ModuleEngines;

        private ModuleEngines currentModuleEngine;
        private bool currentEngineState;
        #endregion


        #region Other class level declarations
        private bool _settingsInitialized = false;
        //[KSPField]
        //public string _startDelay = "1";
        [KSPField]
        public string debugMode = "false";
        #endregion

        public override void OnStart(PartModule.StartState state)
        {
            initializeSettings();
            initializeGUI();
            updatePropulsion();

            //Show Debug GUI?
            if (bool.Parse(debugMode)) { Events["DEBUG_ENGINESSWITCH"].guiActive = true; Events["DEBUG_ENGINESSWITCH"].guiActiveEditor = true; Events["DEBUG_ENGINESSWITCH"].active = true; Debug.Log("Engine Switch 2 debugMode activated"); }
            else { Events["DEBUG_ENGINESSWITCH"].guiActive = false; Events["DEBUG_ENGINESSWITCH"].guiActiveEditor = false; Events["DEBUG_ENGINESSWITCH"].active = false; }
        }
        /// <summary>
        /// Initialize settings for GTI_EngineClassSwitch_2
        /// </summary>
        private void initializeSettings()
        {
            if (!_settingsInitialized)
            {
                Utilities Util = new Utilities();

                arrEngineID = engineID.Trim().Split(';');
                GUIengineIDEmpty = Util.ArraySplitEvaluate(GUIengineID, out arrGUIengineID, ';');
                minReqTechEmpty = Util.ArraySplitEvaluate(minReqTech, out arrMinReqTech, ';');
                maxReqTechEmpty = Util.ArraySplitEvaluate(maxReqTech, out arrMaxReqTech, ';');


                #region Identify ModuleEngines in Scope
                //Find modules which is to be manipulated
                ModuleEngines = part.FindModulesImplementing<ModuleEngines>();
                var toBeRemoved = new List<ModuleEngines>();                      //for removal of irrelevant engine modules
                /*
                foreach (var moduleEngine in ModuleEngines)
                {
                    if (moduleEngine.engineID == engineID)              // || string.IsNullOrEmpty(engineID) || engineID.Trim().Length == 0)
                    {
                        //Debug.Log(moduleEngine.name + " added to list of engines using EngineSwitch");
                    }
                    else
                    {
                        toBeRemoved.Add(moduleEngine);
                    }
                }
                foreach (var remove in toBeRemoved)
                {
                    //disabled until a valid criteria is implemented. Probably tech requirement
                    //ModuleEngines.Remove(remove);
                }
                */

                //If there is an engine, and none is currently selected, then set the active one to be the first one
                if (ModuleEngines.Count > 0)
                {
                    if (ChooseOption == string.Empty)
                    {
                        ChooseOption = arrEngineID[0];
                    }
                }
                //find the current engine and store it in "currentModuleEngine"
                foreach (var moduleEngine in ModuleEngines)
                {
                    if (moduleEngine.engineID == ChooseOption)
                    {
                        currentModuleEngine = moduleEngine;
                    }
                }


                //Now ModulesEngines should have exactly the engine modules in scope
                #endregion

                _settingsInitialized = true;
            }
        }

        /// <summary>
        /// Update propulsion
        /// </summary>
        private void updatePropulsion()
        {
            initializeSettings();
            Debug.Log("updatePropulsion() --> ChooseOption = " + ChooseOption);

            currentEngineState = currentModuleEngine.getIgnitionState;

            foreach (var moduleEngine in ModuleEngines)
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
                    currentModuleEngine = moduleEngine;
                    //Reactivate engine if it was active
                    if (currentEngineState) { moduleEngine.Activate(); }

                    //Activate Gui Events
                    moduleEngine.Events["Activate"].guiActive = true;
                    moduleEngine.Events["Shutdown"].guiActive = true;

                    //Activate Gui Fields
                    moduleEngine.Fields["fuelFlowGui"].guiActive = true;
                    moduleEngine.Fields["finalThrust"].guiActive = true;
                    moduleEngine.Fields["realIsp"].guiActive = true;
                    moduleEngine.Fields["status"].guiActive = true;
                    moduleEngine.Fields["thrustPercentage"].guiActive = true;
                }
                else
                {
                    moduleEngine.Shutdown();
                    //Deactivate Gui Events
                    moduleEngine.Events["Activate"].guiActive = false;
                    moduleEngine.Events["Shutdown"].guiActive = false;
                    //Deactivate Gui Fields
                    moduleEngine.Fields["fuelFlowGui"].guiActive = false;
                    moduleEngine.Fields["finalThrust"].guiActive = false;
                    moduleEngine.Fields["realIsp"].guiActive = false;
                    moduleEngine.Fields["status"].guiActive = false;
                    moduleEngine.Fields["thrustPercentage"].guiActive = false;
                }
            }
        } //END OF updatePropulsion()
        
    }
}
