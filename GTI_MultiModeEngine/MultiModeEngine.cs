using GTI.Config;
using static GTI.Config.GTIConfig;
using GTI.GenericFunctions;
using System;
using System.Collections.Generic;
using UnityEngine;


/*
This module targets "ModuleEngines" modules for engine switching
*/

namespace GTI
{
    partial class GTI_MultiModeEngine : PartModule
    {

        private ConfigNode MultiModeConfigNode = null;
        private List<CustomTypes.engineMultiModeList> engineModeList = new List<CustomTypes.engineMultiModeList>();
        private ModuleEngines ModuleEngines;

        #region Engine parameters
        [KSPField]
        public string maxThrust = string.Empty;
        [KSPField]
        public string EngineTypes = string.Empty;
        [KSPField]
        public string heatProduction = string.Empty;

        [KSPField]
        public string useEngineResponseTime = string.Empty;
        [KSPField]
        public string engineAccelerationSpeed = string.Empty;
        [KSPField]
        public string engineDecelerationSpeed = string.Empty;

        [KSPField]
        public string atmChangeFlows = string.Empty;
        [KSPField]
        public string useVelCurves = string.Empty;
        [KSPField]
        public string useAtmCurves = string.Empty;
        #endregion

        #region Propellant parameters
        [KSPField]
        public string GUIengineModeNames = string.Empty;
        [KSPField]
        public string propellantNames = string.Empty;                                    //the list of propellant setups available to the switch.
        [KSPField]
        public string propellantRatios = string.Empty;                                   //the propellant ratios to set. NOTE: It is the actual fuel flow that defines the thrust => fuel usage.
        [KSPField]
        public string propIgnoreForISP = string.Empty;
        [KSPField]
        public string propDrawGauge = string.Empty;
        [KSPField]
        public string resourceFlowMode = string.Empty;
        #endregion


        //private int selectedMode;

        #region booleans
        private bool _settingsInitialized = false;
        
        private bool GUIengineModeNamesEmpty, PropellantNamesEmpty, PropellantRatiosEmpty;
        private bool PropDrawGaugeEmpty, PropIgnoreForISPEmpty, ResourceFlowModeEmpty;

        private bool MaxThrustEmpty, HeatProdEmpty, EngineTypesEmpty;
        private bool atmChangeFlowsEmpty;
        private bool UseEngineResponseTimeEmpty, EngineAccelerationSpeedEmpty, EngineDecelerationSpeedEmpty;

        private bool AtmosphereCurveEmpty;
        private bool useVelCurvesEmpty, useAtmCurvesEmpty;
        private bool VelCurveEmpty, AtmCurveEmpty;
        private bool useGTIthrottleISPCurvesEmpty;
        private bool GTIthrottleISPCurvesEmpty;        //Custom Curve

        [KSPField]
        public string useGTIthrottleISPCurves = string.Empty;
        #endregion

        //float currentISPfactor = 1;

        //Exposes private information
        public ConfigNode GetMultiModeConfigNode { get { return MultiModeConfigNode; } }
        public List<CustomTypes.engineMultiModeList> GetengineModeList { get { return engineModeList; } }

        private EventVoid onThrottleChangeEvent;

        public override void OnLoad(ConfigNode node)
        {
            GTIDebug.Log("[GTI] GTI_MultiModeEngine : OnLoad() : " + part.vessel.GetName(), iDebugLevel.DebugInfo);

            GTIDebug.Log("Before adding GTI_MultiModeEngine to onThrottleChange Event : " + part.vessel.GetName(), iDebugLevel.DebugInfo);
            onThrottleChangeEvent = GameEvents.FindEvent<EventVoid>("onThrottleChange");
            if (onThrottleChangeEvent != null)
            {
                GTIDebug.Log("Adding GTI_MultiModeEngine to onThrottleChange Event : " + part.vessel.GetName(), iDebugLevel.High);
                onThrottleChangeEvent.Add(onThrottleChange);
            }
            else
            {
                GTIDebug.Log("GTI_MultiModeEngine failed to be added to onThrottleChange Event : Event was null", iDebugLevel.Low);
            }
        }

