using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;
using System;

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
        private bool atmChangeFlowsEmpty, useVelCurvesEmpty, useAtmCurvesEmpty;
        private bool UseEngineResponseTimeEmpty, EngineAccelerationSpeedEmpty, EngineDecelerationSpeedEmpty;

        private bool AtmosphereCurveEmpty, VelCurveEmpty, AtmCurveEmpty;
        #endregion


        //Exposes private information
        public ConfigNode GetMultiModeConfigNode { get { return MultiModeConfigNode; } }
        public List<CustomTypes.engineMultiModeList> GetengineModeList { get { return engineModeList; } }

        public override void OnStart(PartModule.StartState state)
        {
            initializeSettings(true);
            //updatePropulsion(silentUpdate: true);
        }
        public override void OnStartFinished(StartState state)
        {
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

            for (int i = 0; i < engineModeList.Count; i++)
            {
                try { Debug.Log("engineModeList[" + i + "] - atmosphereCurve:\n" + engineModeList[i].atmosphereCurve.ToString()); } catch (System.Exception) { Debug.LogError("engineModeList[" + i + "] - atmosphereCurve:\n" + engineModeList[i].atmosphereCurve.ToString() + " throw"); }
                if (engineModeList[i].velCurve != null) { try { Debug.Log("engineModeList[" + i + "] - velCurve:\n" + engineModeList[i].velCurve.ToString()); } catch (System.Exception) { Debug.LogError("engineModeList[" + i + "] - velCurve:\n engineModeList[i].velCurve.ToString() throw"); } } else { Debug.Log("null value detected"); }
                try { Debug.Log("engineModeList[" + i + "] - atmCurve:\n" + engineModeList[i].atmCurve.ToString()); } catch (System.Exception) { Debug.LogError("engineModeList[" + i + "] - atmCurve:\n engineModeList[i].atmCurve.ToString() throw"); }
            }

        }

        private void initializeSettings(bool loadConfigNode = false)
        {
            if (!_settingsInitialized)
            {
                Utilities Util = new Utilities();
                //Load the part module configurations for GTI_MultiModeEngine
                if (MultiModeConfigNode == null && loadConfigNode == true) { MultiModeConfigNode = Util.GetPartModuleConfig(this.part, "MODULE", "name", "GTI_MultiModeEngine"); Debug.Log("\n" + MultiModeConfigNode.ToString()); } else { Debug.Log("Allready loaded\n" + MultiModeConfigNode.ToString()); }
                Debug.Log("GTI_MultiModeEngine - ConfigNode Loaded");

                #region Parse settings
                string[] arrGUIengineModeNames, arrPropellantNames, arrPropellantRatios;
                string[] arrPropDrawGauge, arrPropIgnoreForISP, arrResourceFlowMode;
                string[] arrMaxThrust, arrHeatProd, arrEngineTypes;
                string[] arratmChangeFlows, arruseVelCurves, arruseAtmCurves;
                string[] arrUseEngineResponseTime, arrEngineAccelerationSpeed, arrEngineDecelerationSpeed;

                //Propellant level
                GUIengineModeNamesEmpty     = Util.ArraySplitEvaluate(GUIengineModeNames    , out arrGUIengineModeNames     , ';');
                PropellantNamesEmpty        = Util.ArraySplitEvaluate(propellantNames       , out arrPropellantNames        , ';');
                PropellantRatiosEmpty       = Util.ArraySplitEvaluate(propellantRatios      , out arrPropellantRatios       , ';');

                PropIgnoreForISPEmpty       = Util.ArraySplitEvaluate(propIgnoreForISP      , out arrPropIgnoreForISP       , ';');
                PropDrawGaugeEmpty          = Util.ArraySplitEvaluate(propDrawGauge         , out arrPropDrawGauge          , ';');
                ResourceFlowModeEmpty       = Util.ArraySplitEvaluate(resourceFlowMode      , out arrResourceFlowMode       , ';');

                MaxThrustEmpty              = Util.ArraySplitEvaluate(maxThrust             , out arrMaxThrust              , ';');

                HeatProdEmpty               = Util.ArraySplitEvaluate(heatProduction        , out arrHeatProd               , ';');
                atmChangeFlowsEmpty         = Util.ArraySplitEvaluate(atmChangeFlows        , out arratmChangeFlows         , ';');

                UseEngineResponseTimeEmpty      = Util.ArraySplitEvaluate(useEngineResponseTime     , out arrUseEngineResponseTime      , ';');
                EngineAccelerationSpeedEmpty    = Util.ArraySplitEvaluate(engineAccelerationSpeed   , out arrEngineAccelerationSpeed    , ';');
                EngineDecelerationSpeedEmpty    = Util.ArraySplitEvaluate(engineDecelerationSpeed   , out arrEngineDecelerationSpeed    , ';');

                EngineTypesEmpty            = Util.ArraySplitEvaluate(EngineTypes           , out arrEngineTypes            , ';');

                useVelCurvesEmpty           = Util.ArraySplitEvaluate(useVelCurves, out arruseVelCurves, ';');
                useAtmCurvesEmpty           = Util.ArraySplitEvaluate(useAtmCurves, out arruseAtmCurves, ';');

                //Get FloatCurves from the part
                ConfigNode[] atmosphereCurves   = MultiModeConfigNode.GetNodes("atmosphereCurve");
                ConfigNode[] velCurves          = MultiModeConfigNode.GetNodes("velCurve");
                ConfigNode[] atmCurves          = MultiModeConfigNode.GetNodes("atmCurve");

                //Test Curves for validity
                if (atmosphereCurves.Length == arrPropellantNames.Length)   { AtmosphereCurveEmpty = false; }   else { AtmosphereCurveEmpty = true; }
                if (velCurves.Length == arrPropellantNames.Length)          { VelCurveEmpty = false; }          else { VelCurveEmpty = true; }
                if (atmCurves.Length == arrPropellantNames.Length)          { AtmCurveEmpty = false; }          else { AtmCurveEmpty = true; }
                #endregion
                Debug.Log("GTI_MultiModeEngine - Input processed to arrays");

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


                    //store FloatCurves
                    engineModeList[i].atmosphereCurve   = AtmosphereCurveEmpty  ? null : atmosphereCurves[i];
                    engineModeList[i].velCurve          = VelCurveEmpty         ? null : velCurves[i];
                    engineModeList[i].atmCurve          = AtmCurveEmpty         ? null : atmCurves[i];
                }
                #endregion
                Debug.Log("GTI_MultiModeEngine - engineModeList populated");


                Debug.Log("\n" +
                    propellantNames + "\t"  + PropellantNamesEmpty + "\n" +
                    propellantRatios + "\t" + PropellantRatiosEmpty + "\n" +
                    propIgnoreForISP + "\t" + PropIgnoreForISPEmpty + "\n" +
                    propDrawGauge + "\t"    + PropDrawGaugeEmpty
                    );


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
            PhysicsUtilities EngineCalc = new PhysicsUtilities();
            Utilities Util = new Utilities();

            //Derive selectedMode from ChooseOption
            FindSelectedPropulsion();

            ConfigNode newPropNode = new ConfigNode();
            float targetRatio;
            bool targetIgnoreForISP, targetDrawGauge;
            string[] arrtargetPropellants, arrtargetRatios, arrtargetIgnoreForISP, arrtargetDrawGuage, arrtargetResourceFlowMode;

            float floatParseResult;
            bool boolParseResult;
            bool currentEngineState = false;
            float maxISP = 0;
            #endregion

            Debug.Log("Ignition Before\n" + 
                "ModuleEngines.EngineIgnited:\t" + ModuleEngines.EngineIgnited +
                "ModuleEngines.getIgnitionState:\t" + ModuleEngines.getIgnitionState);

            //Get the Ignition state, i.e. is the engine shutdown or activated
            currentEngineState = ModuleEngines.getIgnitionState;
            //Shutdown the engine --> Removes the gauges, and make sense to do before changing propellant
            ModuleEngines.Shutdown();

            Debug.Log("Ignition After\n" +
                "ModuleEngines.EngineIgnited:\t" + ModuleEngines.EngineIgnited +
                "ModuleEngines.getIgnitionState:\t" + ModuleEngines.getIgnitionState);

            //ModuleEngines.Flameout(
            //message: "Switch Engine State",
            //statusOnly: false,
            //showFX: false);

            #region ConfigNode Replace previous propellant node
            //Split cfg subsettings into arrays
            Debug.Log("arrtargetPropellants");
            arrtargetPropellants = engineModeList[selectedMode].Propellants.Split(',');
            Debug.Log("arrtargetRatios");
            arrtargetRatios         = engineModeList[selectedMode].propRatios.Split(',');
            Debug.Log("arrtargetIgnoreForISP");
            arrtargetIgnoreForISP   = engineModeList[selectedMode].propIgnoreForISP.Split(',');
            Debug.Log("arrtargetDrawGuage");
            arrtargetDrawGuage      = engineModeList[selectedMode].propDrawGauge.Split(',');
            Debug.Log("arrtargetResourceFlowMode");
            arrtargetResourceFlowMode = ResourceFlowModeEmpty ? new string[0] : engineModeList[selectedMode].resourceFlowMode.Split(',');


            Debug.Log("BEFORE for (int i = 0; i < arrtargetPropellants.Length; i++)");
            //Create new propellent nodes by looping them in.
            for (int i = 0; i < arrtargetPropellants.Length; i++)
            {
                Debug.Log("for (int i = 0; i < arrtargetPropellants.Length; i++) -- i = " + i);
                //Get and convert ratios to floats. They should already have been verified in the CustomTypes.PropellantList class
                targetRatio = Convert.ToSingle(arrtargetRatios[i]);

                //if ignoreForISP have been set wrong or not at all, then we config it to false
                Debug.Log("if (arrtargetIgnoreForISP.Length == arrtargetPropellants.Length)");
                if (!PropIgnoreForISPEmpty)                                   //if (arrtargetIgnoreForISP.Length == arrtargetPropellants.Length)
                {
                    if (!bool.TryParse(arrtargetIgnoreForISP[i], out targetIgnoreForISP)) { targetIgnoreForISP = false; }
                }
                else { targetIgnoreForISP = false; }
                if (!PropDrawGaugeEmpty)
                {
                    if (!bool.TryParse(arrtargetDrawGuage[i], out targetDrawGauge)) { targetDrawGauge = true; }
                }
                else { targetDrawGauge = true; }

                //Debug.Log("!bool.TryParse(arrtargetIgnoreForISP[i], out targetIgnoreForISP)\ntargetIgnoreForISP: " + targetIgnoreForISP);

                ConfigNode propNode = newPropNode.AddNode("PROPELLANT");
                Debug.Log("propNode.AddValue('name', arrtargetPropellants[i])");
                propNode.AddValue("name", arrtargetPropellants[i]);
                propNode.AddValue("ratio", targetRatio);
                propNode.AddValue("ignoreForIsp", targetIgnoreForISP);       //For now we assume all is counted for ISP           //targetIgnoreForISP[i]
                Debug.Log("propNode.AddValue('DrawGauge', targetDrawGauge)");
                propNode.AddValue("DrawGauge", targetDrawGauge);      //I think the gauge  should always be shown
                Debug.Log("propNode.AddValue('resourceFlowMode', arrtargetResourceFlowMode[i])");
                if (!ResourceFlowModeEmpty) propNode.AddValue("resourceFlowMode", arrtargetResourceFlowMode[i]);
            }
            //Update the engine with new propellant configuration
            //NOTICE: The original propellant nodes are overwritten, so we do not need to delete them
            Debug.Log("Before ConfigNode Load");
            ModuleEngines.Load(newPropNode);
            #endregion

            #region Curves
            if (!AtmosphereCurveEmpty) ModuleEngines.atmosphereCurve.Load(engineModeList[selectedMode].atmosphereCurve);

            #endregion

            //Get maxISP from the atmosphere curve
            maxISP = Util.KeyFrameGetMaxValue(ModuleEngines.atmosphereCurve.Curve.keys);

            //Set max Thrust and the corresponding fuelflow
            if (!MaxThrustEmpty) { ModuleEngines.maxThrust = engineModeList[selectedMode].maxThrust; }

            //ModuleEngines.maxFuelFlow = ModuleEngines.maxThrust / (ModuleEngines.atmosphereCurve.Evaluate(0f) * ModuleEngines.g);
            ModuleEngines.maxFuelFlow = EngineCalc.calcFuelFlow(
                Thrust: ModuleEngines.maxThrust,                             //Thrust: propList[selectedPropellant].maxThrust, 
                                                                             //Density: propList[selectedPropellant].propDensity, 
                ISP: ModuleEngines.atmosphereCurve.Evaluate(0f),         //maxISP
                Gravity: ModuleEngines.g
                );
            Debug.Log(
                "\nGravity = \t"                + ModuleEngines.g +
                "\nMaxISP = \t"                 + ModuleEngines.atmosphereCurve.Evaluate(0f) +
                "\nMaxThrust = \t"              + ModuleEngines.maxThrust +
                "\nResulting maxFuelFlow = \t"  + ModuleEngines.maxFuelFlow
                );

            Debug.Log("Before misc settings");
            if (float.TryParse(engineModeList[selectedMode].heatProduction, out floatParseResult) && !HeatProdEmpty)   { ModuleEngines.heatProduction = floatParseResult; }
            if (bool.TryParse(engineModeList[selectedMode].atmChangeFlow, out boolParseResult) && !atmChangeFlowsEmpty) { ModuleEngines.atmChangeFlow = boolParseResult; }

            if (bool.TryParse(engineModeList[selectedMode].useEngineResponseTime, out boolParseResult) && !UseEngineResponseTimeEmpty) { ModuleEngines.useEngineResponseTime = boolParseResult; }
            if (float.TryParse(engineModeList[selectedMode].engineAccelerationSpeed, out floatParseResult) && !EngineAccelerationSpeedEmpty) { ModuleEngines.engineAccelerationSpeed = floatParseResult; }
            if (float.TryParse(engineModeList[selectedMode].engineDecelerationSpeed, out floatParseResult) && !EngineDecelerationSpeedEmpty) { ModuleEngines.engineDecelerationSpeed = floatParseResult; }

            if (bool.TryParse(engineModeList[selectedMode].useVelCurve, out boolParseResult) && !useVelCurvesEmpty) { ModuleEngines.useVelCurve = boolParseResult; }
            if (bool.TryParse(engineModeList[selectedMode].useAtmCurve, out boolParseResult) && !useAtmCurvesEmpty) { ModuleEngines.useAtmCurve = boolParseResult; }

            Debug.Log("Before engine type");
            #region Set the engine type
            //[LiquidFuel, Nuclear, SolidBooster, Turbine, MonoProp, ScramJet, Electric, Generic, Piston]
            if (!EngineTypesEmpty)
            {
                switch (engineModeList[selectedMode].engineType)
                {
                    case "LiquidFuel":
                        ModuleEngines.engineType = EngineType.LiquidFuel;
                        break;
                    case "Nuclear":
                        ModuleEngines.engineType = EngineType.Nuclear;
                        break;
                    case "SolidBooster":
                        ModuleEngines.engineType = EngineType.SolidBooster;
                        break;
                    case "Turbine":
                        ModuleEngines.engineType = EngineType.Turbine;
                        break;
                    case "MonoProp":
                        ModuleEngines.engineType = EngineType.MonoProp;
                        break;
                    case "ScramJet":
                        ModuleEngines.engineType = EngineType.ScramJet;
                        break;
                    case "Electric":
                        ModuleEngines.engineType = EngineType.Electric;
                        break;
                    case "Generic":
                        ModuleEngines.engineType = EngineType.Generic;
                        break;
                    case "Piston":
                        ModuleEngines.engineType = EngineType.Piston;
                        break;
                    default:
                        //Do nothing
                        //ModuleEngines.engineType = EngineType.LiquidFuel;
                        break;
                }
            }
            #endregion

            if (!silentUpdate) writeScreenMessage();

            //Restart engine if it was on before switching
            //moduleEngine.Flameout("Switch Engine State", false, false);
            //ModuleEngines.UnFlameout(false);
            //FlightInputHandler.state.mainThrottle = currentThrottleState;
            //Invoke("ActivateEngine", 0f);
            //ActivateEngine();
            if (currentEngineState) { ModuleEngines.Activate(); }
            Debug.Log("Ignition After after\n" +
                "ModuleEngines.EngineIgnited:\t" + ModuleEngines.EngineIgnited +
                "ModuleEngines.getIgnitionState:\t" + ModuleEngines.getIgnitionState);
            }

    } // END OF -- class GTI_MultiModeEngine
}