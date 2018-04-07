//using System;
//using System.Collections;
//using static GTI.GTIConfig;

//namespace GTI.CustomTypes
//{
//    public class engineMultiModeList : CollectionBase
//    {
//        //private string _names;            //New name of part in event of new propellant
//        private string _propellants = string.Empty;        //The propellants
//        private int    _propAmount;
//        private string _propRatios = string.Empty;         //The propellant ratios
//        private float  _propDensity;
//        private string _ignoreForIsp = string.Empty;
//        private string _drawGauge = string.Empty;

//        private string _heatProd = string.Empty;
//        private string _engineType = string.Empty;


//        //private string _atmosphereCurve = string.Empty;
//        //private string _velCurve = string.Empty;
//        //private string _atmCurve = string.Empty;


//        private ConfigNode _atmosphereCurve = new ConfigNode();
//        private FloatCurve atmosphereFloatCurve = new FloatCurve();
//        public ConfigNode velCurve = new ConfigNode();
//        public ConfigNode atmCurve = new ConfigNode();
//        //public ConfigNode throttleISPCurve = new ConfigNode();
//        private ConfigNode _GTIthrottleISPCurve = new ConfigNode();
//        private FloatCurve GTIthrottleISPFloatCurve = new FloatCurve();


//        private string _atmChangeFlow = string.Empty;
//        private string _useVelCurve = string.Empty;
//        private string _useAtmCurve = string.Empty;
//        //private bool _usethrottleISPCurve = false;
//        public bool useGTIthrottleISPCurve { get; set; } = false;
//        //public string usethrottleISPCurve { get; set; } = "false";

//        public string useEngineResponseTime;
//        public string engineAccelerationSpeed;
//        public string engineDecelerationSpeed;

//        private string _requiredTech = string.Empty;
//        public bool engineAvailable = false;
//        private string _GUIengineModeNames = string.Empty;
//        private string _resourceFlowMode = string.Empty;

//        //private ConfigNode _MultiModeConfigNode = new ConfigNode();

//        //arrPropIgnoreForISP, arrPropDrawGauge, arrHeatProd, arrEngineTypes;

//        //find ISP Turbine = 1 atm, LiquidFuel = 0 atm and use the function for FuelFlow to ensure right flow rate

//        private float _maxThrust;           //  Kg * m/s^2 = N
//                                            //private float _maxFuelFlow;         //  Kg/s

//        public FloatCurve GetGTIthrottleISPFloatCurve
//        {
//            get { return GTIthrottleISPFloatCurve; }
//        }

//        public ConfigNode GTIthrottleISPCurve
//        {
//            get { return _GTIthrottleISPCurve; }
//            set
//            {
//                _GTIthrottleISPCurve = value;

//                if (value != null)
//                {
//                    GTIDebug.Log("Before load of throttleISPFloatCurve", iDebugLevel.DebugInfo);
//                    GTIthrottleISPFloatCurve.Load(value);
//                }
//            }
//        }

//        public string SetUseGTIthrottleISPCurve
//        {
//            set
//            {
//                bool result;
//                if (bool.TryParse(value, out result)) useGTIthrottleISPCurve = result; else useGTIthrottleISPCurve = false;
//            }
//        }

//        public FloatCurve GetatmosphereFloatCurve
//        {
//            get { return atmosphereFloatCurve; }
//        }
//        public ConfigNode atmosphereCurve
//        {
//            get { return _atmosphereCurve; }
//            set
//            {
//                _atmosphereCurve = value;

//                if (value != null)
//                {
//                    GTIDebug.Log("Before load of atmosphereFloatCurve", iDebugLevel.DebugInfo);
//                    atmosphereFloatCurve.Load(value);
//                }
//            }
//        }


//        //For storing and retrieving propellants
//        public string requiredTech
//        {
//            get { return _requiredTech; }
//            set
//            {
//                //_engineConfigAvailable = ResearchAndDevelopment.GetTechnologyState(value) == RDTech.State.Available;
//                _requiredTech = value;
//            }
//        }