        public override void OnStart(PartModule.StartState state)
        {
            GTIDebug.Log("GTI_MultiModeEngine : OnStart() : " + part.vessel.GetName(), iDebugLevel.DebugInfo);
            initializeSettings(true);
            //updatePropulsion(silentUpdate: true);


        }
        public override void OnStartFinished(StartState state)
        {
            GTIDebug.Log("[GTI] GTI_MultiModeEngine : OnStartFinished() : " + part.vessel.GetName(), iDebugLevel.DebugInfo);
            updatePropulsion(silentUpdate: true);

            //part.GetModuleCosts(10000f);

            /*
            Utilities Util = new Utilities();

            foreach (Keyframe key in atmosphereCurve.Curve.keys)
            {
                Debug.Log("GTI_MultiModeEngine atmosphereCurve \t" + key.time + " | " + key.value);
            }
            foreach (Keyframe key in velCurve.Curve.keys)
            {
                Debug.Log("GTI_MultiModeEngine velCurve \t" + key.time + " | " + key.value);
            }

            //Debug.Log(atmosphereCurve.Curve.ToString());
            //Debug.Log(velCurve.ToString());



            //ConfigNode myConfigNode = new ConfigNode();
            //for (int i = 0; i < PartLoader.Instance.loadedParts.Count; i++)
            //{
            //    if (this.part.name == PartLoader.Instance.loadedParts[i].name)
            //    {
            //        myConfigNode = PartLoader.Instance.loadedParts[i].partConfig;
            //        break;
            //    }
            //}
            //Debug.Log(myConfigNode.ToString());

            //ConfigNode[] MultiModeEngineNode = myConfigNode.GetNodes("MODULE");      //GTI_MultiModeEngineNode
            MultiModeConfigNode = Util.GetPartModuleConfig(this.part, "MODULE", "name", "GTI_MultiModeEngine");

            Debug.Log("\n" + MultiModeConfigNode.ToString());

            ConfigNode[] velCurves = MultiModeConfigNode.GetNodes("velCurve");

            

            //myConfigNode.GetNodes("velCurve").ToString();
            Debug.Log("velCurves.Length: " + velCurves.Length);
            foreach (ConfigNode n in velCurves)
            {
                Debug.Log("\n" + n.ToString());
            }

            FloatCurve testCurve = new FloatCurve();
            testCurve.Load(velCurves[0]);

            foreach (Keyframe key in testCurve.Curve.keys)
            {
                Debug.Log("GTI_MultiModeEngine testCurve \t" + key.time + " | " + key.value);
            }
            */

            /*
            for (int i = 0; i < engineModeList.Count; i++)
            {
                try { Debug.Log("engineModeList[" + i + "] - atmosphereCurve:\n" + engineModeList[i].atmosphereCurve.ToString()); } catch (System.Exception) { Debug.LogError("engineModeList[" + i + "] - atmosphereCurve:\n" + engineModeList[i].atmosphereCurve.ToString() + " throw"); }
                if (engineModeList[i].velCurve != null) { try { Debug.Log("engineModeList[" + i + "] - velCurve:\n" + engineModeList[i].velCurve.ToString()); } catch (System.Exception) { Debug.LogError("engineModeList[" + i + "] - velCurve:\n engineModeList[i].velCurve.ToString() throw"); } } else { Debug.Log("null value detected"); }
                try { Debug.Log("engineModeList[" + i + "] - atmCurve:\n" + engineModeList[i].atmCurve.ToString()); } catch (System.Exception) { Debug.LogError("engineModeList[" + i + "] - atmCurve:\n engineModeList[i].atmCurve.ToString() throw"); }
            }
            */

        }

