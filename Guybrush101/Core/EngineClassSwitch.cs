using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GTI.GenericFunctions;


//####################################################################################################################################
// Credits:
// Thanks to FSfuelswitch and InterstellarFuelSwitch for inspiration
// Thanks to NathanKell for invaluable advise on KSP forums
//####################################################################################################################################



namespace GTI
{
    partial class GTI_EngineClassSwitch : PartModule
    {
        //Tech Requirement
        [KSPField]
        public string requiredTech = string.Empty;
        //Engine manipulation
        #region Engine parameters
        [KSPField]
        public string engineID = string.Empty;                                                         //the engine to affect. Needed if multiple engines are present in the part.
        [KSPField]
        public string maxThrust = string.Empty;
        [KSPField]
        public string EngineTypes = string.Empty;
        [KSPField]
        public string heatProduction = string.Empty;
        [KSPField(isPersistant = true)]
        public string engineAvailable = string.Empty;

        [KSPField]
        public string useEngineResponseTime = string.Empty;
        [KSPField]
        public string engineAccelerationSpeed = string.Empty;
        [KSPField]
        public string engineDecelerationSpeed = string.Empty;
        #endregion

        #region Propellant parameters
        [KSPField]
        public string propellantNames = string.Empty;                                    //the list of propellant setups available to the switch.
        [KSPField]
        public string propellantRatios = string.Empty;                                   //the propellant ratios to set. NOTE: It is the actual fuel flow that defines the thrust => fuel usage.
        [KSPField]
        public string propellantIgnoreForIsp = string.Empty;
        [KSPField]
        public string propellantDrawGauge = string.Empty;
        #endregion
        
        #region KeyFrame / Float Curve parameters
        //(float time, float value, float inTangent, float outTangent)
        [KSPField]
        public string atmosphereCurveKeys = string.Empty;
        [KSPField]
        public string velCurveKeys = string.Empty; //"0 0 0 0;1 1 1 1";                 //White space for parameters, ";" for keys and "|" for each setup linked to a propellant setup. Provide keys for all propellants or none, else wierd thing will happen
        [KSPField]
        public string atmCurveKeys = string.Empty; //"0 0 0 0;1 1 0 0";                                  

        [KSPField]
        public string atmChangeFlows = string.Empty;
        [KSPField]
        public string useVelCurves = string.Empty;
        [KSPField]
        public string useAtmCurves = string.Empty;
        #endregion

        #region Empty value indicators (boolean)
        private bool requiredTechEmpty = true, engineAvailableEmpty = true;
        private bool iniGUIpropellantNamesEmpty = true;
        private bool MaxThrustEmpty = true;
        private bool EngineTypesEmpty = true;
        private bool heatProductionEmpty = true;

        //propellantNames is mandatory
        //propellantRatios is mandatory
        private bool propellantIgnoreForIspEmpty = true;
        private bool propellantDrawGaugeEmpty = true;

        private bool atmosphereCurveEmpty = true;
        private bool velCurveEmpty = true;
        private bool atmCurveEmpty = true;

        private bool atmChangeFlowsEmpty = true;
        private bool useVelCurvesEmpty = true;
        private bool useAtmCurvesEmpty = true;

        private bool useEngineResponseTimeEmpty, engineAccelerationSpeedEmpty, engineDecelerationSpeedEmpty;
        #endregion

        #region Arrays and Lists
        //Arrays for data processing
        //private string[] arrPropellantNames;    //for the split list of prop names
        //private string[] arrPropellantRatios;   //for the split list of prop ratios
        //private string[] arrMaxThrust, arrPropDrawGauge, arrHeatProd, arrEngineTypes, arrPropIgnoreForISP, arratmChangeFlows, arruseVelCurves, arruseAtmCurves;
        //private string[] arrAtmosphereCurve, arrPropellantVelCurve, arrPropellantAtmCurve;

        //Customtype for list of relevant settings
        private List<CustomTypes.PropellantList> propList = new List<CustomTypes.PropellantList>();

        [KSPField]
        public string debugMode = "false";

        //For the engines modules
        private List<ModuleEngines> ModuleEngines;
        private ModuleEngines currentModuleEngine;
        #endregion

        #region Other class level declarations
        private bool _settingsInitialized = false;
        [KSPField]
        public string _startDelay = "1";
        //private ConfigNode SourceEffectsNode;
        #endregion

