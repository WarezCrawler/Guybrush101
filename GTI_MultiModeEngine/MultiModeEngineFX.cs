using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;
//using System;

/*
This module targets "ModuleEnginesFX" modules for engine switching
*/

namespace GTI
{
    partial class GTI_MultiModeEngineFX : PartModule        //, IPartCostModifier
    {
        //private string _thismoduleName = "GTI_MultiModeEngineFX";

        [KSPField]
        public string engineID = string.Empty;
        [KSPField]
        public string GUIengineID = string.Empty;

        //[KSPField]
        //public string AddedCost = string.Empty;
        //private bool AddedCostEmpty;

        //[KSPField]
        //public string addedCost = string.Empty;


        #region Empty value indicators (boolean)
        //private bool minReqTechEmpty = true, maxReqTechEmpty = true, engineAvailableEmpty = true, 
        private bool GUIengineIDEmpty = true;
        #endregion


        #region Arrays and Lists
        //For the engines modules
        private List<ModuleEnginesFX> ModuleEngines;

        private ModuleEnginesFX currentModuleEngine;
        private bool currentEngineState;

        private List<CustomTypes.EngineSwitchList> engineList = new List<CustomTypes.EngineSwitchList>();

        //[KSPField(isPersistant = false)]
        //public FloatCurve ThrottleISPCurve = new FloatCurve();
        //[KSPField]
        //public bool useThrottleISPCurve = false;
        //private FloatCurve ThrottleISPFloatCurve;





        #endregion

        #region Other class level declarations
        private bool _settingsInitialized = false;
        //[KSPField]
        //public string _startDelay = "2";
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

