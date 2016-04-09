using System;
using System.Collections.Generic;
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
        //Engine manipulation
        #region KSPFields and supporting settings
        [KSPField]
        public string engineID = string.Empty;                                                         //the engine to affect. Needed if multiple engines are present in the part.
        //[KSPField]
        //public string engineNames = string.Empty;                                                      //Names of the engine after switch, if empty, then not name switching occurs
        [KSPField]
        public string propellantIgnoreForIsp = string.Empty;
        [KSPField]
        public string propellantDrawGauge = string.Empty;
        [KSPField]
        public string heatProduction = string.Empty;
        [KSPField]
        public string EngineTypes = string.Empty;
        [KSPField]
        public string propellantNames = "LiquidFuel,Oxidizer;MonoPropellant";           //the list of propellant setups available to the switch.
        [KSPField]
        public string propellantRatios = "45,55;100";                                   //the propellant ratios to set. NOTE: It is the actual fuel flow that defines the thrust => fuel usage.
        [KSPField]
        public string maxThrust = string.Empty;

        [KSPField]
        public string atmosphereCurveKeys = string.Empty;
        [KSPField]
        public string atmChangeFlows = string.Empty;
        [KSPField]
        public string velCurveKeys = string.Empty; //"0 0 0 0;1 1 1 1";                 //White space for parameters, ";" for keys and "|" for each setup linked to a propellant setup. Provide keys for all propellants or none, else wierd thing will happen
        [KSPField]
        public string useVelCurves = string.Empty;
        [KSPField]
        public string atmCurveKeys = string.Empty; //"0 0 0 0;1 1 0 0";
                                                   //(float time, float value, float inTangent, float outTangent)
        [KSPField]
        public string useAtmCurves = string.Empty;

        private bool MaxThrustEmpty = true;
        private bool EngineTypesEmpty = true;

        private bool heatProductionEmpty = true;

        private bool propellantIgnoreForIspEmpty = true;
        private bool propellantDrawGaugeEmpty = true;

        private bool atmosphereCurveEmpty = true;
        private bool velCurveEmpty = true;
        private bool atmCurveEmpty = true;

        private bool atmChangeFlowsEmpty = true;
        private bool useVelCurvesEmpty = true;
        private bool useAtmCurvesEmpty = true;

        #endregion

        #region Arrays and Lists
        //Arrays for data processing
        private string[] arrPropellantNames;    //for the split list of prop names
        private string[] arrPropellantRatios;   //for the split list of prop ratios
        private string[] arrMaxThrust, arrPropDrawGauge, arrHeatProd, arrEngineTypes, arrPropIgnoreForISP, arratmChangeFlows, arruseVelCurves, arruseAtmCurves;
        private string[] arrAtmosphereCurve, arrPropellantVelCurve, arrPropellantAtmCurve;

        //Customtype for list of relevant settings
        private List<CustomTypes.PropellantList> propList = new List<CustomTypes.PropellantList>();
        
        //For the engines modules
        private List<ModuleEngines> ModuleEngines;
        #endregion

        #region Other class level declarations
        private bool _settingsInitialized = false;

        #endregion

        //####### Beginning of logics
        #region Events
        public override void OnStart(PartModule.StartState state)
        {
            Debug.Log("Tech start: " + ResearchAndDevelopment.GetTechnologyState("start").ToString());
            Debug.Log("Tech basicRocketry: " + ResearchAndDevelopment.GetTechnologyState("basicRocketry").ToString());
            Debug.Log("Tech basicRocketry: " + ResearchAndDevelopment.GetTechnologyState("basicRocketry"));


                

            //Debug.Log("EngineClassSwitch --> OnStart");
            //try
            //{
            //Debug.Log("OnStart --> InitializeSettings");
            InitializeSettings();
                if (selectedPropellant == -1)
                {
                    selectedPropellant = 0;
                    //assignResourcesToPart(false);
                }
                //Debug.Log("OnStart --> assignPropToPart");
                updateEngineModule(false, "OnStart");
            //}
            //catch
            //{
            //    Debug.LogError("EngineClassSwitch: Error on OnStart()");
            //}
        }
        #endregion

        #region Settings
        public void InitializeSettings()
        {
            //Debug.Log("EngineClassSwitch --> InitializeSettings");
            if (!_settingsInitialized)
            {
                Utilities Util = new Utilities();

                #region Parse Arrays
                //Parse the strings into arrays of information
                //Propellant names and ratios are mandatory information

                //Propellant level
                arrPropellantNames = propellantNames.Trim().Split(';');        //arrPropellantNames should now have the propellant names and combinations
                arrPropellantRatios = propellantRatios.Trim().Split(';');      //arrPropellantRatios should now have the propellant ratios and combinations
                propellantIgnoreForIspEmpty = Util.ArraySplitEvaluate(propellantIgnoreForIsp, out arrPropIgnoreForISP, ';');
                propellantDrawGaugeEmpty = Util.ArraySplitEvaluate(propellantDrawGauge, out arrPropDrawGauge, ';');


                //Engine level
                MaxThrustEmpty              = Util.ArraySplitEvaluate(maxThrust, out arrMaxThrust, ';');
                EngineTypesEmpty            = Util.ArraySplitEvaluate(EngineTypes, out arrEngineTypes, ';');
                heatProductionEmpty         = Util.ArraySplitEvaluate(heatProduction, out arrHeatProd, ';');
                atmChangeFlowsEmpty         = Util.ArraySplitEvaluate(atmChangeFlows, out arratmChangeFlows, ';');
                useVelCurvesEmpty           = Util.ArraySplitEvaluate(useVelCurves, out arruseVelCurves, ';');
                useAtmCurvesEmpty           = Util.ArraySplitEvaluate(useAtmCurves, out arruseAtmCurves, ';');

                //Engine level curves
                atmosphereCurveEmpty        = Util.ArraySplitEvaluate(atmosphereCurveKeys, out arrAtmosphereCurve,'|');
                velCurveEmpty               = Util.ArraySplitEvaluate(velCurveKeys, out arrPropellantVelCurve, '|');
                atmCurveEmpty               = Util.ArraySplitEvaluate(atmCurveKeys, out arrPropellantAtmCurve, '|');

                //Test if the two arrays are compatible                 <------ This test should be extended!
                if (arrPropellantNames.Length != arrPropellantRatios.Length)
                {
                    Debug.LogError("EngineClassSwitch: Error on InitializeSettings() - \nPropellant names (" + arrPropellantNames.Length + "pcs) and ratios (" + arrPropellantRatios.Length + "pcs) does not match\nConfig file error");
                }
                if (arrPropellantNames.Length != arrMaxThrust.Length)
                {
                    Debug.LogError("EngineClassSwitch: Error on InitializeSettings() - \nPropellant names (" + arrPropellantNames.Length + "pcs) and maxThrusts (" + arrMaxThrust.Length + "pcs) does not match\nConfig file error");
                }
                #endregion

                #region Populate Propellant List
                try { 
                //Populate the Propellant List (propList) from the array for more intuitive access to this information later
                for (int i = 0; i < arrPropellantRatios.Length; i++)
                    {
                        propList.Add(new CustomTypes.PropellantList()
                        {
                            Propellants = arrPropellantNames[i],
                            PropRatios  = arrPropellantRatios[i]
                            //setMaxThrust = arrMaxThrust[i],
                            //propIgnoreForISP = arrPropIgnoreForISP[i],
                            //propDrawGauge = arrPropDrawGauge[i],
                            //heatProduction = arrHeatProd[i],
                            //engineType = arrEngineTypes[i]
                        });


                        //Propellant level --> Propellant level is needed as the entire node is replaced.
                        propList[i].propIgnoreForISP    = propellantIgnoreForIspEmpty   ? "false"   : arrPropIgnoreForISP[i];       //Has Default value "false"
                        propList[i].propDrawGauge       = propellantDrawGaugeEmpty      ? "true"    : arrPropDrawGauge[i];          //Has Default value "true"

                        //Engine level
                        propList[i].setMaxThrust        = MaxThrustEmpty                ? "ignore"  : arrMaxThrust[i];       //No default - Ignore when updating
                        propList[i].engineType          = EngineTypesEmpty              ? ""        : arrEngineTypes[i];     //No default - Ignore when updating
                        propList[i].heatProduction      = heatProductionEmpty           ? "0"       : arrHeatProd[i];        //No default - Ignore when updating

                        propList[i].atmChangeFlow = atmChangeFlowsEmpty                 ? "true"    : arratmChangeFlows[i];
                        propList[i].useVelCurve = useVelCurvesEmpty                     ? "true"    : arruseVelCurves[i];
                        propList[i].useAtmCurve = useAtmCurvesEmpty                     ? "true"    : arruseAtmCurves[i];

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
                    }
                }
                catch(Exception e)
                {
                    Debug.LogError("Populate Propellant List failed");
                    throw e;
                }
                #endregion

                

                #region Identify ModuleEngines in Scope
                //Find modules which is to be manipulated
                ModuleEngines = part.FindModulesImplementing<ModuleEngines>();
                    var toBeRemoved = new List<ModuleEngines>();                      //for removal of irrelevant engine modules
                    foreach (var moduleEngine in ModuleEngines)
                    {
                        if (moduleEngine.engineID == engineID || string.IsNullOrEmpty(engineID) || engineID.Trim().Length == 0)       //"string.IsNullOrEmpty(engineID) || engineID.Trim().Length==0" is used instead of IsNullOrWhiteSpace()
                        { 
                            //do nothing 
                        }
                        else
                        {
                            toBeRemoved.Add(moduleEngine);
                        }
                    }
                    foreach (var remove in toBeRemoved)
                    {
                        ModuleEngines.Remove(remove);
                    }
                //Now ModulesEngines should have exactly the engine modules in scope
                #endregion

                #region GUI Update
                Debug.Log("--> initializeGUI()");
                initializeGUI();
                Debug.Log("--> initializeGUI() is Done");
                #endregion

                //set settings to initialized
                _settingsInitialized = true;
            }
        }
        #endregion



        #region UpdatePart Engine Module
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
            //PartResourceDefinitionList resourceID;

            foreach (var moduleEngine in this.ModuleEngines)
            {
                bool engineState = false;
                
                //Get the Ignition state, i.e. is the engine shutdown or activated
                engineState = moduleEngine.getIgnitionState;
                //Shutdown the engine --> Removes the gauges, and make sense to do before changing propellant
                //Debug.Log("engine shutdown");
                moduleEngine.Shutdown();
                //Debug.Log("engine shutdown completed");

                //Split cfg subsettings into arrays
                arrtargetPropellants = propList[selectedPropellant].Propellants.Split(',');
                arrtargetRatios = propList[selectedPropellant].PropRatios.Split(',');
                arrtargetIgnoreForISP = propList[selectedPropellant].propIgnoreForISP.Split(',');
                arrtargetDrawGuage = propList[selectedPropellant].propDrawGauge.Split(',');

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

                //Set max Thrust and the corresponding fuelflow
                if (!MaxThrustEmpty)        { moduleEngine.maxThrust        = propList[selectedPropellant].maxThrust; }

                moduleEngine.maxFuelFlow = EngineCalc.calcFuelFlow(
                    Thrust: moduleEngine.maxThrust,                             //Thrust: propList[selectedPropellant].maxThrust, 
                    Density: propList[selectedPropellant].propDensity, 
                    ISP: maxISP
                    );

                if (Single.TryParse(propList[selectedPropellant].heatProduction, out floatParseResult) && !heatProductionEmpty)
                { moduleEngine.heatProduction = floatParseResult; }

                //Set the engine type
                //[LiquidFuel, Nuclear, SolidBooster, Turbine, MonoProp, ScramJet, Electric, Generic, Piston]
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

                //Write the propellant setup to the right click GUI
                if ( iniGUIpropellantNames == string.Empty )
                {
                    //Default naming if no user defined names are found
                    GUIpropellantNames = "[" + selectedPropellant + "] " + propList[selectedPropellant].Propellants.Replace(",", ", ");
                }
                else
                {
                    //User defined names
                    GUIpropellantNames = iniGUIpropellantNames.Trim().Split(';')[selectedPropellant];
                }
                //chooseField.guiName = GUIpropellantNames;

                //Restart engine if it was on before switching
                if (engineState == true) { moduleEngine.Activate(); }
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
        
        #region --------------------------------TESTING---------------------------------------
        [KSPEvent(active = true, guiActive = true, guiActiveEditor = true, guiName = "DEBUG")]
        public void DEBUG_ENGINESSWITCH()
        {
            InitializeSettings();
            PhysicsUtilities Calc = new PhysicsUtilities();
            Utilities MiscFx = new Utilities();
            System.Text.StringBuilder BuildString = new System.Text.StringBuilder();
            float Density = propList[selectedPropellant].propDensity;
            //int i = 0;

            foreach (var moduleEngine in ModuleEngines)
            {

                //Debug.Log("CGF velCurveKeys: " + velCurveKeys);
                //Debug.Log("CGF atmCurveKeys: " + atmCurveKeys);
                Debug.Log(
                    "requestedThrottle: " + moduleEngine.requestedThrottle * 100 + "%" +
                    "\nmaxThrust: " + moduleEngine.maxThrust +
                    "\nresultingThrust: " + moduleEngine.resultingThrust +
                    "\nmaxFuelFlow: " + moduleEngine.maxFuelFlow +
                    "\nrequestedMassFlow: " + moduleEngine.requestedMassFlow +
                    "\nmaxFuelRate (calc): " + Calc.calcFuelRateFromfuelFlow(moduleEngine.maxFuelFlow, Density) +
                    "\nFuelRate (calc): " + Calc.calcFuelRateFromfuelFlow(moduleEngine.requestedMassFlow, Density) +
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
            }
            
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
                BuildString.Length = 0;
                BuildString.Capacity = 16;
            }
        }
        #endregion


    } //class EngineClassSwitch : PartModule 
} //namespace Guybrush101