        //####### Beginning of logics
        #region Events, OnLoad, OnStart
        public override void OnLoad(ConfigNode node)
        {
            try
            {
                //base.OnLoad(node);

                //if (HighLogic.LoadedScene == GameScenes.LOADING)
                //{
                //    SourceEffectsNode = node.GetNode("SOURCEEFFECTS");
                //    Debug.Log("SourceEffectsNode.CountNodes: " + SourceEffectsNode.CountNodes);

                //    foreach (ConfigNode nodeEffect in SourceEffectsNode.nodes)
                //    {
                //        Debug.Log("name: " + nodeEffect.name + " id: " + nodeEffect.id);
                //    }
                //}
                    

            }
            catch(Exception e)
            {
                Debug.LogError("'SourceEffectsNode = node.GetNode('EFFECTS')' Error " + e.Message);
                throw;
            }

        }

        public override void OnStart(PartModule.StartState state)
        {
            
            //Debug.Log("Tech start: " + ResearchAndDevelopment.GetTechnologyState("start").ToString());
            //Debug.Log("Tech basicRocketry: " + ResearchAndDevelopment.GetTechnologyState("basicRocketry").ToString());
            //Debug.Log("Tech basicRocketry: " + ResearchAndDevelopment.GetTechnologyState("basicRocketry"));


            //bool isThatModLoaded = AssemblyLoader.loadedAssemblies.Any(a => a.name == "KerbalEngineer");
            //isThatModLoaded = AssemblyLoader.loadedAssemblies.Any(a => a.name == "KerbalEngineer.Unity");
            //isThatModLoaded = AssemblyLoader.loadedAssemblies.Any(a => a.name == "MightyPirate");     //File name
            //Debug.Log("MightyPirate " + isThatModLoaded);

            InitializeSettings();
            if (selectedPropellant == -1)
            {
                selectedPropellant = 0;
            }
            Debug.Log("OnStart() --> propList.Count: " + propList.Count);
            if (propList.Count > 0)
            {
                //Delaying the update in the OnStart ensures that the effects are correctly initialized
                //part.LoadEffects(SourceEffectsNode);

                //ConfigNode effectsNode = part.partInfo.partConfig.GetNode("EFFECTS");
                //part.partInfo.partConfig.
                

                Invoke("updateEngineModuleFromOnStart", Single.Parse(_startDelay));
            }
        }
        private void updateEngineModuleFromOnStart()
        {
            Debug.Log("OnStart() --> updateEngineModule(false, 'OnStart')");
            updateEngineModule(false, "OnStart");
        }
        #endregion

