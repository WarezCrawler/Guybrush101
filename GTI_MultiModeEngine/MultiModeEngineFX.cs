using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    partial class GTI_MultiModeEngineFX : PartModule
    {
        [KSPField]
        public string engineID = string.Empty;
        [KSPField]
        public string GUIengineID = string.Empty;

        //[KSPField]
        //public string addedCost = string.Empty;


        #region Empty value indicators (boolean)
        //private bool minReqTechEmpty = true, maxReqTechEmpty = true, engineAvailableEmpty = true, 
        private bool GUIengineIDEmpty = true;
        #endregion


        #region Arrays and Lists
        //For the engines modules
        private List<ModuleEngines> ModuleEngines;

        private ModuleEngines currentModuleEngine;
        private bool currentEngineState;

        private List<CustomTypes.EngineSwitchList> engineList = new List<CustomTypes.EngineSwitchList>();
        #endregion


        #region Other class level declarations
        private bool _settingsInitialized = false;
        //[KSPField]
        public string _startDelay = "2";
        [KSPField]
        public string debugMode = "false";
        #endregion

        public override void OnStart(PartModule.StartState state)
        {
            initializeSettings();
            initializeGUI();
            updatePropulsion();

            //Show Debug GUI?
            if (bool.Parse(debugMode)) { Events["DEBUG_ENGINESSWITCH"].guiActive = true; Events["DEBUG_ENGINESSWITCH"].guiActiveEditor = true; Events["DEBUG_ENGINESSWITCH"].active = true; Debug.Log("GTI_MultiModeEngine debugMode activated"); }
            else { Events["DEBUG_ENGINESSWITCH"].guiActive = false; Events["DEBUG_ENGINESSWITCH"].guiActiveEditor = false; Events["DEBUG_ENGINESSWITCH"].active = false; }
        }

        /// <summary>
        /// Initialize settings for GTI_MultiModeEngine
        /// </summary>
        private void initializeSettings()
        {
            if (!_settingsInitialized)
            {
                Utilities Util = new Utilities();
                string[] arrEngineID, arrGUIengineID;   //, arrMinReqTech, arrMaxReqTech;  //, arrEngineAvailable;

                #region Split into Arrays
                arrEngineID = engineID.Trim().Split(';');
                GUIengineIDEmpty = Util.ArraySplitEvaluate(GUIengineID, out arrGUIengineID, ';');
                #endregion

                #region Identify ModuleEngines in Scope
                //Find modules which is to be manipulated
                ModuleEngines = part.FindModulesImplementing<ModuleEngines>();
                var toBeRemoved = new List<ModuleEngines>();                      //for removal of irrelevant engine modules

                //Remove the inteactions buttons of the engines, so that it is controlled by this mod instead
                foreach (var moduleEngine in ModuleEngines)
                {
                    //Deactivate stock engine actions
                    moduleEngine.Actions["OnAction"].active = false;
                    moduleEngine.Actions["ShutdownAction"].active = false;
                    moduleEngine.Actions["ActivateAction"].active = false;

                    //New 21/11-2016
                    //moduleEngine.manuallyOverridden = false;
                    //moduleEngine.isEnabled = false;
                }

                //Populated engineList with the settings
                for (int i = 0; i < arrEngineID.Length; i++)
                {
                    engineList.Add(new CustomTypes.EngineSwitchList()
                    {
                        engineID = arrEngineID[i],
                        GUIengineID = GUIengineIDEmpty ? arrEngineID[i] : arrGUIengineID[i],
                    });
                }

                //If there is an engine, and none is currently selected, then set the active one to be the first one
                if (ModuleEngines.Count > 0) { selPropFromChooseOption(); }
               
                //find the current engine and store it in "currentModuleEngine"
                foreach (var moduleEngine in ModuleEngines)
                {
                    if (moduleEngine.engineID == ChooseOption)
                    {
                        currentModuleEngine = moduleEngine;
                    }
                }
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
            //Debug.Log("GTI_MultiModeEngine: updatePropulsion() --> ChooseOption = " + ChooseOption);

            currentEngineState = currentModuleEngine.getIgnitionState;

            FindSelectedPropulsion();
            writeScreenMessage();

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
                    //Debug.Log("GTI_MultiModeEngine: Set currentModuleEngine " + moduleEngine.engineID);
                    currentModuleEngine = moduleEngine;
                    //Reactivate engine if it was active
                    if (currentEngineState)
                    {
                        //Debug.Log("GTI_MultiModeEngine: Activate() " + moduleEngine.engineID);
                        moduleEngine.Activate();
                    }
                    moduleEngine.manuallyOverridden = false;
                    moduleEngine.isEnabled = true;
                }
                else
                {
                    //Debug.Log("GTI_MultiModeEngine: Shutdown() " + moduleEngine.engineID);
                    moduleEngine.Shutdown();
                    moduleEngine.manuallyOverridden = true;
                    moduleEngine.isEnabled = false;
                }
            }

        } //END OF updatePropulsion()


        /// <summary>
        /// selPropFromChooseOption set selectedPropulsion from ChooseOption.
        /// If ChooseOption is empty, then the first engine in engineList is returned.
        /// Dependent on: engineList, ChooseOption, selectedPropulsion
        /// </summary>
        private void selPropFromChooseOption()
        {
            if (ChooseOption == string.Empty)
            {
                ChooseOption = engineList[0].engineID;
                selectedPropulsion = 0;
            }
            else
            {
                for (int i = 0; i < engineList.Count; i++)
                {
                    if (ChooseOption == engineList[i].engineID)
                    {
                        selectedPropulsion = i;
                        return;
                    }
                    
                }
            }
        }
    }
}