        private void initializeSettings(bool loadConfigNode = false)
        {
            Debug.Log("[GTI] GTI_MultiModeEngine : initializeSettings() " + !_settingsInitialized);
            if (!_settingsInitialized)
            {
                //Utilities Util = new Utilities();
                //Load the part module configurations for GTI_MultiModeEngine
                if (MultiModeConfigNode == null && loadConfigNode == true) { MultiModeConfigNode = Utilities.GetPartModuleConfig(this.part, "MODULE", "name", "GTI_MultiModeEngine"); GTIDebug.Log("\n" + MultiModeConfigNode.ToString(), iDebugLevel.DebugInfo); } else { GTIDebug.Log("Allready loaded\n" + MultiModeConfigNode.ToString(), iDebugLevel.Medium); }
                //Debug.Log("GTI_MultiModeEngine - ConfigNode Loaded");

                #region Parse settings
                //bool boolParseResult;

                string[] arrGUIengineModeNames, arrPropellantNames, arrPropellantRatios;
                string[] arrPropDrawGauge, arrPropIgnoreForISP, arrResourceFlowMode;
                string[] arrMaxThrust, arrHeatProd, arrEngineTypes;
                string[] arratmChangeFlows;
                string[] arrUseEngineResponseTime, arrEngineAccelerationSpeed, arrEngineDecelerationSpeed;

                string[] arruseVelCurves, arruseAtmCurves;
                string[] arrusethrottleISPCurves;

                //Propellant level
                GUIengineModeNamesEmpty     = Utilities.ArraySplitEvaluate(GUIengineModeNames    , out arrGUIengineModeNames     , ';');
                PropellantNamesEmpty        = Utilities.ArraySplitEvaluate(propellantNames       , out arrPropellantNames        , ';');
                PropellantRatiosEmpty       = Utilities.ArraySplitEvaluate(propellantRatios      , out arrPropellantRatios       , ';');

                PropIgnoreForISPEmpty       = Utilities.ArraySplitEvaluate(propIgnoreForISP      , out arrPropIgnoreForISP       , ';');
                PropDrawGaugeEmpty          = Utilities.ArraySplitEvaluate(propDrawGauge         , out arrPropDrawGauge          , ';');
                ResourceFlowModeEmpty       = Utilities.ArraySplitEvaluate(resourceFlowMode      , out arrResourceFlowMode       , ';');

                MaxThrustEmpty              = Utilities.ArraySplitEvaluate(maxThrust             , out arrMaxThrust              , ';');

                HeatProdEmpty               = Utilities.ArraySplitEvaluate(heatProduction        , out arrHeatProd               , ';');
                atmChangeFlowsEmpty         = Utilities.ArraySplitEvaluate(atmChangeFlows        , out arratmChangeFlows         , ';');

                UseEngineResponseTimeEmpty      = Utilities.ArraySplitEvaluate(useEngineResponseTime     , out arrUseEngineResponseTime      , ';');
                EngineAccelerationSpeedEmpty    = Utilities.ArraySplitEvaluate(engineAccelerationSpeed   , out arrEngineAccelerationSpeed    , ';');
                EngineDecelerationSpeedEmpty    = Utilities.ArraySplitEvaluate(engineDecelerationSpeed   , out arrEngineDecelerationSpeed    , ';');

                EngineTypesEmpty            = Utilities.ArraySplitEvaluate(EngineTypes           , out arrEngineTypes            , ';');

                useVelCurvesEmpty           = Utilities.ArraySplitEvaluate(useVelCurves, out arruseVelCurves, ';');
                useAtmCurvesEmpty           = Utilities.ArraySplitEvaluate(useAtmCurves, out arruseAtmCurves, ';');

                useGTIthrottleISPCurvesEmpty   = Utilities.ArraySplitEvaluate(useGTIthrottleISPCurves, out arrusethrottleISPCurves, ';');

                //Get FloatCurves from the part
                ConfigNode[] atmosphereCurves   = MultiModeConfigNode.GetNodes("atmosphereCurve");
                ConfigNode[] velCurves          = MultiModeConfigNode.GetNodes("velCurve");
                ConfigNode[] atmCurves          = MultiModeConfigNode.GetNodes("atmCurve");
                ConfigNode[] throttleISPCurves  = MultiModeConfigNode.GetNodes("throttleISPCurve");

                //Test Curves for validity
                if (atmosphereCurves.Length == arrPropellantNames.Length)   { AtmosphereCurveEmpty = false; }   else { AtmosphereCurveEmpty = true; }
                if (velCurves.Length == arrPropellantNames.Length)          { VelCurveEmpty = false; }          else { VelCurveEmpty = true; }
                if (atmCurves.Length == arrPropellantNames.Length)          { AtmCurveEmpty = false; }          else { AtmCurveEmpty = true; }
                if (throttleISPCurves.Length == arrPropellantNames.Length)  { GTIthrottleISPCurvesEmpty = false; } else { GTIthrottleISPCurvesEmpty = true; }
                #endregion
                //Debug.Log("GTI_MultiModeEngine - Input processed to arrays");

                #region Populate list with settings
                for (int i = 0; i < arrPropellantNames.Length; i++)
                {
                    engineModeList.Add(new CustomTypes.engineMultiModeList()
                    {
                        GUIengineModeNames  = arrGUIengineModeNames[i],
                        Propellants         = arrPropellantNames[i],
                        propRatios          = arrPropellantRatios[i]
                    });


                    //Propellant level --> Propellant level is needed as the entire node is replaced.
                    engineModeList[i].propIgnoreForISP  = PropIgnoreForISPEmpty ? string.Empty   : arrPropIgnoreForISP[i];       //Has Default value "false"
                    engineModeList[i].propDrawGauge     = PropDrawGaugeEmpty    ? string.Empty   : arrPropDrawGauge[i];          //Has Default value "true"

                    engineModeList[i].resourceFlowMode  = ResourceFlowModeEmpty                 ? string.Empty  : arrResourceFlowMode[i];
                    engineModeList[i].setMaxThrust      = MaxThrustEmpty                        ? "0"           : arrMaxThrust[i];

                    engineModeList[i].heatProduction = HeatProdEmpty                            ? string.Empty : arrHeatProd[i];
                    engineModeList[i].atmChangeFlow = atmChangeFlowsEmpty                       ? string.Empty : arratmChangeFlows[i];
                    engineModeList[i].useEngineResponseTime = UseEngineResponseTimeEmpty        ? string.Empty : arrUseEngineResponseTime[i];
                    engineModeList[i].engineAccelerationSpeed = EngineAccelerationSpeedEmpty    ? string.Empty : arrEngineAccelerationSpeed[i];
                    engineModeList[i].engineDecelerationSpeed = EngineDecelerationSpeedEmpty    ? string.Empty : arrEngineDecelerationSpeed[i];
                    engineModeList[i].useVelCurve = useVelCurvesEmpty                           ? string.Empty : arruseVelCurves[i];
                    engineModeList[i].useAtmCurve = useAtmCurvesEmpty                           ? string.Empty : arruseAtmCurves[i];

                    engineModeList[i].SetUseGTIthrottleISPCurve = useGTIthrottleISPCurvesEmpty        ? string.Empty : arrusethrottleISPCurves[i];
                    //GTIDebug.Log("Parsing the usethrottleISPCurve");
                    //if (bool.TryParse(arrusethrottleISPCurves[i], out boolParseResult) && !usethrottleISPCurvesEmpty) { engineModeList[i].usethrottleISPCurve = boolParseResult; } else usethrottleISPCurvesEmpty = false;

                    //store FloatCurves
                    engineModeList[i].atmosphereCurve   = AtmosphereCurveEmpty      ? null : atmosphereCurves[i];
                    engineModeList[i].velCurve          = VelCurveEmpty             ? null : velCurves[i];
                    engineModeList[i].atmCurve          = AtmCurveEmpty             ? null : atmCurves[i];
                    engineModeList[i].GTIthrottleISPCurve  = GTIthrottleISPCurvesEmpty    ? null : throttleISPCurves[i];
                }
                #endregion
                //Debug.Log("GTI_MultiModeEngine - engineModeList populated");


                GTIDebug.Log("\n" +
                    propellantNames + "\t" + PropellantNamesEmpty + "\n" +
                    propellantRatios + "\t" + PropellantRatiosEmpty + "\n" +
                    propIgnoreForISP + "\t" + PropIgnoreForISPEmpty + "\n" +
                    propDrawGauge + "\t" + PropDrawGaugeEmpty
                    , iDebugLevel.DebugInfo);

                #region Identify ModuleEngines in Scope
                //Find module which is to be manipulated - NOTE: Only the first one is being handled. There should not be multiple when using this module
                ModuleEngines = part.FindModuleImplementing<ModuleEngines>();
                #endregion

                if (ModuleEngines != null) { selPropFromChooseOption(); }

                initializeGUI();
                _settingsInitialized = true;
            }
        } // END OF -- initializeSettings()