        #region Settings
        public void InitializeSettings()
        {

            Debug.Log("EngineClassSwitch --> InitializeSettings");
            if (!_settingsInitialized)
            {
                Utilities Util = new Utilities();

                string[] arrPropellantNames, arrPropellantRatios;
                string[] arrPropDrawGauge, arrPropIgnoreForISP;
                string[] arrrequiredTech, arrEngineAvailable, arrGUIpropellantNames;
                string[] arrMaxThrust, arrHeatProd, arrEngineTypes, arratmChangeFlows, arruseVelCurves, arruseAtmCurves;
                string[] arrAtmosphereCurve, arrPropellantVelCurve, arrPropellantAtmCurve;
                string[] arrUseEngineResponseTime, arrEngineAccelerationSpeed, arrEngineDecelerationSpeed;


                #region Parse Arrays
                //Parse the strings into arrays of information
                //Propellant names and ratios are mandatory information

                //Propellant level
                arrPropellantNames          = propellantNames.Trim().Split(';');                //arrPropellantNames should now have the propellant names and combinations
                arrPropellantRatios         = propellantRatios.Trim().Split(';');               //arrPropellantRatios should now have the propellant ratios and combinations
                propellantIgnoreForIspEmpty = Util.ArraySplitEvaluate(propellantIgnoreForIsp,   out arrPropIgnoreForISP     , ';');
                propellantDrawGaugeEmpty    = Util.ArraySplitEvaluate(propellantDrawGauge,      out arrPropDrawGauge        , ';');

                //Required Technology & GUI
                requiredTechEmpty           = Util.ArraySplitEvaluate(requiredTech,             out arrrequiredTech         , ';');
                engineAvailableEmpty        = Util.ArraySplitEvaluate(engineAvailable,          out arrEngineAvailable      , ';');     //***
                iniGUIpropellantNamesEmpty  = Util.ArraySplitEvaluate(iniGUIpropellantNames,    out arrGUIpropellantNames   , ';');

                //Engine level
                MaxThrustEmpty              = Util.ArraySplitEvaluate(maxThrust,                out arrMaxThrust            , ';');
                EngineTypesEmpty            = Util.ArraySplitEvaluate(EngineTypes,              out arrEngineTypes          , ';');
                heatProductionEmpty         = Util.ArraySplitEvaluate(heatProduction,           out arrHeatProd             , ';');
                atmChangeFlowsEmpty         = Util.ArraySplitEvaluate(atmChangeFlows,           out arratmChangeFlows       , ';');
                useVelCurvesEmpty           = Util.ArraySplitEvaluate(useVelCurves,             out arruseVelCurves         , ';');
                useAtmCurvesEmpty           = Util.ArraySplitEvaluate(useAtmCurves,             out arruseAtmCurves         , ';');

                //Engine level curves
                atmosphereCurveEmpty        = Util.ArraySplitEvaluate(atmosphereCurveKeys,      out arrAtmosphereCurve      , '|');
                velCurveEmpty               = Util.ArraySplitEvaluate(velCurveKeys,             out arrPropellantVelCurve   , '|');
                atmCurveEmpty               = Util.ArraySplitEvaluate(atmCurveKeys,             out arrPropellantAtmCurve   , '|');

                useEngineResponseTimeEmpty  = Util.ArraySplitEvaluate(useEngineResponseTime,    out arrUseEngineResponseTime, ';');
                engineAccelerationSpeedEmpty = Util.ArraySplitEvaluate(engineAccelerationSpeed, out arrEngineAccelerationSpeed, ';');
                engineDecelerationSpeedEmpty = Util.ArraySplitEvaluate(engineDecelerationSpeed, out arrEngineDecelerationSpeed, ';');



                //Test if the two arrays are compatible                 <------ This test should be extended!
                if (arrPropellantNames.Length != arrPropellantRatios.Length)
                {
                    Debug.LogError("EngineClassSwitch: Error on InitializeSettings() - \nPropellant names (" + arrPropellantNames.Length + "pcs) and ratios (" + arrPropellantRatios.Length + "pcs) does not match\nConfig file error");
                }
                //Added a check for editor, where it will reevaluate, so that the parts does not need to be reloaded
                if ((arrEngineAvailable.Length != arrPropellantNames.Length) || HighLogic.LoadedSceneIsEditor)
                { engineAvailable = string.Empty; engineAvailableEmpty = Util.ArraySplitEvaluate(engineAvailable, out arrEngineAvailable, ';'); }   //***

                #endregion

                #region Populate Propellant List
                try
                { 
                //Populate the Propellant List (propList) from the array for more intuitive access to this information later
                for (int i = 0; i < arrPropellantRatios.Length; i++)
                    {
                        propList.Add(new CustomTypes.PropellantList()
                        {
                            Propellants = arrPropellantNames[i],
                            PropRatios  = arrPropellantRatios[i],
                            engineAvailable = engineAvailableEmpty ? true : bool.Parse(arrEngineAvailable[i])
                        });

                        propList[i].requiredTech        = requiredTechEmpty             ? "start"   : arrrequiredTech[i];
                        //propList[i].GUIpropellantNames  = iniGUIpropellantNamesEmpty    ? string.Empty : arrGUIpropellantNames[i];
                        propList[i].GUIpropellantNames = iniGUIpropellantNamesEmpty ? arrPropellantNames[i] : arrGUIpropellantNames[i];

                        //Propellant level --> Propellant level is needed as the entire node is replaced.
                        propList[i].propIgnoreForISP    = propellantIgnoreForIspEmpty   ? "false"   : arrPropIgnoreForISP[i];       //Has Default value "false"
                        propList[i].propDrawGauge       = propellantDrawGaugeEmpty      ? "true"    : arrPropDrawGauge[i];          //Has Default value "true"

                        //Engine level
                        propList[i].setMaxThrust        = MaxThrustEmpty                ? "ignore"  : arrMaxThrust[i];       //No default - Ignore when updating
                        propList[i].engineType          = EngineTypesEmpty              ? ""        : arrEngineTypes[i];     //No default - Ignore when updating
                        propList[i].heatProduction      = heatProductionEmpty           ? "0"       : arrHeatProd[i];        //No default - Ignore when updating

                        propList[i].atmChangeFlow       = atmChangeFlowsEmpty               ? "true"    : arratmChangeFlows[i];
                        propList[i].useVelCurve         = useVelCurvesEmpty                 ? "true"    : arruseVelCurves[i];
                        propList[i].useAtmCurve         = useAtmCurvesEmpty                 ? "true"    : arruseAtmCurves[i];

                        propList[i].useEngineResponseTime = useEngineResponseTimeEmpty      ? string.Empty : arrUseEngineResponseTime[i];
                        propList[i].engineAccelerationSpeed = engineAccelerationSpeedEmpty  ? string.Empty : arrEngineAccelerationSpeed[i];
                        propList[i].engineDecelerationSpeed = engineDecelerationSpeedEmpty  ? string.Empty : arrEngineDecelerationSpeed[i];

                        if (!atmosphereCurveEmpty)  //No default - Ignore when updating
                        {
                            propList[i].atmosphereCurve = arrAtmosphereCurve[i];
                        }
                        if (!velCurveEmpty)         //No default - Ignore when updating
                        {
                            propList[i].velCurve = arrPropellantVelCurve[i];
                        }
                        if (!atmCurveEmpty)         //No default - Ignore when updating
                        {
                            propList[i].atmCurve = arrPropellantAtmCurve[i];
                        }

                        if (engineAvailableEmpty)
                        {
                            propList[i].engineAvailable = (ResearchAndDevelopment.GetTechnologyState(propList[i].requiredTech) == RDTech.State.Available);
                        }
                    }
                    //Initialize the effects for EngineSwitch

                    //***INITIALIZING OF EFFECT MANIPULATION DEACTIVATED DUE TO MALFUNCTION
                    //InitializeEffects();
                }
                catch(Exception e)
                {
                    Debug.LogError("Populate Propellant List failed");
                    throw e;
                }
                #endregion
                //ResearchAndDevelopment.GetTechnologyState("basicRocketry")
                //Debug.Log("Remove tech which is not available. propList.Count before: " + propList.Count);
                for (int i = propList.Count - 1; i >= 0; i--)
                {
                    //if ((ResearchAndDevelopment.GetTechnologyState(propList[i].requiredTech) == RDTech.State.Unavailable)) { propList.RemoveAt(i); }
                    if (!propList[i].engineAvailable) { propList.RemoveAt(i); }
                }
                if (engineAvailableEmpty)
                {
                    foreach (var item in propList)
                    {
                        //item.engineAvailable;
                        engineAvailable += item.engineAvailable.ToString() + ";";
                    }
                    engineAvailable = engineAvailable.Substring(0, engineAvailable.Length - 1);
                }

                if (propList.Count == 0)
                {
                    availableInFlight = false;
                    availableInEditor = false;
                }
                //foreach (var item in propList)
                //{
                //    Debug.Log(item.Propellants);
                //}
                //Debug.Log("Done removing tech which is not available. propList.Count after: " + propList.Count);

                #region Identify ModuleEngines in Scope
                //Find modules which is to be manipulated
                ModuleEngines = part.FindModulesImplementing<ModuleEngines>();
                    var toBeRemoved = new List<ModuleEngines>();                      //for removal of irrelevant engine modules
                    foreach (var moduleEngine in ModuleEngines)
                    {
                        if (moduleEngine.engineID == engineID || string.IsNullOrEmpty(engineID) || engineID.Trim().Length == 0)       //"string.IsNullOrEmpty(engineID) || engineID.Trim().Length==0" is used instead of IsNullOrWhiteSpace()
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
                        ModuleEngines.Remove(remove);
                        //part.RemoveModule(remove);
                    }

                //Now ModulesEngines should have exactly the engine modules in scope
                #endregion

                #region GUI Update
                //Debug.Log("--> initializeGUI()");
                //initializeGUI();
                initializeGUI_2();

                //Show Debug GUI?
                if (bool.Parse(debugMode)) { Events["DEBUG_ENGINESSWITCH"].guiActive = true; Events["DEBUG_ENGINESSWITCH"].guiActiveEditor = true; Events["DEBUG_ENGINESSWITCH"].active = true; Debug.Log("Engine Switch debugMode activated"); }
                else { Events["DEBUG_ENGINESSWITCH"].guiActive = false; Events["DEBUG_ENGINESSWITCH"].guiActiveEditor = false; Events["DEBUG_ENGINESSWITCH"].active = false; /*Debug.Log("debugMode deactivated");*/ }
                //Debug.Log("--> initializeGUI() is Done");
                #endregion

                //set settings to initialized
                _settingsInitialized = true;
            }
        }
        #endregion
        
        
        #region UpdatePart Engine Module
        private void ActivateEngine()
        {
            if (currentEngineState == true) { currentModuleEngine.Activate(); }
            //currentModuleEngine.useEngineResponseTime = true;
        }
        private void ShutdownEngine()
        {
            //Shutdown the engine --> Removes the gauges, and make sense to do before changing propellant
            currentModuleEngine.Shutdown();
        }