        public override void OnStartFinished(StartState state)
        {
            //File path
            //Debug.Log("GTI_MultiModeEngineFX dll path: " + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            //string assemblyFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string xmlFileName = Path.Combine(assemblyFolder, "AggregatorItems.xml");



            //Debug.Log("ThrottleISPCurve.maxTime\n" + ThrottleISPCurve.maxTime);
            //Debug.Log("ThrottleISPCurve.minTime\n" + ThrottleISPCurve.minTime);
            //Debug.Log("ThrottleISPCurve.Evaluate(0.5f)\n" + ThrottleISPCurve.Evaluate(0.5f));
            //Debug.Log("ThrottleISPCurve.Curve.ToString()\n" + ThrottleISPCurve.Curve.ToString());


            //Debug.Log("CFNTest " + ThrottleISPCurve.Curve.length);
            //foreach (Keyframe ky in ThrottleISPCurve.Curve.keys)
            //{
            //    Debug.Log("CFNTest2 " + ky.time + "|" + ky.value);
            //}
            //Debug.Log("ConfigNode: " + ThrottleISPCurve.ToString());
            /*
            Utilities Util = new Utilities();
            string partName = string.Empty;
            string partTitle = string.Empty;

            //Debug.Log(".GetPartUrl: " + Util.GetPartUrl(this.part));
            Debug.Log(".GetPartConfig: " + Util.GetPartConfig(this.part, out partName, out partTitle));
            Debug.Log(".partName: " + partName);
            Debug.Log(".partTitle: " + partTitle);


            Debug.Log(".GetPartModuleConfigs start");
            foreach (ConfigNode n in Util.GetPartModuleConfigs(this.part, "GTI_MultiModeEngineFX12"))
            {
                Debug.Log(".GetPartModuleConfig: " + n);
            }

            Debug.Log(".GetPartModuleConfigs.ToString(): " + Util.GetPartModuleConfigs(this.part, "GTI_MultiModeEngineFX").ToString());
            //Debug.Log(".GetPartModuleConfigs: " + Util.GetPartModuleConfigs(this.part, "GTI_MultiModeEngineFX"));
            Debug.Log(".GetPartModuleConfigs end");
            */
            //ConfigNode myNode = new ConfigNode();
            //ConfigNode.LoadObjectFromConfig(this, myNode);
            //Debug.Log("LoadObjectFromConfig --> myNode.ToString()\n" + myNode.ToString());

            //ThrottleISPCurve.ToString();

            //ConfigNode[] myNodes;
            //ConfigNode myNode = new ConfigNode();
            ////string[] NodeValues;

            //Debug.Log("GTI_MultiModeEngineFX - ConfigNode Debug");

            //ConfigNode.LoadObjectFromConfig(this, myNode);
            //Debug.Log("LoadObjectFromConfig --> myNode.ToString()\n" + myNode.ToString());


            //myNodes = GameDatabase.Instance.GetConfigNodes("PART");

            //Debug.Log("GameDatabase.Instance.GetConfigs('PART').ToString()");
            //Debug.Log(GameDatabase.Instance.GetConfigs("PART").ToString());

            //Debug.Log("GameDatabase.Instance.GetConfigNode('root/').ToString()");
            //Debug.Log(GameDatabase.Instance.GetConfigNode("root/").ToString());

            //Debug.Log("GameDatabase.Instance.root.ToString()");
            //Debug.Log(GameDatabase.Instance.root.ToString());
            //Debug.Log("GameDatabase.Instance.root.url");
            //Debug.Log(GameDatabase.Instance.root.url);
            //Debug.Log("GameDatabase.Instance.root.AllConfigs.ToString()");
            //Debug.Log(GameDatabase.Instance.root.AllConfigs.ToString());


            //Debug.Log("this.part.protoPartSnapshot.ToString()");
            //Debug.Log(this.part.protoPartSnapshot.ToString());
            //Debug.Log("this.part.FindModulesImplementing<GTI_MultiModeEngineFX>().ToString()");
            //Debug.Log(this.part.FindModulesImplementing<GTI_MultiModeEngineFX>().ToString());


            //GTIndustries/Parts/Engines/IonEngine/GTI_IonEngine/GTI_ionEngine
            //Debug.Log("GameDatabase.Instance.GetConfigNode('GTIndustries/Parts/Engines/IonEngine/GTI_IonEngine/GTI_ionEngine')");
            //Debug.Log(GameDatabase.Instance.GetConfigNode("GTIndustries/Parts/Engines/IonEngine/GTI_IonEngine/GTI_ionEngine"));




            //Debug.Log(this.part.);


            //Debug.Log("this.part.name: " + this.part.name);
            //foreach (ConfigNode part_node in myNodes)
            //{
            //    //if (part_node.GetValue("name") == null) { continue; }
            //    Debug.Log(part_node.GetValue("name"));
            //    //Debug.Log(part_node.ToString());

            //    if (part_node.GetValue("name") == this.part.name)
            //    {
            //        myNode = part_node.GetNode("ThrottleISPCurve");
            //        Debug.Log("myNode.ToString()\n" + myNode.ToString());
            //    }
            //}
            //this.vessel.Parts[0].

            //Debug.Log("this.part.protoPartSnapshot.partData.nodes.ToString()");
            //Debug.Log(this.part.protoPartSnapshot.partData.ToString());


            //Debug.Log("for (int i = 0; i < PartLoader.Instance.loadedParts.Count; i++)  #" + PartLoader.Instance.loadedParts.Count);

            //ConfigNode myConfigNode = new ConfigNode();
            //ConfigNode[] myConfigNodes;
            //ConfigNode myFloatCurveNode = new ConfigNode();

            //for (int i = 0; i < PartLoader.Instance.loadedParts.Count; i++)
            //{
            //    if (this.part.name == PartLoader.Instance.loadedParts[i].name)
            //    {
            //        Debug.Log("name [" + i + "] " + PartLoader.Instance.loadedParts[i].name);
            //        Debug.Log("title [" + i + "] " + PartLoader.Instance.loadedParts[i].title);
            //        //Debug.Log("[" + i + "] " + PartLoader.Instance.loadedParts[i].partConfig.name);
            //        try { Debug.Log(PartLoader.Instance.loadedParts[i].partConfig.ToString()); } catch { Debug.LogWarning(".loadedParts[i].partConfig.ToString() threw error"); }
            //        //try { Debug.Log(PartLoader.Instance.loadedParts[i].partConfig.nodes.ToString()); } catch { Debug.LogWarning(".loadedParts[i].partConfig.nodes.ToString() threw error"); }
            //        //try { Debug.Log(PartLoader.Instance.loadedParts[i].partConfig.values.ToString()); } catch { Debug.LogWarning(".loadedParts[i].partConfig.values.ToString() threw error"); }
            //        //Debug.Log("partUrlConfig [" + i + "] " + PartLoader.Instance.loadedParts[i].partUrlConfig.ToString());
            //        Debug.Log("partUrl " + PartLoader.Instance.loadedParts[i].partUrl);
            //        Debug.Log("internalConfig [" + i + "] " + PartLoader.Instance.loadedParts[i].internalConfig.ToString());

            //        myConfigNode = PartLoader.Instance.loadedParts[i].partConfig;

            //        break;
            //    }

            //PartLoader.Instance.loadedParts[i].partConfig.ToString();
            //}
            //Debug.Log(".HasNode('GTI_MultiModeEngineFX'): " + myConfigNode.HasNode("GTI_MultiModeEngineFX"));
            //Debug.Log(".HasNode('MODULE'): " + myConfigNode.HasNode("MODULE"));
            //Debug.Log(".HasValue('GTI_MultiModeEngineFX'): " + myConfigNode.HasValue("GTI_MultiModeEngineFX"));

            //myConfigNodes = myConfigNode.GetNodes("MODULE");

            //foreach (ConfigNode n in myConfigNodes)
            //{
            //    Debug.Log("this.name " + this.name);        //_thismoduleName
            //    if (n.GetValue("name") == _thismoduleName)          //"GTI_MultiModeEngineFX"
            //    {
            //        if (n.TryGetNode("ThrottleISPCurve", ref myFloatCurveNode))
            //        {
            //            Debug.Log("GTI_MultiModeEngineFX loading FloatCurve");
            //            ThrottleISPCurve.Load(myFloatCurveNode);
            //            break;
            //        }
            //    }


            //    //foreach (ConfigNode.Value v in n.values)
            //    //{
            //    //    Debug.Log("name " + v.name + " --> Value " + v.value);
            //    //}
            //}

        }