        /// <summary>
        /// Update propulsion
        /// </summary>
        private void updatePropulsion(bool silentUpdate = false)
        {
            #region declarations
            //Derive selectedMode from ChooseOption
            FindSelectedPropulsion();

            ConfigNode newPropNode = new ConfigNode();
            float targetRatio;
            bool targetIgnoreForISP, targetDrawGauge;
            string[] arrtargetPropellants, arrtargetRatios, arrtargetIgnoreForISP, arrtargetDrawGuage, arrtargetResourceFlowMode;

            float floatParseResult;
            bool boolParseResult;
            bool currentEngineState = false;
            //float maxISP = 0;
            #endregion

            //Debug.Log("Ignition Before\n" +
            //    "ModuleEngines.EngineIgnited:\t" + ModuleEngines.EngineIgnited +
            //    "ModuleEngines.getIgnitionState:\t" + ModuleEngines.getIgnitionState);

            //Get the Ignition state, i.e. is the engine shutdown or activated
            currentEngineState = ModuleEngines.getIgnitionState;
            //Shutdown the engine --> Removes the gauges, and make sense to do before changing propellant
            ModuleEngines.Shutdown();

            //Debug.Log("Ignition After\n" +
            //    "ModuleEngines.EngineIgnited:\t" + ModuleEngines.EngineIgnited +
            //    "ModuleEngines.getIgnitionState:\t" + ModuleEngines.getIgnitionState);

            //ModuleEngines.Flameout(
            //message: "Switch Engine State",
            //statusOnly: false,
            //showFX: false);

            #region ConfigNode Replace previous propellant node
            //Split cfg subsettings into arrays
            //Debug.Log("arrtargetPropellants");
            arrtargetPropellants = engineModeList[selectedMode].Propellants.Split(',');
            //Debug.Log("arrtargetRatios");
            arrtargetRatios         = engineModeList[selectedMode].propRatios.Split(',');
            //Debug.Log("arrtargetIgnoreForISP");
            arrtargetIgnoreForISP   = engineModeList[selectedMode].propIgnoreForISP.Split(',');
            //Debug.Log("arrtargetDrawGuage");
            arrtargetDrawGuage      = engineModeList[selectedMode].propDrawGauge.Split(',');
            //Debug.Log("arrtargetResourceFlowMode");
            arrtargetResourceFlowMode = ResourceFlowModeEmpty ? new string[0] : engineModeList[selectedMode].resourceFlowMode.Split(',');
            

            //Debug.Log("BEFORE for (int i = 0; i < arrtargetPropellants.Length; i++)");
            //Create new propellent nodes by looping them in.
            for (int i = 0; i < arrtargetPropellants.Length; i++)
            {
                //Debug.Log("for (int i = 0; i < arrtargetPropellants.Length; i++) -- i = " + i);
                //Get and convert ratios to floats. They should already have been verified in the CustomTypes.PropellantList class
                targetRatio = Convert.ToSingle(arrtargetRatios[i]);

                //if ignoreForISP have been set wrong or not at all, then we config it to false
                //Debug.Log("if (arrtargetIgnoreForISP.Length == arrtargetPropellants.Length)");
                if (!PropIgnoreForISPEmpty)                                    //if (arrtargetIgnoreForISP.Length == arrtargetPropellants.Length)
                {
                    if (!bool.TryParse(arrtargetIgnoreForISP[i], out targetIgnoreForISP)) { targetIgnoreForISP = false; }
                }
                else { targetIgnoreForISP = false; }
                if (!PropDrawGaugeEmpty)
                {
                    if (!bool.TryParse(arrtargetDrawGuage[i], out targetDrawGauge)) { targetDrawGauge = true; Debug.Log("MultiModeEngine: Parsing of arrtargetDrawGuage[i] failed. Set to true."); }
                }
                else { targetDrawGauge = true; /*Debug.Log("MultiModeEngine: PropDrawGauge was empty. Set to true.");*/ }

                //Debug.Log("!bool.TryParse(arrtargetIgnoreForISP[i], out targetIgnoreForISP)\ntargetIgnoreForISP: " + targetIgnoreForISP);

                ConfigNode propNode = newPropNode.AddNode("PROPELLANT");
                propNode.AddValue("name", arrtargetPropellants[i]);
                propNode.AddValue("ratio", targetRatio);
                propNode.AddValue("ignoreForIsp", targetIgnoreForISP);       //For now we assume all is counted for ISP           //targetIgnoreForISP[i]
                propNode.AddValue("DrawGauge", targetDrawGauge);             //I think the gauge  should always be shown

                if (!ResourceFlowModeEmpty)
                {
                    propNode.AddValue("resourceFlowMode", arrtargetResourceFlowMode[i]); //Debug.Log("propNode.AddValue('resourceFlowMode', " + arrtargetResourceFlowMode[i] + ")");
                }
            }
            //Update the engine with new propellant configuration
            //NOTICE: The original propellant nodes are overwritten, so we do not need to delete them
            //Debug.Log("Before ConfigNode Load\n" + newPropNode.ToString());
            ModuleEngines.Load(newPropNode);
            #endregion

            #region Curves
            if (!AtmosphereCurveEmpty) ModuleEngines.atmosphereCurve.Load(engineModeList[selectedMode].atmosphereCurve);
            if (!VelCurveEmpty) ModuleEngines.velCurve.Load(engineModeList[selectedMode].velCurve);
            if (!AtmCurveEmpty) ModuleEngines.atmCurve.Load(engineModeList[selectedMode].atmCurve);

            //Debug.Log("[GTI] ISP Float Curve : " + engineModeList[selectedMode].GetthrottleISPFloatCurve.Evaluate(0.5f).ToString());
            if (!GTIthrottleISPCurvesEmpty) GTIDebug.Log("ISP Float Curve : " + engineModeList[selectedMode].GTIthrottleISPCurve.ToString(), iDebugLevel.DebugInfo); else GTIDebug.Log("no ISP Float Curve : " + part.vessel.GetName(), iDebugLevel.DebugInfo);

            #endregion

            //Get maxISP from the atmosphere curve
            //maxISP = Utilities.KeyFrameGetMaxValue(ModuleEngines.atmosphereCurve.Curve.keys);

            //Set max Thrust and the corresponding fuelflow
            if (!MaxThrustEmpty) { ModuleEngines.maxThrust = engineModeList[selectedMode].maxThrust; }


            //ModuleEngines.maxFuelFlow = ModuleEngines.maxThrust / (ModuleEngines.atmosphereCurve.Evaluate(0f) * ModuleEngines.g);
            ModuleEngines.maxFuelFlow = PhysicsUtilities.calcFuelFlow(
                Thrust: ModuleEngines.maxThrust,                             //Thrust: propList[selectedPropellant].maxThrust, 
                                                                             //Density: propList[selectedPropellant].propDensity, 
                ISP: ModuleEngines.atmosphereCurve.Evaluate(0f),         //maxISP
                Gravity: ModuleEngines.g
                );



            GTIDebug.Log("Evaluate whether to use the onThrottleChange() method in updatePropulsion()");
            if (!ModuleEngines.useThrottleIspCurve && !useGTIthrottleISPCurvesEmpty)
            {
                //Update ISP curve based in the GTI throttleISPCurve
                //!throttleISPCurvesEmpty && !usethrottleISPCurvesEmpty && bool.Parse(engineModeList[selectedMode].usethrottleISPCurve) && !ModuleEngines.useThrottleIspCurve
                onThrottleChange();
            }
            else
            {
                //Write reason for not executing onThrottleChange()
                if (!useGTIthrottleISPCurvesEmpty) GTIDebug.Log("!useGTIthrottleISPCurvesEmpty: " + !useGTIthrottleISPCurvesEmpty, iDebugLevel.High);
                if (!ModuleEngines.useThrottleIspCurve) GTIDebug.Log("!ModuleEngines.useThrottleIspCurve: " + !ModuleEngines.useThrottleIspCurve, iDebugLevel.High);
            }
           

            GTIDebug.Log(
                "\nGravity = \t" + ModuleEngines.g +
                "\nMaxISP = \t" + ModuleEngines.atmosphereCurve.Evaluate(0f) +
                "\nMaxThrust = \t" + ModuleEngines.maxThrust +
                "\nResulting maxFuelFlow = \t" + ModuleEngines.maxFuelFlow
                , iDebugLevel.DebugInfo);

            //Debug.Log("Before misc settings");
            if (float.TryParse(engineModeList[selectedMode].heatProduction, out floatParseResult) && !HeatProdEmpty)   { ModuleEngines.heatProduction = floatParseResult; }
            if (bool.TryParse(engineModeList[selectedMode].atmChangeFlow, out boolParseResult) && !atmChangeFlowsEmpty) { ModuleEngines.atmChangeFlow = boolParseResult; }

            if (bool.TryParse(engineModeList[selectedMode].useEngineResponseTime, out boolParseResult) && !UseEngineResponseTimeEmpty) { ModuleEngines.useEngineResponseTime = boolParseResult; }
            if (float.TryParse(engineModeList[selectedMode].engineAccelerationSpeed, out floatParseResult) && !EngineAccelerationSpeedEmpty) { ModuleEngines.engineAccelerationSpeed = floatParseResult; }
            if (float.TryParse(engineModeList[selectedMode].engineDecelerationSpeed, out floatParseResult) && !EngineDecelerationSpeedEmpty) { ModuleEngines.engineDecelerationSpeed = floatParseResult; }

            if (bool.TryParse(engineModeList[selectedMode].useVelCurve, out boolParseResult) && !useVelCurvesEmpty) { ModuleEngines.useVelCurve = boolParseResult; }
            if (bool.TryParse(engineModeList[selectedMode].useAtmCurve, out boolParseResult) && !useAtmCurvesEmpty) { ModuleEngines.useAtmCurve = boolParseResult; }

            //Debug.Log("Before engine type");
            #region Set the engine type
            //[LiquidFuel, Nuclear, SolidBooster, Turbine, MonoProp, ScramJet, Electric, Generic, Piston]
            if (!EngineTypesEmpty) { ModuleEngines.engineType = Utilities.GetEngineType(engineModeList[selectedMode].engineType); }
            #endregion

            if (!silentUpdate) writeScreenMessage();

            //Restart engine if it was on before switching
            //moduleEngine.Flameout("Switch Engine State", false, false);
            //ModuleEngines.UnFlameout(false);
            //FlightInputHandler.state.mainThrottle = currentThrottleState;
            //Invoke("ActivateEngine", 0f);
            //ActivateEngine();
            
            if (currentEngineState) { ModuleEngines.Activate(); }
            //Debug.Log("Ignition After after\n" +
            //    "ModuleEngines.EngineIgnited:\t" + ModuleEngines.EngineIgnited +
            //    "ModuleEngines.getIgnitionState:\t" + ModuleEngines.getIgnitionState);
        }