//        public string GUIengineModeNames
//        {
//            get { return _GUIengineModeNames; }
//            set { _GUIengineModeNames = value; }
//        }




//        public string Propellants
//        {
//            get { return _propellants; }
//            set { _propellants = value; CalcDensity(_propellants, _propRatios, _ignoreForIsp); }
//        }

//        //For storing and retrieving propellant ratios
//        public string propRatios
//        {
//            get { return _propRatios; }
//            set
//            {
//                string[] arrInString;
//                bool booparse;

//                _propRatios = value;
                
//                //Evaluate if multi propellants are in the string, put result to _propAmount
//                arrInString = value.Split(',');
//                _propAmount = arrInString.Length;
//                try
//                {
//                    foreach(string item in arrInString)
//                    {
//                        float numvalue;
//                        booparse = Single.TryParse(item, out numvalue);
//                        if (!booparse)
//                                GTIDebug.LogWarning("CustomTypes.PropellantList -> Could not parse propellant ratio " + item + " into integer.", iDebugLevel.Low);
//                    }
//                }
//                catch (Exception e) { GTIDebug.LogError("CustomTypes.PropellantList -> Could not parse propellant ratio into integer.\n" + value + "\nError trown:\n" + e);throw e; }
//                CalcDensity(_propellants,_propRatios, _ignoreForIsp);
//            }
//        }
//        public float propDensity
//        {
//            get { return _propDensity; }
//        }
//        private bool CalcDensity(string inPropellants, string inRatios, string inIgnoreForIsp)
//        {
//            bool returnSuccessStatus = false, useIgnoreForISP = false, IgnoreForISP;
//            string[] arrInPropellants, arrInRatios, arrIgnoreForIsp;

//            try
//            {
//                arrInPropellants = inPropellants.Trim().Split(',');
//                arrInRatios = inRatios.Trim().Split(',');

//                if (arrInPropellants.Length != arrInRatios.Length) { return false; }

//                //Decide if ignoreForISP property should be used for density calculation
//                arrIgnoreForIsp = inIgnoreForIsp.Trim().Split(',');
//                if ((string.IsNullOrEmpty(inIgnoreForIsp) || inIgnoreForIsp.Trim().Length == 0))
//                { useIgnoreForISP = false; } 
//                else
//                { 
//                    //arrIgnoreForIsp = inIgnoreForIsp.Trim().Split(',');
//                    if ( arrIgnoreForIsp.Length == arrInPropellants.Length ) { useIgnoreForISP = true; }
//                }
//            }
//            catch
//            {
//                //If split fails, return no success
//                return false;
//            }

//            try
//            { 
//                //PhysicsUtilities fx = new PhysicsUtilities();
//                //GTIDebug.Log("Running _propDensity = fx.calcWeightedDensity(_propellants, _propRatios)");

//                //Create strings for the calculation
//                if (useIgnoreForISP)                    //Is IgnoreForISP to be used
//                { 
//                    inPropellants = string.Empty;
//                    inRatios = string.Empty;
//                    //loop the arrays and recreate cleaned arrays
//                    for (int i = 0; i < arrInPropellants.Length; i++)
//                    {
//                        //GTIDebug.Log("if ( !bool.TryParse(arrIgnoreForIsp[i], out IgnoreForISP) || IgnoreForISP == false)");
//                        if ( !bool.TryParse(arrIgnoreForIsp[i], out IgnoreForISP) || IgnoreForISP == false)
//                        {
//                            inPropellants = inPropellants + "," + arrInPropellants[i];
//                            inRatios = inRatios + "," + arrInRatios[i];
//                        }
//                    }
//                }

//                //Calculate the weighted density of the propellants
//                _propDensity = PhysicsUtilities.calcWeightedDensity(inPropellants, inRatios);
//                if ( _propDensity > 0 ) { returnSuccessStatus = true; } else { returnSuccessStatus = false; }