        public override void OnLoad(ConfigNode node)
        {

            //onTestEvent = new EventData<>

            //base.OnLoad(node);
            //ConfigNode ThrottleISPCurveNode = node.GetNode("ThrottleISPCurve");
            //FloatCurve ThrottleISPFloatCurve = new FloatCurve();



            //ConfigNode myNode = new ConfigNode();
            //ConfigNode.LoadObjectFromConfig(this.part, myNode);
            //Debug.Log("LoadObjectFromConfig --> myNode.ToString()\n" + myNode.ToString());



            //Debug.Log("node.name  " + node.name);
            //Debug.Log("node.id  " + node.id);
            //Debug.Log("node.CountNodes  " + node.CountNodes);
            //Debug.Log("node.CountValues  " + node.CountValues);
            //Debug.Log("node.ToString\n" + node.ToString());
            //Debug.Log("ThrottleISPCurve.maxTime\n" + ThrottleISPCurve.maxTime);
            //Debug.Log("ThrottleISPCurve.minTime\n" + ThrottleISPCurve.minTime);
            //Debug.Log("ThrottleISPCurve.Curve.keys.ToString()\n" + ThrottleISPCurve.Curve.keys.ToString());
            //Debug.Log("Print Keys from KeyFrame");
            //foreach (Keyframe k in ThrottleISPCurve.Curve.keys)
            //{
            //    Debug.Log("k.time: " + k.time);
            //    Debug.Log("k.value: " + k.value);
            //    Debug.Log("k.inTangent: " + k.inTangent);
            //    Debug.Log("k.outTangent: " + k.outTangent);
            //    Debug.Log("k.tangentMode: " + k.tangentMode);
            //}

            //NodeValues = node.GetValues();

            //Debug.Log("foreach (string n in NodeValues)");
            //foreach (string n in NodeValues)
            //{
            //    Debug.Log("n  " + n);
            //}

            //Debug.Log("foreach (ConfigNode n in node.nodes)");
            //foreach (ConfigNode n in node.nodes)
            //{
            //    Debug.Log("n.name  " + n.name);
            //    Debug.Log("n.id  " + n.id);
            //    Debug.Log("n.CountNodes  " + n.CountNodes);
            //    Debug.Log("n.CountValues  " + n.CountValues);
            //}



            //Debug.Log("ThrottleISPCurveNode.CountNodes  " + ThrottleISPCurveNode.CountNodes);
            //Debug.Log("ThrottleISPCurveNode.CountValues  " + ThrottleISPCurveNode.CountValues);

            //foreach (ConfigNode.Value value in ThrottleISPCurveNode.values)
            //{
            //    Debug.Log("name: " + value.name + "   value: " + value.value);
            //}

            //Debug.Log("GTI_MultiModeEngineFX - Load to FloatCurve");
            //ThrottleISPFloatCurve.Load(ThrottleISPCurveNode);
            //Debug.Log("GTI_MultiModeEngineFX - Load to FloatCurve - DONE");


            //other tests with resources, and available resources
            //vessel.resourcePartSet.GetConnectedResourceTotals(int id, out double amount, out double maxAmount, [bool pulling = True]);
            //PartResourceLibrary.Instance.GetDefinition(arrResources[i]).id
        }