        bool currentEngineState = false;
        //float currentThrottleState;

        private void updateEngineModule(bool calledByPlayer, string callingFunction = "player")
        {
            string[] arrtargetPropellants, arrtargetRatios, arrtargetIgnoreForISP, arrtargetDrawGuage;
            float targetRatio;
            bool targetIgnoreForISP;
            float maxISP = 0;
            float floatParseResult;

            ConfigNode newPropNode = new ConfigNode();
            Utilities MiscFx = new Utilities();
            PhysicsUtilities EngineCalc = new PhysicsUtilities();

            writeScreenMessage();

            foreach (var moduleEngine in ModuleEngines)
            {

                //Debug.Log("moduleEngine.isEnabled: " + moduleEngine.isEnabled);
                //moduleEngine.isEnabled = !moduleEngine.isEnabled;
                //Debug.Log("moduleEngine.isEnabled: " + moduleEngine.isEnabled);
                //bool engineState = false;
                //float ThrottleState;

                currentModuleEngine = moduleEngine;
                //Get the Ignition state, i.e. is the engine shutdown or activated
                currentEngineState = currentModuleEngine.getIgnitionState;
                ShutdownEngine();

                //Deactivate effects to force KSP to update them as intended
                //currentModuleEngine.DeactivateLoopingFX();
                //currentModuleEngine.DeactivatePowerFX();
                //currentModuleEngine.DeactivateRunningFX();

                


                //currentThrottleState = FlightInputHandler.state.mainThrottle;
                //FlightInputHandler.state.mainThrottle = 0f;
                //currentModuleEngine.useEngineResponseTime = false;

                //moduleEngine.maxFuelFlow = 0f;

                //Invoke("ShutdownEngine",2f);

                ////Get the Ignition state, i.e. is the engine shutdown or activated
                //engineState = currentModuleEngine.getIgnitionState;
                ////Shutdown the engine --> Removes the gauges, and make sense to do before changing propellant
                ////Debug.Log("engine shutdown");
                //currentModuleEngine.Shutdown();
                ////Debug.Log("1) FlightInputHandler.state.mainThrottle : " + FlightInputHandler.state.mainThrottle);
                //ThrottleState = FlightInputHandler.state.mainThrottle;
                //FlightInputHandler.state.mainThrottle = 0f;

                //StartCoroutine(wait());
                //Debug.Log("2) FlightInputHandler.state.mainThrottle : " + FlightInputHandler.state.mainThrottle);

                moduleEngine.Flameout(
                    message: "Switch Engine State", 
                    statusOnly: false, 
                    showFX: false);

                //Invoke("shortwait", 2f);
                
                //Debug.Log("engine shutdown completed");

                //Split cfg subsettings into arrays
                arrtargetPropellants    = propList[selectedPropellant].Propellants.Split(',');
                arrtargetRatios         = propList[selectedPropellant].PropRatios.Split(',');
                arrtargetIgnoreForISP   = propList[selectedPropellant].propIgnoreForISP.Split(',');
                arrtargetDrawGuage      = propList[selectedPropellant].propDrawGauge.Split(',');

                //Create new propellent nodes by looping them in.
                for (int i = 0; i < arrtargetPropellants.Length; i++)
                {
                    //Get and convert ratios to floats. They should already have been verified in the CustomTypes.PropellantList class
                    targetRatio = Convert.ToSingle(arrtargetRatios[i]);
                    
                    //if ignoreForISP have been set wrong or not at all, then we config it to false
                    if (arrtargetIgnoreForISP.Length == arrtargetPropellants.Length)
                    {
                        if (!bool.TryParse(arrtargetIgnoreForISP[i], out targetIgnoreForISP)) { targetIgnoreForISP = false; }
                        //targetIgnoreForISP = arrtargetIgnoreForISP[i];
                    } else { targetIgnoreForISP = false; }

                    //Debug.Log("!bool.TryParse(arrtargetIgnoreForISP[i], out targetIgnoreForISP)\ntargetIgnoreForISP: " + targetIgnoreForISP);

                    ConfigNode propNode = newPropNode.AddNode("PROPELLANT");
                    propNode.AddValue("name", arrtargetPropellants[i]);
                    propNode.AddValue("ratio", targetRatio);
                    propNode.AddValue("ignoreForIsp", targetIgnoreForISP);       //For now we assume all is counted for ISP           //targetIgnoreForISP[i]
                    propNode.AddValue("DrawGauge", arrtargetDrawGuage[i]);      //I think the gauge  should always be shown
                }
                //Update the engine with new propellant configuration
                //NOTICE: The original propellant nodes are overwritten, so we do not need to delete them
                moduleEngine.Load(newPropNode);
                
                //Debug.Log("Start of curve editing");
                //Change the atmosphere curve (ISP)
                if (!atmosphereCurveEmpty)
                {
                    //Debug.Log("Setting atmosphere Curve (ISP) " + atmosphereCurveKeys);
                    moduleEngine.atmosphereCurve.Curve.keys =
                        MiscFx.KeyFrameFromString(
                            inKeys: propList[selectedPropellant].atmosphereCurve,
                            iniKeys: moduleEngine.atmosphereCurve.Curve.keys
                            );
                }
                //Change the Velocity curve
                if (!velCurveEmpty)                                         //(string.IsNullOrEmpty(velCurveKeys) || velCurveKeys.Trim().Length == 0))
                {
                    //Debug.Log("Setting Velocity Curve " + velCurveKeys);
                    moduleEngine.velCurve.Curve.keys =
                        MiscFx.KeyFrameFromString(
                            inKeys: propList[selectedPropellant].velCurve,
                            iniKeys: moduleEngine.velCurve.Curve.keys
                            );
                }
                //Change the Atm curve
                if (!atmCurveEmpty)                                         //(string.IsNullOrEmpty(atmCurveKeys) || atmCurveKeys.Trim().Length == 0))
                {
                    //Debug.Log("Setting Atm Curve: "+ atmCurveKeys);
                    moduleEngine.atmCurve.Curve.keys = 
                        MiscFx.KeyFrameFromString(
                            inKeys: propList[selectedPropellant].atmCurve,
                            iniKeys: moduleEngine.atmCurve.Curve.keys
                            );
                }
               
                //Set which curves to use
                if (!atmChangeFlowsEmpty)   { moduleEngine.atmChangeFlow    = bool.Parse(propList[selectedPropellant].atmChangeFlow); }
                if (!useVelCurvesEmpty)     { moduleEngine.useVelCurve      = bool.Parse(propList[selectedPropellant].useVelCurve); }
                if (!useAtmCurvesEmpty)     { moduleEngine.useAtmCurve      = bool.Parse(propList[selectedPropellant].useAtmCurve); }

                //Get maxISP from the atmosphere curve
                maxISP = MiscFx.KeyFrameGetMaxValue(moduleEngine.atmosphereCurve.Curve.keys);
                //moduleEngine.atmosphereCurve.FindMinMaxValue

                //Set max Thrust and the corresponding fuelflow
                if (!MaxThrustEmpty)        { moduleEngine.maxThrust        = propList[selectedPropellant].maxThrust; }

                //maxFuelFlow = engine.maxThrust / (engine.atmosphereCurve.Evaluate(0f) * engine.g)
                moduleEngine.maxFuelFlow = EngineCalc.calcFuelFlow(
                    Thrust: moduleEngine.maxThrust,                             //Thrust: propList[selectedPropellant].maxThrust, 
                    //Density: propList[selectedPropellant].propDensity, 
                    ISP: maxISP
                    );

                if (Single.TryParse(propList[selectedPropellant].heatProduction, out floatParseResult) && !heatProductionEmpty)
                { moduleEngine.heatProduction = floatParseResult; }

                //Set the engine type
                //[LiquidFuel, Nuclear, SolidBooster, Turbine, MonoProp, ScramJet, Electric, Generic, Piston]
                if (!EngineTypesEmpty)
                {
                    switch (propList[selectedPropellant].engineType)
                    {
                        case "LiquidFuel":
                            moduleEngine.engineType = EngineType.LiquidFuel;
                            break;
                        case "Nuclear":
                            moduleEngine.engineType = EngineType.Nuclear;
                            break;
                        case "SolidBooster":
                            moduleEngine.engineType = EngineType.SolidBooster;
                            break;
                        case "Turbine":
                            moduleEngine.engineType = EngineType.Turbine;
                            break;
                        case "MonoProp":
                            moduleEngine.engineType = EngineType.MonoProp;
                            break;
                        case "ScramJet":
                            moduleEngine.engineType = EngineType.ScramJet;
                            break;
                        case "Electric":
                            moduleEngine.engineType = EngineType.Electric;
                            break;
                        case "Generic":
                            moduleEngine.engineType = EngineType.Generic;
                            break;
                        case "Piston":
                            moduleEngine.engineType = EngineType.Piston;
                            break;
                        default:
                            moduleEngine.engineType = EngineType.LiquidFuel;
                            break;
                    }
                }

                moduleEngine.useEngineResponseTime = useEngineResponseTimeEmpty ? false : bool.Parse(propList[selectedPropellant].useEngineResponseTime);
                moduleEngine.engineAccelerationSpeed = engineAccelerationSpeedEmpty ? 0 : Single.Parse(propList[selectedPropellant].engineAccelerationSpeed);
                moduleEngine.engineDecelerationSpeed = engineDecelerationSpeedEmpty ? 0 : Single.Parse(propList[selectedPropellant].engineDecelerationSpeed);

                //if (!useEngineResponseTimeEmpty)
                //{
                //    EngineEffectNode.SetValue("useEngineResponseTime", propList[selectedPropellant].useEngineResponseTime, true);
                //    Debug.Log("EngineEffectNode.SetValue('useEngineResponseTime', " + propList[selectedPropellant].useEngineResponseTime + ", true);");
                //}
                //if (!engineAccelerationSpeedEmpty)
                //{
                //    EngineEffectNode.SetValue("engineAccelerationSpeed", propList[selectedPropellant].engineAccelerationSpeed, true);
                //    Debug.Log("EngineEffectNode.SetValue('engineAccelerationSpeed', " + propList[selectedPropellant].engineAccelerationSpeed + ", true);");
                //}
                //if (!engineDecelerationSpeedEmpty)
                //{
                //    EngineEffectNode.SetValue("engineDecelerationSpeed", propList[selectedPropellant].engineDecelerationSpeed, true);
                //    Debug.Log("EngineEffectNode.SetValue('engineDecelerationSpeed', " + propList[selectedPropellant].engineDecelerationSpeed + ", true);");
                //}

                //Update the effects
                //updateEngineModuleEffects(
                //    calledByPlayer: calledByPlayer, 
                //    callingFunction: callingFunction, 
                //    moduleEngine: moduleEngine);

                //Write the propellant setup to the right click GUI
                GUIpropellantNames = propList[selectedPropellant].Propellants.Replace(",", ", ");
                
                if (iniGUIpropellantNamesEmpty)
                {   //Default naming if no user defined names are found
                    GUIpropellantNames = "[" + selectedPropellant + "] " + propList[selectedPropellant].Propellants.Replace(",", ", ");
                }
                else
                {   //User defined names
                    GUIpropellantNames = propList[selectedPropellant].GUIpropellantNames;                               //iniGUIpropellantNames.Trim().Split(';')[selectedPropellant];
                }
                
                //Write on screen message
                //ScreenMessages.PostScreenMessage("message", 3f, ScreenMessageStyle.UPPER_LEFT);
                ScreenMessages.PostScreenMessage(propList[selectedPropellant].GUIpropellantNames, 1.5f, ScreenMessageStyle.UPPER_CENTER);

                //Restart engine if it was on before switching
                //moduleEngine.Flameout("Switch Engine State", false, false);
                currentModuleEngine.UnFlameout(false);
                //FlightInputHandler.state.mainThrottle = currentThrottleState;
                //Invoke("ActivateEngine", 0f);
                //ActivateEngine();
                if (currentEngineState) { moduleEngine.Activate(); }
            }
        }
        #endregion

