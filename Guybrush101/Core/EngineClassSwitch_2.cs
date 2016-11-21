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
        [KSPField(isPersistant = true)]
        public string engineAvailable = string.Empty;
        [KSPField]
        public bool applyTechReqToSandbox = false;
        [KSPField]
        public string minReqTech = string.Empty;
        [KSPField]
        public string maxReqTech = string.Empty;

        #region Empty value indicators (boolean)
        private bool minReqTechEmpty = true, maxReqTechEmpty = true, engineAvailableEmpty = true, GUIengineIDEmpty = true;
        #endregion


        #region Arrays and Lists
        //private string[] arrEngineID, arrGUIengineID, arrMinReqTech, arrMaxReqTech; 

        //For the engines modules
        private List<ModuleEngines> ModuleEngines;

        private ModuleEngines currentModuleEngine;
        private bool currentEngineState;

        private List<CustomTypes.EngineSwitchList> engineList = new List<CustomTypes.EngineSwitchList>();
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
                string[] arrEngineID, arrGUIengineID, arrMinReqTech, arrMaxReqTech, arrEngineAvailable;

                #region Split into Arrays
                arrEngineID =       engineID.Trim().Split(';');
                GUIengineIDEmpty =  Util.ArraySplitEvaluate(GUIengineID,    out arrGUIengineID  , ';');
                minReqTechEmpty =   Util.ArraySplitEvaluate(minReqTech,     out arrMinReqTech   , ';');
                maxReqTechEmpty =   Util.ArraySplitEvaluate(maxReqTech,     out arrMaxReqTech   , ';');

                engineAvailableEmpty = Util.ArraySplitEvaluate(engineAvailable, out arrEngineAvailable, ';');

                //CONTROL: Evaluate if the engineAvailable is of same size as the engineID one. If not, we reevaluate, since the might be new engines in the part. This is not perfect, but the function does not fail.
                //Added a check for editor, where it will reevaluate, so that the parts does not need to be reloaded
                if ((arrEngineAvailable.Length != arrEngineID.Length) || HighLogic.LoadedSceneIsEditor)
                {
                    engineAvailable = string.Empty; engineAvailableEmpty = Util.ArraySplitEvaluate(engineAvailable, out arrEngineAvailable, ';');
                    Debug.LogError("GTI_EngineClassSwitch_2 -> arrEngineAvailable.Length != arrEngineID.Length \narrEngineID.Length: " + arrEngineID.Length + "\narrEngineAvailable.Length: " + arrEngineAvailable.Length);
                }
                Debug.Log("engineAvailable: \n'" + engineAvailable + "' \nafter evaluation against engineID");
                //engineAvailable
                #endregion

                #region Identify ModuleEngines in Scope
                //Find modules which is to be manipulated
                ModuleEngines = part.FindModulesImplementing<ModuleEngines>();
                var toBeRemoved = new List<ModuleEngines>();                      //for removal of irrelevant engine modules

                foreach (var moduleEngine in ModuleEngines)
                {
                    //Deactivate stock engine actions
                    moduleEngine.Actions["OnAction"].active = false;
                    moduleEngine.Actions["ShutdownAction"].active = false;
                    moduleEngine.Actions["ActivateAction"].active = false;

                    //New 21/11-2016
                    moduleEngine.manuallyOverridden = false;
                    moduleEngine.isEnabled = false;

                    //if (moduleEngine.engineID == engineID)              // || string.IsNullOrEmpty(engineID) || engineID.Trim().Length == 0)
                    //{
                    //    //Debug.Log(moduleEngine.name + " added to list of engines using EngineSwitch");
                    //}
                    //else
                    //{
                    //    toBeRemoved.Add(moduleEngine);
                    //}
                }
                //foreach (var remove in toBeRemoved)
                //{
                //    //disabled until a valid criteria is implemented. Probably tech requirement
                //    //ModuleEngines.Remove(remove);
                //}

                for (int i = 0; i < arrEngineID.Length; i++)
                {
                    engineList.Add(new CustomTypes.EngineSwitchList()
                    {
                        engineID = arrEngineID[i],
                        GUIengineID = GUIengineIDEmpty ? arrEngineID[i] : arrGUIengineID[i],
                        minReqTech = minReqTechEmpty ? "start" : arrMinReqTech[i],
                        maxReqTech = maxReqTechEmpty ? string.Empty : arrMaxReqTech[i],
                        engineAvailable = engineAvailableEmpty ? true : bool.Parse(arrEngineAvailable[i])
                    });
                    //engineList[i].minReqTech = minReqTechEmpty ? "start" : arrMinReqTech[i];
                    //engineList[i].maxReqTech = maxReqTechEmpty ? string.Empty : arrMaxReqTech[i];
                    //engineList[i].GUIengineID = GUIengineIDEmpty ? string.Empty : arrGUIengineID[i];
                }
                
                foreach (var item in engineList)
                {
                    Debug.Log(
                        "\n" + item.engineID +
                        "\n" + item.GUIengineID +
                        "\n" + item.minReqTech +
                        "\n" + item.maxReqTech +
                        "\n");
                }


                /*HERE GOES ANY CHECKS AND REMOVALS BASED ON TECHLEVEL*/
                //CheckTech(ref arrEngineAvailable, Util);
                
                //ResearchAndDevelopment.GetTechnologyState(propList[i].requiredTech) == RDTech.State.Unavailable
                //for (int i = engineList.Count - 1; i >= 0; i--)
                //{
                //    Debug.Log("engineList[i].engineAvailable: " + engineList[i].engineAvailable);
                //    if (!engineList[i].engineAvailable) { engineList.RemoveAt(i); }
                //}
                
                //If there is an engine, and none is currently selected, then set the active one to be the first one
                if (ModuleEngines.Count > 0)
                {
                    selPropFromChooseOption();
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
                    currentModuleEngine = moduleEngine;
                    //Reactivate engine if it was active
                    if (currentEngineState)
                    {
                        moduleEngine.Activate();
                    }
                    moduleEngine.manuallyOverridden = true;
                    moduleEngine.isEnabled = true;
                    //moduleEngine.FXReset();
                    //moduleEngine.ActivatePowerFX();
                    //moduleEngine.ActivateRunningFX();
                    //moduleEngine.InitializeFX();

                    #region Update GUI
                    //Activate Gui Events
                    moduleEngine.Events["Activate"].guiActive = true;
                    moduleEngine.Events["Shutdown"].guiActive = true;

                    //Activate Gui Fields
                    moduleEngine.Fields["fuelFlowGui"].guiActive = true;
                    moduleEngine.Fields["finalThrust"].guiActive = true;
                    moduleEngine.Fields["realIsp"].guiActive = true;
                    moduleEngine.Fields["status"].guiActive = true;
                    moduleEngine.Fields["thrustPercentage"].guiActive = true;
                    #endregion
                }
                else
                {
                    moduleEngine.Shutdown();
                    moduleEngine.manuallyOverridden = false;
                    moduleEngine.isEnabled = false;
                    //moduleEngine.FXReset();
                    //moduleEngine.DeactivatePowerFX();
                    //moduleEngine.DeactivateRunningFX();
                    //moduleEngine.DeactivateLoopingFX();
                    #region Update GUI
                    //Deactivate Gui Events
                    moduleEngine.Events["Activate"].guiActive = false;
                    moduleEngine.Events["Shutdown"].guiActive = false;
                    //Deactivate Gui Fields
                    moduleEngine.Fields["fuelFlowGui"].guiActive = false;
                    moduleEngine.Fields["finalThrust"].guiActive = false;
                    moduleEngine.Fields["realIsp"].guiActive = false;
                    moduleEngine.Fields["status"].guiActive = false;
                    moduleEngine.Fields["thrustPercentage"].guiActive = false;
                    #endregion
                }
            }
            //Find the ID (int) of "ChooseOption"
            //for (int i = 0; i < arrEngineID.Length; i++)
            //{
            //    if (arrEngineID[i] == ChooseOption) { selectedPropulsion = i; break; } else { selectedPropulsion = 0; }
            //}

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
                    selectedPropulsion = (ChooseOption == engineList[i].engineID) ? i : 0;
                }
            }
        }

        private void CheckTech(ref string[] arrEngineAvailable, Utilities Util)
        {
            if (engineAvailableEmpty)
            {
                Debug.Log("Check ResearchAndDevelopment.GetTechnologyState for engineList");
                foreach (var item in engineList)
                {
                    //Debug.Log(
                    //"\n(ResearchAndDevelopment.GetTechnologyState(item.minReqTech) == RDTech.State.Unavailable) && !minReqTechEmpty" +
                    //"\nResearchAndDevelopment.GetTechnologyState(item.minReqTech): " + ResearchAndDevelopment.GetTechnologyState(item.minReqTech) +
                    //"\n!minReqTechEmpty: " + !minReqTechEmpty
                    //);
                    //Debug.Log(
                    //"\n(ResearchAndDevelopment.GetTechnologyState(item.maxReqTech) == RDTech.State.Available) && !maxReqTechEmpty && (HighLogic.CurrentGame.Mode != Game.Modes.SANDBOX || applyTechReqToSandbox)" +
                    //"\nResearchAndDevelopment.GetTechnologyState(item.maxReqTech): " + ResearchAndDevelopment.GetTechnologyState(item.maxReqTech) +
                    //"\n!maxReqTechEmpty: " + !maxReqTechEmpty +
                    //"\nHighLogic.CurrentGame.Mode: " + HighLogic.CurrentGame.Mode +
                    //"\napplyTechReqToSandbox: " + applyTechReqToSandbox
                    //);
                    if ((ResearchAndDevelopment.GetTechnologyState(item.minReqTech) == RDTech.State.Unavailable) && !minReqTechEmpty)
                    {
                        Debug.Log("(ResearchAndDevelopment.GetTechnologyState(item.minReqTech) == RDTech.State.Unavailable) && !minReqTechEmpty\ntrue");
                        item.engineAvailable = false;
                    }
                    else if ((ResearchAndDevelopment.GetTechnologyState(item.maxReqTech) == RDTech.State.Available) && (!maxReqTechEmpty) && (HighLogic.CurrentGame.Mode != Game.Modes.SANDBOX || applyTechReqToSandbox))
                    {
                        Debug.Log("(ResearchAndDevelopment.GetTechnologyState(item.maxReqTech) == RDTech.State.Available) && !maxReqTechEmpty && (HighLogic.CurrentGame.Mode != Game.Modes.SANDBOX || applyTechReqToSandbox)\ntrue");
                        item.engineAvailable = false;
                    }
                    else
                    {
                        item.engineAvailable = true;
                    }
                    engineAvailable += item.engineAvailable.ToString() + ";";
                }
                engineAvailable = engineAvailable.Substring(0, engineAvailable.Length - 1);   //Remove the last and redundant ";"
                Debug.Log("engineAvailable: \n'" + engineAvailable + "' \nbuild from the engineList");

            }
            else
            {
                //TAKE THE KSPFIELD AND LOAD INTO engineList
                //At this point the engineList has all engineID's and the engineAvailable list is supposed to follow that at this point. That way we can now use it to select what is available, based on it.
                //engineAvailable is persistent, hence loaded from the save file of any active vessel, and recalculated for any new vessels.
                
                //engineAvailableEmpty = Util.ArraySplitEvaluate(engineAvailable, out arrEngineAvailable, ';');
                Debug.Log("engineAvailable: '" + engineAvailable + "' based on the [KSPField]");
            }
        }
        
    }
}