//                //GTIDebug.Log("_propDensity = fx.calcWeightedDensity(_propellants, _propRatios) is successfull");
//            }
//            catch
//            {
//                GTIDebug.LogError("Guybrush101.CustomTypes.CalcDensity Failed By Exception");
//                returnSuccessStatus = false;
//                //throw;
//            }

//            return returnSuccessStatus;
//        }
//        //public void setMaxThrust(string inMaxThrust)
//        //{
//        //    float outMaxThrust = 0;
//        //    Single.TryParse(inMaxThrust, out outMaxThrust);
//        //    _maxThrust = outMaxThrust;
//        //}
//        public string setMaxThrust
//        {
//            set
//            {
//                float outMaxThrust = 0;
//                Single.TryParse(value, out outMaxThrust);
//                _maxThrust = outMaxThrust;
//            }
//        }
//        public float maxThrust
//        {
//            get { return _maxThrust; }
//        }
//        public string propIgnoreForISP
//        {
//            get { return _ignoreForIsp; }
//            set { _ignoreForIsp = value; }
//        }
//        public string propDrawGauge
//        {
//            get { return _drawGauge; }
//            set { _drawGauge = value; }
//        }
//        public string heatProduction
//        {
//            get { return _heatProd; }
//            set { _heatProd = value; }
//        }
//        public string engineType
//        {
//            get { return _engineType; }
//            set { _engineType = value; }
//        }
//        //NEW 9/4-2016
//        public string atmChangeFlow
//        {
//            get { return _atmChangeFlow; }
//            set { _atmChangeFlow = value; }
//        }
//        public string useVelCurve
//        {
//            get { return _useVelCurve; }
//            set { _useVelCurve = value; }
//        }
//        public string useAtmCurve
//        {
//            get { return _useAtmCurve; }
//            set { _useAtmCurve = value; }
//        }

//        public int propAmount
//        {
//            get { return _propAmount; }
//            //set { _propAmount = value; }  //Internally calculated
//        }

//        public string resourceFlowMode
//        {
//            get { return null; }
//            set
//            {
//                switch (value)
//                {
//                    case "ALL_VESSEL":
//                        _resourceFlowMode = value;
//                        break;
//                    case "ALL_VESSEL_BALANCE":
//                        _resourceFlowMode = value;
//                        break;
//                    case "NO_FLOW":
//                        _resourceFlowMode = value;
//                        break;
//                    case "NULL":
//                        _resourceFlowMode = value;
//                        break;
//                    case "STACK_PRIORITY_SEARCH":
//                        _resourceFlowMode = value;
//                        break;
//                    case "STAGE_PRIORITY_FLOW":
//                        _resourceFlowMode = value;
//                        break;
//                    case "STAGE_PRIORITY_FLOW_BALANCE":
//                        _resourceFlowMode = value;
//                        break;
//                    case "STAGE_STACK_FLOW":
//                        _resourceFlowMode = value;
//                        break;
//                    case "STAGE_STACK_FLOW_BALANCE":
//                        _resourceFlowMode = value;
//                        break;
//                    default:
//                        _resourceFlowMode = string.Empty;
//                        break;
//                }
//            }
//        }




//        //public string atmosphereCurve
//        //{
//        //    get { return _atmosphereCurve; }
//        //    set { _atmosphereCurve = value; }
//        //}
//        //public string velCurve
//        //{
//        //    get { return _velCurve; }
//        //    set { _velCurve = value; }
//        //}
//        //public string atmCurve
//        //{
//        //    get { return _atmCurve; }
//        //    set { _atmCurve = value; }
//        //}
//        //public int Length
//        //{
//        //    get
//        //    {
//        //        //foreach (string item in propList)
//        //        //return 0;
//        //    }
//        //}
        
//    }
//}