        /// <summary>
        /// Run this method when the onThrottleChange event is observed
        /// </summary>
        private void onThrottleChange()
        {
            //!throttleISPCurvesEmpty && !usethrottleISPCurvesEmpty && bool.Parse(engineModeList[selectedMode].usethrottleISPCurve) && !ModuleEngines.useThrottleIspCurve

            //Criteria: "Active Vessel", "throttleCurve exists", "throttleCurve is to be used" and "stock throttle curve not implemented"
            if (this.part.vessel == FlightGlobals.ActiveVessel && !GTIthrottleISPCurvesEmpty && engineModeList[selectedMode].useGTIthrottleISPCurve && !ModuleEngines.useThrottleIspCurve)
            {
                float newISPfactor = engineModeList[selectedMode].GetGTIthrottleISPFloatCurve.Evaluate(ModuleEngines.requestedThrottle);
                //float ISPrefactor = newISPfactor / currentISPfactor;
                float time, value, inTangent, outTangent;
                AnimationCurve newCurve = new AnimationCurve();
                
                GTIDebug.Log("START-------------------------------------------------------------------------------- ", iDebugLevel.DebugInfo);
                GTIDebug.Log("part is in ActiveVessel - event accepted : " + this.part.vessel.GetName() + " : " + this.part.name, iDebugLevel.DebugInfo);
                GTIDebug.Log("FlightInputHandler.state.mainThrottle: " + FlightInputHandler.state.mainThrottle, iDebugLevel.DebugInfo);
                GTIDebug.Log("ModuleEngines.requestedThrottle: " + ModuleEngines.requestedThrottle, iDebugLevel.DebugInfo);
                GTIDebug.Log("ModuleEngines.realIsp: " + ModuleEngines.realIsp, iDebugLevel.DebugInfo);

                GTIDebug.Log("engineModeList[selectedMode].GetGTIthrottleISPFloatCurve: " + engineModeList[selectedMode].GetGTIthrottleISPFloatCurve.Evaluate(ModuleEngines.requestedThrottle), iDebugLevel.DebugInfo);


                GTIDebug.Log("newISPfactor:" + newISPfactor, iDebugLevel.DebugInfo);
                //Create new atmosphereCurve (ISP)
                for (int i = 0; i < engineModeList[selectedMode].GetatmosphereFloatCurve.Curve.keys.Length; i++)
                {
                    time = engineModeList[selectedMode].GetatmosphereFloatCurve.Curve.keys[i].time;                     //ModuleEngines.atmosphereCurve.Curve.keys[i].time;
                    value = engineModeList[selectedMode].GetatmosphereFloatCurve.Curve.keys[i].value;
                    inTangent = engineModeList[selectedMode].GetatmosphereFloatCurve.Curve.keys[i].inTangent;
                    outTangent = engineModeList[selectedMode].GetatmosphereFloatCurve.Curve.keys[i].outTangent;

                    newCurve.AddKey(new Keyframe(time, value * newISPfactor, inTangent, outTangent));
                }
                for (int i = 0; i < newCurve.keys.Length; i++)
                {
                    GTIDebug.Log("post newCurve.keys[" + i + "] \t.time = " + newCurve.keys[i].time + "\t.value = " + newCurve.keys[i].value, iDebugLevel.DebugInfo);
                }

                //Update the atmosphereCurve
                ModuleEngines.atmosphereCurve.Curve = newCurve;

                //Recalc fuel flow based on updated atmosphereCurve
                ModuleEngines.maxFuelFlow = PhysicsUtilities.calcFuelFlow(
                    Thrust: ModuleEngines.maxThrust,
                    ISP: ModuleEngines.atmosphereCurve.Evaluate(0f),
                    Gravity: ModuleEngines.g
                    );
                GTIDebug.Log("ModuleEngines.maxFuelFlow: " + ModuleEngines.maxFuelFlow, iDebugLevel.DebugInfo);

                //currentISPfactor = newISPfactor;

                GTIDebug.Log("END---------------------------------------------------------------------------------- ", iDebugLevel.DebugInfo);
            }
            else
            {
                //Write reason for not executing onThrottleChange()
                if (this.part.vessel == FlightGlobals.ActiveVessel) GTIDebug.Log("part not in ActiveVessel - event is ignored : " + this.part.vessel.GetName() + " : " + this.part.name, iDebugLevel.DebugInfo);
                if (!GTIthrottleISPCurvesEmpty) GTIDebug.Log("!GTIthrottleISPCurvesEmpty: " + !GTIthrottleISPCurvesEmpty, iDebugLevel.DebugInfo);
                if (engineModeList[selectedMode].useGTIthrottleISPCurve) GTIDebug.Log("engineModeList[selectedMode].useGTIthrottleISPCurve: " + engineModeList[selectedMode].useGTIthrottleISPCurve, iDebugLevel.DebugInfo);
                if (!ModuleEngines.useThrottleIspCurve) GTIDebug.Log("!ModuleEngines.useThrottleIspCurve: " + !ModuleEngines.useThrottleIspCurve, iDebugLevel.DebugInfo);
            }
        }

        private void OnDestroy()
        {
            GTIDebug.Log("[GTI] GTI_MultiModeEngine destroyed", iDebugLevel.Medium);
            if (onThrottleChangeEvent != null)
            {
                GTIDebug.Log("[GTI] Removing GTI_MultiModeEngine from onThrottleChange Event", iDebugLevel.Medium);
                onThrottleChangeEvent.Remove(onThrottleChange);
            }
        }

    } // END OF -- class GTI_MultiModeEngine
}