        /// <summary>
        /// Initialize settings for GTI_MultiModeEngine
        /// </summary>
        private void initializeSettings()
        {
            if (!_settingsInitialized)
            {
                Utilities Util = new Utilities();
                string[] arrEngineID, arrGUIengineID;        //, arrAddedCosts;   //, arrMinReqTech, arrMaxReqTech;  //, arrEngineAvailable;

                #region Split into Arrays
                arrEngineID = engineID.Trim().Split(';');
                GUIengineIDEmpty = Utilities.ArraySplitEvaluate(GUIengineID, out arrGUIengineID, ';');
                //AddedCostEmpty = Util.ArraySplitEvaluate(AddedCost, out arrAddedCosts, ';');
                //AddedCostEmpty = arrAddedCosts.Length == arrEngineID.Length ? AddedCostEmpty : false;       //Check if costs have been defined as expected
                #endregion

                #region Identify ModuleEngines in Scope
                //Find modules which is to be manipulated
                ModuleEngines = part.FindModulesImplementing<ModuleEnginesFX>();
                var toBeRemoved = new List<ModuleEnginesFX>();                      //for removal of irrelevant engine modules

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
                        //AddedCost = AddedCostEmpty ? 0f : float.Parse(arrAddedCosts[i])
                    });
                }

                //If there is an engine, and none is currently selected, then set the active one to be the first one
                if (ModuleEngines.Count > 0) { selPropFromChooseOption(); }

                //find the current engine and store it in "currentModuleEngine"
                foreach (ModuleEnginesFX moduleEngine in ModuleEngines)
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
            //UpdateCost();

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

        public float GetModuleCost(float defaultCost, ModifierStagingSituation sit)
        {
            Debug.Log("[GTI] System.Environment.StackTrace\n" + System.Environment.StackTrace);
            Debug.Log("[GTI] UnityEngine.StackTraceUtility.ExtractStackTrace()\n" + UnityEngine.StackTraceUtility.ExtractStackTrace());
            return UpdateCost();
            //part.GetModuleCosts(10000f);
            //throw new NotImplementedException();
        }
        public float UpdateCost()
        {
            try
            {
                if (HighLogic.LoadedSceneIsEditor)
                {
                    Debug.Log("[GTI] UpdateCost() in LoadedSceneIsEditor");
                    //return engineList[selectedPropulsion].AddedCost;
                    return 100000f + selectedPropulsion;
                }
                else
                {
                    Debug.Log("[GTI] UpdateCost() in NOT LoadedSceneIsEditor");
                    return 11f;
                }
            }
            catch
            {
                Debug.LogError("[GTI] UpdateCost() Error Catch");
                return 11111f;
            }
        }

        public ModifierChangeWhen GetModuleCostChangeWhen()
        {
            Debug.Log("[GTI] GetModuleCostChangeWhen()");
            return ModifierChangeWhen.FIXED;
            //try
            //{
            //    if (HighLogic.LoadedSceneIsEditor)
            //    {
            //        Debug.Log("GetModuleCostChangeWhen() in LoadedSceneIsEditor");
            //        //return ModifierChangeWhen.CONSTANTLY;
            //        return ModifierChangeWhen.FIXED;
            //    }
            //    else
            //    {
            //        Debug.Log("GetModuleCostChangeWhen() in NOT LoadedSceneIsEditor");
            //        return ModifierChangeWhen.FIXED;
            //    }
            //}
            //catch
            //{
            //    Debug.LogError("GetModuleCostChangeWhen() Error Catch");
            //    return ModifierChangeWhen.FIXED;
            //}
        }
    }
}