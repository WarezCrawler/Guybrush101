using static GTI.GTIConfig;
using static GTI.Utilities;
using static GTI.PhysicsUtilities;
//using GTI.CustomTypes;

using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace GTI
{
    //Engine Type
    public class MultiModeEngine : IMultiMode
    {
        public int moduleIndex { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }

        //public string propellants { get; set; }
        private string _propellants;
        private string[] _propellantsArray;
        public string propellants
        {
            get => _propellants;
            set
            {
                _propellants = value;
                _propellantsArray = value.Split(',');
            }
        }
        public string[] GetPropellants => _propellantsArray;    //property get:

        private string _propRatios;
        private string[] _propRatiosArray;
        //For storing and retrieving propellant ratios
        public string propRatios
        {
            get => _propRatios;
            set
            {
                //string[] arrInString;
                //bool booparse;

                _propRatios = value;

                //Evaluate if multi propellants are in the string, put result to _propAmount
                _propRatiosArray = value.Split(',');
                //_propAmount = _propRatiosArray.Length;
                try
                {
                    foreach (string item in _propRatiosArray)
                    {
                        //booparse = Single.TryParse(item, out numvalue);
                        if (!Single.TryParse(item, out float numvalue))
                            GTIDebug.LogWarning("CustomTypes.PropellantList -> Could not parse propellant ratio " + item + " into integer.", iDebugLevel.Low);
                    }
                }
                catch (Exception e) { GTIDebug.LogError("CustomTypes.PropellantList -> Could not parse propellant ratio into integer.\n" + value + "\nError trown:\n" + e); throw e; }
                //CalcDensity(propellants, _propRatios, propIgnoreForISP);
            }
        }
        public string[] GetPropellantRatios
        {
            get
            {
                return _propRatiosArray;
            }
        }


        public string propIgnoreForISP { get; set; }
        public string propDrawGauge { get; set; }
        private string _resourceFlowMode;
        public string resourceFlowMode
        {
            get => _resourceFlowMode;
            set
            {
                switch (value)
                {
                    case "ALL_VESSEL":
                        _resourceFlowMode = value;
                        break;
                    case "ALL_VESSEL_BALANCE":
                        _resourceFlowMode = value;
                        break;
                    case "NO_FLOW":
                        _resourceFlowMode = value;
                        break;
                    case "NULL":
                        _resourceFlowMode = value;
                        break;
                    case "STACK_PRIORITY_SEARCH":
                        _resourceFlowMode = value;
                        break;
                    case "STAGE_PRIORITY_FLOW":
                        _resourceFlowMode = value;
                        break;
                    case "STAGE_PRIORITY_FLOW_BALANCE":
                        _resourceFlowMode = value;
                        break;
                    case "STAGE_STACK_FLOW":
                        _resourceFlowMode = value;
                        break;
                    case "STAGE_STACK_FLOW_BALANCE":
                        _resourceFlowMode = value;
                        break;
                    default:
                        _resourceFlowMode = string.Empty;
                        break;
                }
            }
        }
        public float maxThrust { get; set; }
        public string SetMaxThrust
        {
            set
            {
                Single.TryParse(value, out float outMaxThrust);       //float outMaxThrust = 0;
                maxThrust = outMaxThrust;
            }
        }
        public string heatProduction { get; set; }
        public string engineType { get; set; }
        public string atmChangeFlow { get; set; }
        public string useEngineResponseTime { get; set; }
        public string engineAccelerationSpeed { get; set; }
        public string engineDecelerationSpeed { get; set; }

        private ConfigNode _atmosphereCurve = new ConfigNode();
        public FloatCurve atmosphereFloatCurve { get; private set; } = new FloatCurve();
        public ConfigNode atmosphereCurve
        {
            get => _atmosphereCurve;
            set
            {
                _atmosphereCurve = value;

                if (value != null)
                {
                    GTIDebug.Log("Before load of atmosphereFloatCurve", iDebugLevel.DebugInfo);
                    atmosphereFloatCurve.Load(value);
                }
            }
        }
        public string useVelCurve { get; set; }
        public ConfigNode velCurve { get; set; }
        public string useAtmCurve { get; set; }
        public ConfigNode atmCurve { get; set; }

        public bool useGTIthrottleISPCurve { get; set; } = false;
        private ConfigNode _GTIthrottleISPCurve = new ConfigNode();
        public FloatCurve GTIthrottleISPFloatCurve { get; private set; } = new FloatCurve();
        public ConfigNode GTIthrottleISPCurve
        {
            get => _GTIthrottleISPCurve;
            set
            {
                _GTIthrottleISPCurve = value;
                if (value != null)
                {
                    GTIDebug.Log("Before load of throttleISPFloatCurve", iDebugLevel.DebugInfo);
                    GTIthrottleISPFloatCurve.Load(value);
                }
                else
                {
                    GTIthrottleISPFloatCurve = null;
                }
            }
        }
        public string SetUseGTIthrottleISPCurve
        {
            set
            {
                bool result;
                if (bool.TryParse(value, out result)) useGTIthrottleISPCurve = result; else useGTIthrottleISPCurve = false;
            }
        }

        #region Supporting settings
        //private float _propDensity;
        //private bool _propDensityCalculated = false;
        //private float propDensity
        //{
        //    get
        //    {
        //        //Check if input arrays is symmetric
        //        if (_propellantsArray.Length == _propRatiosArray.Length)
        //        {
        //            _propDensityCalculated = CalcDensity(propellants, _propRatios, propIgnoreForISP);
        //            //Check if the density is calculated
        //            if (_propDensityCalculated)
        //            {
        //                return _propDensity;
        //            }
        //            else
        //            {
        //                return 0;
        //            }

        //        }
        //        else
        //        {
        //            //Failure results in false calculation and returning 0
        //            _propDensityCalculated = false;
        //            return 0;
        //        }

        //    }
        //    set
        //    {
        //        _propDensity = value;
        //    }
        //}
        //private int _propAmount;
        #endregion

        #region Methods
        //private bool CalcDensity(string inPropellants, string inRatios, string inIgnoreForIsp)
        //{
        //    bool returnSuccessStatus = false, useIgnoreForISP = false, IgnoreForISP;
        //    string[] arrInPropellants, arrInRatios, arrIgnoreForIsp;

        //    try
        //    {
        //        arrInPropellants = inPropellants.Trim().Split(',');
        //        arrInRatios = inRatios.Trim().Split(',');

        //        if (arrInPropellants.Length != arrInRatios.Length) { return false; }

        //        //Decide if ignoreForISP property should be used for density calculation
        //        arrIgnoreForIsp = inIgnoreForIsp.Trim().Split(',');
        //        if ((string.IsNullOrEmpty(inIgnoreForIsp) || inIgnoreForIsp.Trim().Length == 0))
        //        { useIgnoreForISP = false; }
        //        else
        //        {
        //            //arrIgnoreForIsp = inIgnoreForIsp.Trim().Split(',');
        //            if (arrIgnoreForIsp.Length == arrInPropellants.Length) { useIgnoreForISP = true; }
        //        }
        //    }
        //    catch
        //    {
        //        //If split fails, return no success
        //        return false;
        //    }

        //    try
        //    {
        //        //GTIDebug.Log("Running _propDensity = fx.calcWeightedDensity(_propellants, _propRatios)");

        //        //Create strings for the calculation
        //        if (useIgnoreForISP)                    //Is IgnoreForISP to be used
        //        {
        //            inPropellants = string.Empty;
        //            inRatios = string.Empty;
        //            //loop the arrays and recreate cleaned arrays
        //            for (int i = 0; i < arrInPropellants.Length; i++)
        //            {
        //                //GTIDebug.Log("if ( !bool.TryParse(arrIgnoreForIsp[i], out IgnoreForISP) || IgnoreForISP == false)");
        //                if (!bool.TryParse(arrIgnoreForIsp[i], out IgnoreForISP) || IgnoreForISP == false)
        //                {
        //                    inPropellants = inPropellants + "," + arrInPropellants[i];
        //                    inRatios = inRatios + "," + arrInRatios[i];
        //                }
        //            }
        //        }

        //        //Calculate the weighted density of the propellants
        //        propDensity = PhysicsUtilities.calcWeightedDensity(inPropellants, inRatios);
        //        if (propDensity > 0) { returnSuccessStatus = true; } else { returnSuccessStatus = false; }

        //        //GTIDebug.Log("_propDensity = fx.calcWeightedDensity(_propellants, _propRatios) is successfull");
        //    }
        //    catch
        //    {
        //        GTIDebug.LogError("Guybrush101.CustomTypes.CalcDensity Failed By Exception");
        //        returnSuccessStatus = false;
        //        //throw;
        //    }

        //    return returnSuccessStatus;
        //}

        //public override string ToString()
        //{
        //    return moduleIndex + "\t" + ID + "\t" + Name;
        //}
        #endregion
    }

    //partModule
    public class GTI_MultiModeEngine : GTI_MultiMode<MultiModeEngine>
    {

        private ConfigNode MultiModeConfigNode = null;

        private ModuleEngines ModuleEngines;
        private bool currentEngineState;

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

        #region infoFields (only for GetInfo())
        [KSPField]
        public string infoatmosphereCurve_Vac = string.Empty;
        [KSPField]
        public string infoatmosphereCurve_Atm = string.Empty;
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

        #region booleans
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

        bool initialUpdate = true;

        [KSPField]
        public string useGTIthrottleISPCurves = string.Empty;
        #endregion

        //Exposes private information
        public ConfigNode GetMultiModeConfigNode => MultiModeConfigNode;
        public List<MultiModeEngine> GetengineModeList => modes;

        private EventData<float, float> onThrottleChangeEvent;


        public override void OnLoad(ConfigNode node)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                GTIDebug.Log("GTI_MultiModeEngine : OnLoad() : " + GTIDebug.GetVesselName(part), iDebugLevel.DebugInfo);

                GTIDebug.Log("Before adding GTI_MultiModeEngine to onThrottleChange Event : " + GTIDebug.GetVesselName(part), iDebugLevel.DebugInfo);
                onThrottleChangeEvent = GameEvents.FindEvent<EventData<float, float>>("onThrottleChange");
                if (onThrottleChangeEvent != null)
                {
                    GTIDebug.Log("Adding GTI_MultiModeEngine to onThrottleChange Event : " + GTIDebug.GetVesselName(part), iDebugLevel.High);
                    onThrottleChangeEvent.Add(onThrottleChange);
                }
                else
                {
                    GTIDebug.Log("GTI_MultiModeEngine failed to be added to onThrottleChange Event : Event was null", iDebugLevel.Low);
                }
            }
        }


        protected override void initializeSettings()
        {
            GTIDebug.Log("GTI_MultiModeEngine : initializeSettings() " + !_settingsInitialized, iDebugLevel.High);
            if (!_settingsInitialized)
            {
                //force disable of moduleAnimationGroup
                useModuleAnimationGroup = false;

                //Load the part module configurations for GTI_MultiModeEngine
                GTIDebug.Log(this.GetType().Name + " - ConfigNode Before Load", iDebugLevel.DebugInfo);
                if (MultiModeConfigNode == null) { MultiModeConfigNode = part.GetPartModuleConfig("MODULE", "name", this.GetType().Name); GTIDebug.Log("\n" + MultiModeConfigNode.ToString(), iDebugLevel.DebugInfo); } else { GTIDebug.Log("Allready loaded\n" + MultiModeConfigNode.ToString(), iDebugLevel.Medium); }
                GTIDebug.Log(this.GetType().Name + " - ConfigNode Loaded", iDebugLevel.DebugInfo);

                #region Identify ModuleEngines in Scope
                //Find module which is to be manipulated - NOTE: Only the first one is being handled. There should not be multiple when using this module
                ModuleEngines = part.FindModuleImplementing<ModuleEngines>();
                #endregion

                #region Parse settings
                //bool boolParseResult;

                //string[] arrGUIengineModeNames, arrPropellantNames, arrPropellantRatios;
                //string[] arrPropDrawGauge, arrPropIgnoreForISP, arrResourceFlowMode;
                //string[] arrMaxThrust, arrHeatProd, arrEngineTypes;
                //string[] arratmChangeFlows;
                //string[] arrUseEngineResponseTime, arrEngineAccelerationSpeed, arrEngineDecelerationSpeed;

                //string[] arruseVelCurves, arruseAtmCurves;
                //string[] arrusethrottleISPCurves;

                //Propellant level
                GUIengineModeNamesEmpty = ArraySplitEvaluate(GUIengineModeNames, out string[] arrGUIengineModeNames, ';');
                PropellantNamesEmpty = ArraySplitEvaluate(propellantNames, out string[] arrPropellantNames, ';');
                PropellantRatiosEmpty = ArraySplitEvaluate(propellantRatios, out string[] arrPropellantRatios, ';');

                PropIgnoreForISPEmpty = ArraySplitEvaluate(propIgnoreForISP, out string[] arrPropIgnoreForISP, ';');
                PropDrawGaugeEmpty = ArraySplitEvaluate(propDrawGauge, out string[] arrPropDrawGauge, ';');
                ResourceFlowModeEmpty = ArraySplitEvaluate(resourceFlowMode, out string[] arrResourceFlowMode, ';');

                MaxThrustEmpty = ArraySplitEvaluate(maxThrust, out string[] arrMaxThrust, ';');

                HeatProdEmpty = ArraySplitEvaluate(heatProduction, out string[] arrHeatProd, ';');
                atmChangeFlowsEmpty = ArraySplitEvaluate(atmChangeFlows, out string[] arratmChangeFlows, ';');

                UseEngineResponseTimeEmpty = ArraySplitEvaluate(useEngineResponseTime, out string[] arrUseEngineResponseTime, ';');
                EngineAccelerationSpeedEmpty = ArraySplitEvaluate(engineAccelerationSpeed, out string[] arrEngineAccelerationSpeed, ';');
                EngineDecelerationSpeedEmpty = ArraySplitEvaluate(engineDecelerationSpeed, out string[] arrEngineDecelerationSpeed, ';');

                EngineTypesEmpty = ArraySplitEvaluate(EngineTypes, out string[] arrEngineTypes, ';');

                useVelCurvesEmpty = ArraySplitEvaluate(useVelCurves, out string[] arruseVelCurves, ';');
                useAtmCurvesEmpty = ArraySplitEvaluate(useAtmCurves, out string[] arruseAtmCurves, ';');

                useGTIthrottleISPCurvesEmpty = ArraySplitEvaluate(useGTIthrottleISPCurves, out string[] arrusethrottleISPCurves, ';');

                //Get FloatCurves from the part
                ConfigNode[] atmosphereCurves = MultiModeConfigNode.GetNodes("atmosphereCurve");
                ConfigNode[] velCurves = MultiModeConfigNode.GetNodes("velCurve");
                ConfigNode[] atmCurves = MultiModeConfigNode.GetNodes("atmCurve");
                ConfigNode[] throttleISPCurves = MultiModeConfigNode.GetNodes("throttleISPCurve");

                if (GTIConfig.DebugLevel == iDebugLevel.DebugInfo)
                {
                    if (atmosphereCurves != null && atmosphereCurves.Length > 0) GTIDebug.Log(atmosphereCurves.ToStringExt(), iDebugLevel.DebugInfo);
                    if (velCurves != null && velCurves.Length > 0) GTIDebug.Log(velCurves.ToStringExt(), iDebugLevel.DebugInfo);
                    if (atmCurves != null && atmCurves.Length > 0) GTIDebug.Log(atmCurves.ToStringExt(), iDebugLevel.DebugInfo);
                    if (throttleISPCurves != null && throttleISPCurves.Length > 0) GTIDebug.Log(throttleISPCurves.ToStringExt(), iDebugLevel.DebugInfo);
                }

                //Test Curves for validity
                if (atmosphereCurves.Length == arrPropellantNames.Length)   { AtmosphereCurveEmpty = false; } else { AtmosphereCurveEmpty = true; }
                if (velCurves.Length == arrPropellantNames.Length)          { VelCurveEmpty = false; } else { VelCurveEmpty = true; }
                if (atmCurves.Length == arrPropellantNames.Length)          { AtmCurveEmpty = false; } else { AtmCurveEmpty = true; }
                if (throttleISPCurves.Length == arrPropellantNames.Length)  { GTIthrottleISPCurvesEmpty = false; } else { GTIthrottleISPCurvesEmpty = true; }
                #endregion

                #region Populate list with settings
                modes = new List<MultiModeEngine>(arrPropellantNames.Length);
                for (int i = 0; i < arrPropellantNames.Length; i++)
                {
                    modes.Add(new MultiModeEngine()
                    {
                        moduleIndex = i,
                        ID = i.ToString(),
                        Name = GUIengineModeNamesEmpty ? arrPropellantNames[i] : arrGUIengineModeNames[i],
                        propellants = arrPropellantNames[i],
                        propRatios = arrPropellantRatios[i],

                        propIgnoreForISP = PropIgnoreForISPEmpty ? string.Empty : arrPropIgnoreForISP[i],       //Has Default value "false"
                        propDrawGauge = PropDrawGaugeEmpty ? string.Empty : arrPropDrawGauge[i],                //Has Default value "true"

                        resourceFlowMode = ResourceFlowModeEmpty ? string.Empty : arrResourceFlowMode[i],
                        SetMaxThrust = MaxThrustEmpty ? "0" : arrMaxThrust[i],

                        heatProduction = HeatProdEmpty ? string.Empty : arrHeatProd[i],
                        atmChangeFlow = atmChangeFlowsEmpty ? string.Empty : arratmChangeFlows[i],
                        useEngineResponseTime = UseEngineResponseTimeEmpty ? string.Empty : arrUseEngineResponseTime[i],
                        engineAccelerationSpeed = EngineAccelerationSpeedEmpty ? string.Empty : arrEngineAccelerationSpeed[i],
                        engineDecelerationSpeed = EngineDecelerationSpeedEmpty ? string.Empty : arrEngineDecelerationSpeed[i],
                        useVelCurve = useVelCurvesEmpty ? string.Empty : arruseVelCurves[i],
                        useAtmCurve = useAtmCurvesEmpty ? string.Empty : arruseAtmCurves[i],

                        SetUseGTIthrottleISPCurve = useGTIthrottleISPCurvesEmpty ? string.Empty : arrusethrottleISPCurves[i],

                        //store FloatCurves
                        atmosphereCurve = AtmosphereCurveEmpty ? null : atmosphereCurves[i],
                        velCurve = VelCurveEmpty ? null : velCurves[i],
                        atmCurve = AtmCurveEmpty ? null : atmCurves[i],
                        GTIthrottleISPCurve = GTIthrottleISPCurvesEmpty ? null : throttleISPCurves[i]
                    });
                }
                #endregion
                GTIDebug.Log("GTI_MultiModeEngine - engineModeList populated", iDebugLevel.DebugInfo);


                GTIDebug.Log("\n" +
                    propellantNames + "\t" + PropellantNamesEmpty + "\n" +
                    propellantRatios + "\t" + PropellantRatiosEmpty + "\n" +
                    propIgnoreForISP + "\t" + PropIgnoreForISPEmpty + "\n" +
                    propDrawGauge + "\t" + PropDrawGaugeEmpty
                    , iDebugLevel.DebugInfo);

                GTIDebug.Log("GTI_MultiModeEngine : initializeSettings() DONE", iDebugLevel.VeryHigh);
            }
        }

        public override void updateMultiMode(bool silentUpdate = false)
        {
            GTIDebug.Log(this.GetType().Name + " -- updateMultiMode(silentUpdate:= " + silentUpdate + ")", iDebugLevel.High);

            ConfigNode newPropNode = new ConfigNode();
            float targetRatio;
            bool targetIgnoreForISP, targetDrawGauge;
            string[] arrtargetPropellants, arrtargetRatios, arrtargetIgnoreForISP, arrtargetDrawGuage, arrtargetResourceFlowMode;

            float floatParseResult;
            bool boolParseResult;
            //currentEngineState = false;

            //Get the Ignition state, i.e. is the engine shutdown or activated
            currentEngineState = ModuleEngines.getIgnitionState;
            //Shutdown the engine --> Removes the gauges, and make sense to do before changing propellant
            if (currentEngineState)
            {
                ModuleEngines.EngineIgnited = false;
            }
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (!initialUpdate)
                    ModuleEngines.Shutdown();
            }


            #region ConfigNode Replace previous propellant node
            //Split cfg subsettings into arrays
            arrtargetPropellants = modes[selectedMode].propellants.Split(',');
            arrtargetRatios = modes[selectedMode].propRatios.Split(',');
            arrtargetIgnoreForISP = modes[selectedMode].propIgnoreForISP.Split(',');
            arrtargetDrawGuage = modes[selectedMode].propDrawGauge.Split(',');
            arrtargetResourceFlowMode = ResourceFlowModeEmpty ? new string[0] : modes[selectedMode].resourceFlowMode.Split(',');


            //Debug.Log("BEFORE for (int i = 0; i < arrtargetPropellants.Length; i++)");
            //Create new propellent nodes by looping them in.
            for (int i = 0; i < arrtargetPropellants.Length; i++)
            {
                //Get and convert ratios to floats. They should already have been verified in the CustomTypes.PropellantList class
                targetRatio = Convert.ToSingle(arrtargetRatios[i]);

                //if ignoreForISP have been set wrong or not at all, then we config it to false
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
            if (modes.Count > 1) ModuleEngines.Load(newPropNode);
            #endregion

            #region Curves
            if (!AtmosphereCurveEmpty) ModuleEngines.atmosphereCurve.Load(modes[selectedMode].atmosphereCurve);
            if (!VelCurveEmpty) ModuleEngines.velCurve.Load(modes[selectedMode].velCurve);
            if (!AtmCurveEmpty) ModuleEngines.atmCurve.Load(modes[selectedMode].atmCurve);

            //Debug.Log("[GTI] ISP Float Curve : " + engineModeList[selectedMode].GetthrottleISPFloatCurve.Evaluate(0.5f).ToString());
            if (!GTIthrottleISPCurvesEmpty) GTIDebug.Log("ISP Float Curve : " + modes[selectedMode].GTIthrottleISPCurve.ToString(), iDebugLevel.DebugInfo); else GTIDebug.Log("no ISP Float Curve : " + GTIDebug.GetVesselName(part), iDebugLevel.DebugInfo);

            #endregion

            //Get maxISP from the atmosphere curve
            //maxISP = KeyFrameGetMaxValue(ModuleEngines.atmosphereCurve.Curve.keys);

            //Set max Thrust and the corresponding fuelflow
            if (!MaxThrustEmpty) { ModuleEngines.maxThrust = modes[selectedMode].maxThrust; }


            //ModuleEngines.maxFuelFlow = ModuleEngines.maxThrust / (ModuleEngines.atmosphereCurve.Evaluate(0f) * ModuleEngines.g);
            ModuleEngines.maxFuelFlow = calcFuelFlow(
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
                onThrottleChange(FlightInputHandler.state.mainThrottle, FlightInputHandler.state.mainThrottle);
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
            if (float.TryParse(modes[selectedMode].heatProduction, out floatParseResult) && !HeatProdEmpty) { ModuleEngines.heatProduction = floatParseResult; }
            if (bool.TryParse(modes[selectedMode].atmChangeFlow, out boolParseResult) && !atmChangeFlowsEmpty) { ModuleEngines.atmChangeFlow = boolParseResult; }

            if (bool.TryParse(modes[selectedMode].useEngineResponseTime, out boolParseResult) && !UseEngineResponseTimeEmpty) { ModuleEngines.useEngineResponseTime = boolParseResult; }
            if (float.TryParse(modes[selectedMode].engineAccelerationSpeed, out floatParseResult) && !EngineAccelerationSpeedEmpty) { ModuleEngines.engineAccelerationSpeed = floatParseResult; }
            if (float.TryParse(modes[selectedMode].engineDecelerationSpeed, out floatParseResult) && !EngineDecelerationSpeedEmpty) { ModuleEngines.engineDecelerationSpeed = floatParseResult; }

            if (bool.TryParse(modes[selectedMode].useVelCurve, out boolParseResult) && !useVelCurvesEmpty) { ModuleEngines.useVelCurve = boolParseResult; }
            if (bool.TryParse(modes[selectedMode].useAtmCurve, out boolParseResult) && !useAtmCurvesEmpty) { ModuleEngines.useAtmCurve = boolParseResult; }

            //Debug.Log("Before engine type");
            #region Set the engine type
            //[LiquidFuel, Nuclear, SolidBooster, Turbine, MonoProp, ScramJet, Electric, Generic, Piston]
            if (!EngineTypesEmpty) { ModuleEngines.engineType = GetEngineType(modes[selectedMode].engineType); }
            #endregion

            if (!silentUpdate) writeScreenMessage();

            //Restart engine if it was on before switching
            //???? if (!initialUpdate)
            if (currentEngineState)
            {
                if (!initialUpdate)
                    ModuleEngines.Activate();
                if (HighLogic.LoadedSceneIsFlight)
                    ModuleEngines.EngineIgnited = true;
            }
            initialUpdate = false;
        }

        protected override void writeScreenMessage()
        {
            //string strOutInfo = string.Empty;
            StringBuilder strOutInfo = new StringBuilder();

            strOutInfo.AppendLine("Engine mode changed to " + modes[selectedMode].Name);
            strOutInfo.AppendLine("Propellants:");
            strOutInfo.AppendLine(modes[selectedMode].propellants);

            GTIDebug.Log("\nGTI_MultiModeEngine:\n" + strOutInfo.ToString(), iDebugLevel.DebugInfo);

            writeScreenMessage(
                Message: strOutInfo.ToString(),
                messagePosition: messagePosition,
                duration: 3f
                );
        }

        protected override void ModuleAnimationGroupEvent_DisableModules()
        {
            throw new NotImplementedException();
        }

        private void onThrottleChange(float newThrottle, float origThrottle)
        {
            //!throttleISPCurvesEmpty && !usethrottleISPCurvesEmpty && bool.Parse(engineModeList[selectedMode].usethrottleISPCurve) && !ModuleEngines.useThrottleIspCurve

            //Criteria: "Active Vessel", "throttleCurve exists", "throttleCurve is to be used" and "stock throttle curve not implemented"
            if (this.part.vessel == FlightGlobals.ActiveVessel && !GTIthrottleISPCurvesEmpty && modes[selectedMode].useGTIthrottleISPCurve && !ModuleEngines.useThrottleIspCurve)
            {
                float newISPfactor = modes[selectedMode].GTIthrottleISPFloatCurve.Evaluate(ModuleEngines.requestedThrottle);
                //float ISPrefactor = newISPfactor / currentISPfactor;
                float time, value, inTangent, outTangent;
                AnimationCurve newCurve = new AnimationCurve();

                GTIDebug.Log("START-------------------------------------------------------------------------------- ", iDebugLevel.DebugInfo);
                GTIDebug.Log("part is in ActiveVessel - event accepted : " + GTIDebug.GetVesselName(this.part) + " : " + this.part.name, iDebugLevel.DebugInfo);
                GTIDebug.Log("FlightInputHandler.state.mainThrottle: " + FlightInputHandler.state.mainThrottle, iDebugLevel.DebugInfo);
                GTIDebug.Log("ModuleEngines.requestedThrottle: " + ModuleEngines.requestedThrottle, iDebugLevel.DebugInfo);
                GTIDebug.Log("newThrottle: " + newThrottle, iDebugLevel.DebugInfo);
                GTIDebug.Log("origThrottle: " + origThrottle, iDebugLevel.DebugInfo);
                GTIDebug.Log("ModuleEngines.realIsp: " + ModuleEngines.realIsp, iDebugLevel.DebugInfo);

                GTIDebug.Log("modes[selectedMode].GetGTIthrottleISPFloatCurve: " + modes[selectedMode].GTIthrottleISPFloatCurve.Evaluate(ModuleEngines.requestedThrottle), iDebugLevel.DebugInfo);


                GTIDebug.Log("newISPfactor:" + newISPfactor, iDebugLevel.DebugInfo);
                //Create new atmosphereCurve (ISP)
                for (int i = 0; i < modes[selectedMode].atmosphereFloatCurve.Curve.keys.Length; i++)
                {
                    time = modes[selectedMode].atmosphereFloatCurve.Curve.keys[i].time;                     //ModuleEngines.atmosphereCurve.Curve.keys[i].time;
                    value = modes[selectedMode].atmosphereFloatCurve.Curve.keys[i].value;
                    inTangent = modes[selectedMode].atmosphereFloatCurve.Curve.keys[i].inTangent;
                    outTangent = modes[selectedMode].atmosphereFloatCurve.Curve.keys[i].outTangent;

                    newCurve.AddKey(new Keyframe(time, value * newISPfactor, inTangent, outTangent));
                }
                for (int i = 0; i < newCurve.keys.Length; i++)
                {
                    GTIDebug.Log("post newCurve.keys[" + i + "] \t.time = " + newCurve.keys[i].time + "\t.value = " + newCurve.keys[i].value, iDebugLevel.DebugInfo);
                }

                //Update the atmosphereCurve
                ModuleEngines.atmosphereCurve.Curve = newCurve;

                //Recalc fuel flow based on updated atmosphereCurve
                ModuleEngines.maxFuelFlow = calcFuelFlow(
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
                if (!this.part.vessel == FlightGlobals.ActiveVessel) GTIDebug.Log("part not in ActiveVessel - event is ignored : " + GTIDebug.GetVesselName(this.part) + " : " + this.part.name, iDebugLevel.DebugInfo);
                if (!GTIthrottleISPCurvesEmpty) GTIDebug.Log("!GTIthrottleISPCurvesEmpty: " + !GTIthrottleISPCurvesEmpty, iDebugLevel.DebugInfo);
                if (modes[selectedMode].useGTIthrottleISPCurve) GTIDebug.Log("engineModeList[selectedMode].useGTIthrottleISPCurve: " + modes[selectedMode].useGTIthrottleISPCurve, iDebugLevel.DebugInfo);
                if (!ModuleEngines.useThrottleIspCurve) GTIDebug.Log("!ModuleEngines.useThrottleIspCurve: " + !ModuleEngines.useThrottleIspCurve, iDebugLevel.DebugInfo);
            }
        }
        protected override void OnDestroy()
        {
            //GTIDebug.Log("GTI_MultiModeEngine destroyed", iDebugLevel.VeryHigh);
            base.OnDestroy();
            if (onThrottleChangeEvent != null)
            {
                GTIDebug.Log("Removing GTI_MultiModeEngine from onThrottleChange Event", iDebugLevel.VeryHigh);
                onThrottleChangeEvent.Remove(onThrottleChange);
            }
        }

        public override string GetInfo()
        {
            StringBuilder Info = new StringBuilder();

            string[] arrGUIengineModeNames;
            string[] arrPropellantNames, arrPropellantRatios;
            string[] arrSubPropellantNames, arrSubPropellantRatios;
            string[] arrPropDrawGauge, arrPropIgnoreForISP, arrResourceFlowMode;
            string[] arrMaxThrust, arrHeatProd, arrEngineTypes;
            string[] arratmChangeFlows;
            string[] arrUseEngineResponseTime, arrEngineAccelerationSpeed, arrEngineDecelerationSpeed;
            string[] arrinfoatmosphereCurve_Vac, arrinfoatmosphereCurve_Atm;
            bool infoatmosphereCurve_VacEmpty, infoatmosphereCurve_AtmEmpty;

            string[] arruseVelCurves, arruseAtmCurves;
            string[] arrusethrottleISPCurves;

            //Propellant level
            GUIengineModeNamesEmpty = ArraySplitEvaluate(GUIengineModeNames, out arrGUIengineModeNames, ';');
            PropellantNamesEmpty = ArraySplitEvaluate(propellantNames, out arrPropellantNames, ';');
            PropellantRatiosEmpty = ArraySplitEvaluate(propellantRatios, out arrPropellantRatios, ';');

            PropIgnoreForISPEmpty = ArraySplitEvaluate(propIgnoreForISP, out arrPropIgnoreForISP, ';');
            PropDrawGaugeEmpty = ArraySplitEvaluate(propDrawGauge, out arrPropDrawGauge, ';');
            ResourceFlowModeEmpty = ArraySplitEvaluate(resourceFlowMode, out arrResourceFlowMode, ';');

            MaxThrustEmpty = ArraySplitEvaluate(maxThrust, out arrMaxThrust, ';');

            HeatProdEmpty = ArraySplitEvaluate(heatProduction, out arrHeatProd, ';');
            atmChangeFlowsEmpty = ArraySplitEvaluate(atmChangeFlows, out arratmChangeFlows, ';');

            UseEngineResponseTimeEmpty = ArraySplitEvaluate(useEngineResponseTime, out arrUseEngineResponseTime, ';');
            EngineAccelerationSpeedEmpty = ArraySplitEvaluate(engineAccelerationSpeed, out arrEngineAccelerationSpeed, ';');
            EngineDecelerationSpeedEmpty = ArraySplitEvaluate(engineDecelerationSpeed, out arrEngineDecelerationSpeed, ';');

            EngineTypesEmpty = ArraySplitEvaluate(EngineTypes, out arrEngineTypes, ';');

            useVelCurvesEmpty = ArraySplitEvaluate(useVelCurves, out arruseVelCurves, ';');
            useAtmCurvesEmpty = ArraySplitEvaluate(useAtmCurves, out arruseAtmCurves, ';');

            useGTIthrottleISPCurvesEmpty = ArraySplitEvaluate(useGTIthrottleISPCurves, out arrusethrottleISPCurves, ';');

            infoatmosphereCurve_VacEmpty = ArraySplitEvaluate(infoatmosphereCurve_Vac, out arrinfoatmosphereCurve_Vac, ';');
            infoatmosphereCurve_AtmEmpty = ArraySplitEvaluate(infoatmosphereCurve_Atm, out arrinfoatmosphereCurve_Atm, ';');

            //Build string
            Info.AppendLine("<color=yellow># Engine Modes Available: " + arrPropellantNames.Length + "</color>");
            for (int i = 0; i < arrPropellantNames.Length; i++)
            {
                Info.Append("<b><color=yellow>Engine Mode: </color></b>");
                //if (!GUIengineModeNamesEmpty) Info.Append("<b><color=yellow>Name: </color></b>" + arrGUIengineModeNames[i]);
                Info.Append(GUIengineModeNamesEmpty ? i.ToString() : arrGUIengineModeNames[i]);
                Info.AppendLine(); Info.AppendLine();

                //Propellants
                arrSubPropellantNames = arrPropellantNames[i].Split(',');
                arrSubPropellantRatios = arrPropellantRatios[i].Split(',');

                Info.AppendLine("<color=yellow>Propellants: </color>");
                for (int j = 0; j < arrSubPropellantNames.Length; j++)
                {
                    Info.Append(arrSubPropellantNames[j]);
                    Info.Append(" (");
                    Info.Append(arrSubPropellantRatios[j]);
                    Info.Append(") ");
                    if (j < arrSubPropellantNames.Length - 1) Info.AppendLine();
                }
                Info.AppendLine();

                Info.Append("Max thrust: ");
                Info.Append(arrMaxThrust[i]);
                Info.AppendLine();

                //Write ISP informations based on passive fields in the module
                if (!infoatmosphereCurve_VacEmpty)
                {
                    Info.Append("Engine ISP (Vac.): ");
                    Info.Append(arrinfoatmosphereCurve_Vac[i]);
                    Info.AppendLine();
                }
                if (!infoatmosphereCurve_AtmEmpty)
                {
                    Info.Append("Engine ISP (Atm.): ");
                    Info.Append(arrinfoatmosphereCurve_Atm[i]);
                    Info.AppendLine();
                }

                Info.AppendLine();
            }

            Info.AppendLine("\nIn Flight switching is <color=yellow>" + (availableInFlight ? "available" : "not available") + "</color>");

            //str.AppendFormat("Maximal force: {0:0.0}iN\n", maxGeneratorForce);
            //str.AppendFormat("Maximal charge time: {0:0.0}s\n\n", maxChargeTime);
            //str.AppendFormat("Requires\n");
            //str.AppendFormat("- Electric charge: {0:0.00}/s\n\n", requiredElectricalCharge);
            //str.Append("Navigational computer\n");
            //str.Append("- Required force\n");
            //str.Append("- Success probability\n");

            //return "GTI Multi Mode Engine";
            return Info.ToString();
        }
    }
}