        #region VAB Information
        public override string GetInfo()
        {
            try
            {
                //we need to run the InitializeSettings here, because the OnStart does not run before this.
                //InitializeSettings();

                //string strOutInfo = string.Empty;
                System.Text.StringBuilder strOutInfo = new System.Text.StringBuilder();
                //string[] _propellants, _propratios;

                strOutInfo.AppendLine("Propellants available");
                //foreach (CustomTypes.PropellantList item in propList)
                //{
                //    strOutInfo.AppendLine(item.Propellants.Replace(",",", "));
                //}
                strOutInfo.AppendLine(propellantNames.Replace(";", "; "));
                return strOutInfo.ToString();
            }
            catch (Exception e)
            {
                Debug.LogError("EngineClassSwitch GetInfo Error " + e.Message);
                throw;
            }
        }
        #endregion
        






        #region --------------------------------Debugging---------------------------------------
        [KSPEvent(active = true, guiActive = true, guiActiveEditor = true, guiName = "DEBUG")]
        public void DEBUG_ENGINESSWITCH()
        {
            InitializeSettings();
            PhysicsUtilities Calc = new PhysicsUtilities();
            Utilities MiscFx = new Utilities();
            //System.Text.StringBuilder BuildString = new System.Text.StringBuilder();
            float Density = propList[selectedPropellant].propDensity;
            //int i = 0;

            foreach (var moduleEngine in ModuleEngines)
            {
                moduleEngine.Events["Activate"].active = !moduleEngine.Events["Activate"].active;
                moduleEngine.Events["Shutdown"].active = !moduleEngine.Events["Activate"].active;
                moduleEngine.Events["Activate"].guiActive = moduleEngine.Events["Activate"].active;
                moduleEngine.Events["Shutdown"].guiActive = !moduleEngine.Events["Activate"].active;

                //if (moduleEngine.Events["Activate"].active)
                //{
                //    //moduleEngine.Events["Activate"].active = false;
                //    moduleEngine.Events["Shutdown"].active = false;
                //    moduleEngine.Events["Activate"].guiActive = false;
                //    moduleEngine.Events["Shutdown"].guiActive = false;
                //}
                //else
                //{
                //    //moduleEngine.Events["Activate"].active = true;
                //    moduleEngine.Events["Shutdown"].active = true;
                //    moduleEngine.Events["Activate"].guiActive = true;
                //    moduleEngine.Events["Shutdown"].guiActive = true;
                //}

                //Debug.Log("CGF velCurveKeys: " + velCurveKeys);
                //Debug.Log("CGF atmCurveKeys: " + atmCurveKeys);
                Debug.Log(
                    "requestedThrottle: " + moduleEngine.requestedThrottle * 100 + "%" +
                    "\nmaxThrust: " + moduleEngine.maxThrust +
                    "\nresultingThrust: " + moduleEngine.resultingThrust +
                    "\nmaxFuelFlow: " + moduleEngine.maxFuelFlow +
                    "\nrequestedMassFlow: " + moduleEngine.requestedMassFlow +
                    "\nmaxFuelRate (calc): " + Calc.calcFuelRateFromFuelFlow(moduleEngine.maxFuelFlow, Density) +
                    "\nFuelRate (calc): " + Calc.calcFuelRateFromFuelFlow(moduleEngine.requestedMassFlow, Density) +
                    "\nWeighted Density of " + propList[selectedPropellant].Propellants + " is " + Density + "Kg/L"
                    );

                foreach(var propellant in moduleEngine.propellants)
                {
                    Debug.Log(
                        "foreach(var propellant in moduleEngine.propellants)" +
                        "\nPropellant: " + propellant.name +
                        "\nratio: " + propellant.ratio +
                        "\ndrawStackGauge: " + propellant.drawStackGauge +
                        "\nignoreForISP: " + propellant.ignoreForIsp);
                }
                

                //for (int j = 0; j < moduleEngine.atmosphereCurve.Curve.length; j++)
                //{
                //    Debug.Log(moduleEngine.atmosphereCurve.Curve[j].time + ", " + moduleEngine.atmosphereCurve.Curve[j].value);
                //}

                //DEBUG
                /*i = 0;
                float CurveTimeValue = 0;
                foreach (Keyframe key in moduleEngine.atmosphereCurve.Curve.keys)
                {
                    Debug.Log("atmosphereKey[" + i + "]: " + key.time + " " + key.value + " " + key.inTangent + " " + key.outTangent);
                    if (CurveTimeValue < key.value) { CurveTimeValue = key.value;  }
                    i++;
                }*/
                Debug.Log(MiscFx.KeyFrameGetToCFG(moduleEngine.atmosphereCurve.Curve.keys, "atmosphereKeys --> "));
                /*Debug.Log(
                    "ISP: " + CurveTimeValue +
                    "\nmaxFuelRate should be: " + Calc.calcFuelFlow(moduleEngine.maxThrust, Density, CurveTimeValue)
                );*/
                Debug.Log(MiscFx.KeyFrameGetToCFG(moduleEngine.atmCurve.Curve.keys, "atmCurveKeys --> "));
                Debug.Log(MiscFx.KeyFrameGetToCFG(moduleEngine.velCurve.Curve.keys, "velCurveKeys --> "));


                foreach (var engineEvent in moduleEngine.Events)
                {
                    Debug.Log("moduleEngine.Events" +
                        "\nGUIName: " + engineEvent.GUIName +
                        "\nid: " + engineEvent.id +
                        "\nname: " + engineEvent.name +
                        "\nactive: " + engineEvent.active +
                        "\nassigned: " + engineEvent.assigned +
                        "\ncategory: " + engineEvent.category +
                        "\nexternalToEVAOnly: " + engineEvent.externalToEVAOnly +
                        "\nguiActive: " + engineEvent.guiActive +
                        "\nguiActiveEditor: " + engineEvent.guiActiveEditor +
                        "\nguiActiveUncommand: " + engineEvent.guiActiveUncommand +
                        "\nguiActiveUnfocused: " + engineEvent.guiActiveUnfocused +
                        "\nguiIcon: " + engineEvent.guiIcon +
                        "\nunfocusedRange: " + engineEvent.unfocusedRange);
                }
            }
            
            /*
            foreach (var bodyItem in FlightGlobals.Bodies)
            {
                Debug.Log("Planet stats -->" +
                    "\nBodyName: " + bodyItem.bodyName +
                    "\nRadius: " + bodyItem.Radius +
                    "\nsphereOfInfluence: " + bodyItem.sphereOfInfluence +
                    "\nGetInstanceID: " + bodyItem.GetInstanceID() +
                    "\natmospherePressureCurve:" + MiscFx.KeyFrameGetToCFG(bodyItem.atmospherePressureCurve.Curve.keys, "bodyItem.atmospherePressureCurve.Curve.keys --> ")
                    );

                //Clear string Builder
                //BuildString.Length = 0;
                //BuildString.Capacity = 16;
            }
            */
        }
        #endregion


    } //class EngineClassSwitch : PartModule 
} //namespace Guybrush101
