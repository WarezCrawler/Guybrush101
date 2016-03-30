using System;
using System.Collections.Generic;
using UnityEngine;
using Guybrush101.GenericFunctions;

//####################################################################################################################################
// Credits:
// Thanks to FSfuelswitch and InterstellarFuelSwitch for inspiration
// Thanks to NathanKell for invaluable advise on KSP forums
//####################################################################################################################################



namespace Guybrush101
{
    class EngineClassSwitch : PartModule
    {
        //Engine manipulation
        #region KSPFields and supporting settings
        [KSPField]
        public string engineID = string.Empty;                                                         //the engine to affect. Needed if multiple engines are present in the part.
        [KSPField]
        public string engineNames = string.Empty;                                                      //Names of the engine after switch, if empty, then not name switching occurs
        [KSPField]
        public string propellantNames = "LiquidFuel,Oxidizer;MonoPropellant";           //the list of propellant setups available to the switch.
        [KSPField]
        public string propellantRatios = "45,55;100";                                   //the propellant ratios to set. NOTE: It is the actual fuel flow that defines the thrust => fuel usage.
        [KSPField]
        public string maxThrust = string.Empty;
        [KSPField(isPersistant = true)]
        public int selectedPropellant = -1;                                             //holds the selected propellant setup.
        [KSPField]
        public string atmosphereCurveKeys = string.Empty;
        [KSPField]
        public string velCurveKeys = string.Empty; //"0 0 0 0;1 1 1 1";                 //White space for parameters, ";" for keys and "|" for each setup linked to a propellant setup. Provide keys for all propellants or none, else wierd thing will happen
        [KSPField]
        public string atmCurveKeys = string.Empty; //"0 0 0 0;1 1 0 0";
                                                   //(float time, float value, float inTangent, float outTangent)
        private bool atmosphereCurveEmpty = true;
        private bool velCurveEmpty = true;
        private bool atmCurveEmpty = true;
        
        //GUI fields for information
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Propellants")]
        private string GUIpropellantNames = String.Empty;
        [KSPField]
        public string iniGUIpropellantNames = string.Empty;

        //Availability of the functionality
        [KSPField]
        public bool availableInFlight = false;
        [KSPField]
        public bool availableInEditor = true;

        #endregion

        #region Arrays and Lists
        //Arrays for data processing
        private string[] arrPropellantNames;    //for the split list of prop names
        private string[] arrPropellantRatios;   //for the split list of prop ratios
        private string[] arrMaxThrust;
        private string[] arrAtmosphereCurve, arrPropellantVelCurve, arrPropellantAtmCurve;

        //Customtype for list of relevant settings
        private List<CustomTypes.PropellantList> propList = new List<CustomTypes.PropellantList>();
        
        //For the engines modules
        private List<ModuleEngines> ModuleEngines;
        #endregion

        #region Other class level declarations
        private bool _settingsInitialized = false;

        //private Part cPart;      //current part       //irrelevant here -> could be relevant in supporting class modules
        //public ConfigNode ECSnode;

        #endregion

        //####### Beginning of logics
        #region Events
        public override void OnStart(PartModule.StartState state)
        {
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
                //try
                //{
                #region GUI Update
                var nextEvent = Events["nextPropellantEvent"];
                    nextEvent.guiActive = availableInFlight;
                    nextEvent.guiActiveEditor = availableInEditor;
                    //nextEvent.guiName = nextTankSetupText;

                var previousEvent = Events["previousPropellantEvent"];
                    previousEvent.guiActive = availableInFlight;
                    previousEvent.guiActiveEditor = availableInEditor;
                //previousEvent.guiName = previousTankSetupText;
                #endregion

                #region Parse Arrays
                //Parse the strings into arrays of information
                arrPropellantNames = propellantNames.Trim().Split(';');        //arrPropellantNames should now have the propellant names and combinations
                arrPropellantRatios = propellantRatios.Trim().Split(';');      //arrPropellantRatios should now have the propellant ratios and combinations
                arrMaxThrust = maxThrust.Trim().Split(';');

                atmosphereCurveEmpty = ((string.IsNullOrEmpty(atmosphereCurveKeys) || atmosphereCurveKeys.Trim().Length == 0));
                velCurveEmpty = ((string.IsNullOrEmpty(velCurveKeys) || velCurveKeys.Trim().Length == 0));
                atmCurveEmpty = ((string.IsNullOrEmpty(atmCurveKeys) || velCurveKeys.Trim().Length == 0));
                
                if (!atmosphereCurveEmpty)
                {
                    //Debug.Log("arrAtmosphereCurve --> splitting the array based on\n" + atmosphereCurveKeys);
                    arrAtmosphereCurve = atmosphereCurveKeys.Split('|');
                }
                if (!velCurveEmpty)
                {
                    //Debug.Log("arrPropellantVelCurve --> splitting the array based on\n" + velCurveKeys);
                    arrPropellantVelCurve = velCurveKeys.Split('|');
                }
                if (!atmCurveEmpty)
                {
                    //Debug.Log("arrPropellantAtmCurve --> splitting the array based on\n" + atmCurveKeys);
                    arrPropellantAtmCurve = atmCurveKeys.Split('|');
                }

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
                //Populate the Propellant List (propList) from the array for more intuitive access to this information later
                for (int i = 0; i < arrPropellantRatios.Length; i++)
                    {
                        propList.Add(new CustomTypes.PropellantList()
                        {
                            Propellants = arrPropellantNames[i],
                            PropRatios  = arrPropellantRatios[i],
                            setMaxThrust = arrMaxThrust[i]
                        });
                        if (!atmosphereCurveEmpty)
                        {
                            propList[i].atmosphereCurve = arrAtmosphereCurve[i];
                        }
                        if (!velCurveEmpty)
                        {
                            propList[i].velCurve = arrPropellantVelCurve[i];
                        }
                        if (!atmCurveEmpty)
                        {
                            propList[i].atmCurve = arrPropellantAtmCurve[i];
                        }
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
                            //What to do if the engine is in scope???!!!
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

                //set settings to initialized
                _settingsInitialized = true;
                //}
                //catch
                //{
                //    Debug.LogError("EngineClassSwitch: Error on InitializeSettings()");
                //}
            }
        }
        #endregion

        #region User_Interface
        //START - Events for selection of propellants
        //NEXT
        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Next propellant setup")]
        public void nextPropellantEvent()
        {
            //InitializeSettings();
            selectedPropellant++;
            if (selectedPropellant > arrPropellantNames.GetUpperBound(0))
            {
                //if we move from last propellant, then the next one is the first one - aka 0
                selectedPropellant = 0;
            }
            updateEngineModule(true);
        }
      //PREVIOUS
        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Previous propellant setup")]
        public void previousPropellantEvent()
        {
            //InitializeSettings();
            selectedPropellant--;
            if (selectedPropellant < 0)
            {
                //if we move from the first propellant, then the previous is the last - aka the upperbound
                selectedPropellant = arrPropellantNames.GetUpperBound(0);
            }
            updateEngineModule(true);
        }
        //END - Events for selection of propellants
        #endregion

        #region UpdatePart Engine Module
        private void updateEngineModule(bool calledByPlayer, string callingFunction = "player")
        {
            //string[] currentResource;
            string[] targetPropellants;
            string[] targetRatios;
            float targetRatio;

            //int i = 0; //Integer for looping etc.
            float maxISP = 0;
            //float Density = 1;

            ConfigNode newPropNode = new ConfigNode();
            HelperFunctions MiscFx = new HelperFunctions();
            EngineCalculations EngineCalc = new EngineCalculations();
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

                //Debug.Log(
                //    "selectedPropellant: "  + selectedPropellant +
                //    "\nPropellants: "       + propList[selectedPropellant].Propellants +
                //    "\nPropRatios: "        + propList[selectedPropellant].PropRatios +
                //    "\nCalledby: "          + callingFunction
                //    );


                targetPropellants = propList[selectedPropellant].Propellants.Split(',');
                targetRatios = propList[selectedPropellant].PropRatios.Split(',');
                
                //Create new propellent nodes by looping them in.
                for (int i = 0; i < targetPropellants.Length; i++)
                {
                    //Get and convert ratios to floats. They should already have been verified in the CustomTypes.PropellantList class
                    targetRatio = Convert.ToSingle(targetRatios[i]);

                    ConfigNode propNode = newPropNode.AddNode("PROPELLANT");
                    propNode.AddValue("name", targetPropellants[i]);
                    propNode.AddValue("ratio", targetRatio);
                    propNode.AddValue("ignoreForIsp", false);       //For now we assume all is counted for ISP
                    propNode.AddValue("DrawGauge", true);      //I think the gauge  should always be shown
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

                //Density = propList[selectedPropellant].propDensity;
                foreach (Keyframe key in moduleEngine.atmosphereCurve.Curve.keys)
                {
                    //Debug.Log("atmosphereKey[" + i + "]: " + key.time + " " + key.value + " " + key.inTangent + " " + key.outTangent);
                    if ( maxISP < key.value ) { maxISP = key.value; }
                    //i++;
                }
                //Set max Thrust and the corresponding fuelflow
                moduleEngine.maxThrust = propList[selectedPropellant].maxThrust;
                moduleEngine.maxFuelFlow = EngineCalc.calcFuelFlow(
                    Thrust: propList[selectedPropellant].maxThrust, 
                    Density: propList[selectedPropellant].propDensity, 
                    ISP: maxISP
                    );

                //moduleEngine.maxThrust = 0;



                //Debug.Log("End of curve editing");

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


                //Restart engine if it was on before switching
                //Debug.Log("engine restart");
                if (engineState == true) { moduleEngine.Activate(); }
                //Debug.Log("engine restarted");

                //ConfigNode newPropNode = new ConfigNode();
                //ConfigNode propNode = newPropNode.AddNode("PROPELLANT");
                //propNode.AddValue("name", "LiquidFuel");
                //propNode.AddValue("ratio", 0.9f);
                //propNode = newPropNode.AddNode("PROPELLANT");
                //propNode.AddValue("name", "Oxidizer");
                //propNode.AddValue("ratio", 0.9f);
            }
        }
        #endregion

        #region VAB Information
        public override string GetInfo()
        {
            try
            {
                //we need to run the InitializeSettings here, because the OnStart does not run before this.
                InitializeSettings();

                //string strOutInfo = string.Empty;
                System.Text.StringBuilder strOutInfo = new System.Text.StringBuilder();
                //string[] _propellants, _propratios;

                strOutInfo.AppendLine("Propellants available");
                foreach (CustomTypes.PropellantList item in propList)
                {
                    strOutInfo.AppendLine(item.Propellants.Replace(",",", "));
                }


                    //Debug.Log(
                    //    "selectedPropellant: " + selectedPropellant +
                    //    "\nPropellants: " + propList[selectedPropellant].Propellants +
                    //    "\nPropRatios: " + propList[selectedPropellant].PropRatios +
                    //    "\nCalledby: " + "GetInfo"
                    //    );

                    //foreach (CustomTypes.PropellantList item in propList)
                    //{
                    //    _propellants = item.Propellants.Split(',');
                    //    _propratios = item.PropRatios.Split(',');
                    //    if (_propellants.Length > 1)
                    //    {
                    //        for (int i = 0; i < _propellants.Length; i++)
                    //        {
                    //            strOutInfo += _propellants[i];
                    //            strOutInfo += " (" + _propratios[i] + ")";
                    //            if (i < _propellants.Length - 1) { strOutInfo += ","; }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        strOutInfo += _propellants;
                    //    }
                    //    strOutInfo += "; ";
                    //}
                    //strOutInfo = "Propellant configurations:\n" + strOutInfo.Substring(0, strOutInfo.Length - 1);

                    return strOutInfo.ToString();
            }
            catch (Exception e)
            {
                Debug.LogError("EngineClassSwitch GetInfo Error " + e.Message);
                throw;
            }
        }
        #endregion
        
        ////Function for creating correctly formatted KeyFrames from specifically formatted strings
        //private Keyframe[] KeyFrameFromString(string inKeys, Keyframe[] iniKeys)
        //{
        //    //Format of inKey is
        //    // "0 0 0 0;1 1 1 1;2 2 2 2"
        //    // Where "white space" delimits individual parameters for the float curve (float time, float value, float inTangent, float outTangent)
        //    // Where ";" delimits the keys in the curve (no spaces around ";" is allowed, since these are used as parameter delimiter. No trimming possible because of same fact)
        //    // All keys in the string have to belong together

        //    Keyframe[] outKeys;
        //    int keyCount;
        //    string[] keyArray, keyCurvePoints;

        //    keyArray = inKeys.Split(';');
        //    keyCount = keyArray.Length;

        //    outKeys = new Keyframe[keyCount];

        //    //Debug.Log("inKeys: " + inKeys);
        //    //Debug.Log("keyCount: " + keyCount);
        //    //Assign values to KeyFrames
        //    for (int i = 0; i < keyCount; i++)      //keyCount = keyArray.Length;
        //    {
        //        keyCurvePoints = keyArray[i].Split(new char[0]);   //split by white space

        //        //Debug.Log("keyCurvePoint.Length: " + keyCurvePoint.Length);
        //        //Debug.Log("keyArray[i]: " + keyArray[i]);

        //        if (keyCurvePoints.Length == 2)
        //        { outKeys[i] = new Keyframe(Single.Parse(keyCurvePoints[0]), Single.Parse(keyCurvePoints[1])); }
        //        else if (keyCurvePoints.Length == 4)
        //        { outKeys[i] = new Keyframe(Single.Parse(keyCurvePoints[0]), Single.Parse(keyCurvePoints[1]), Single.Parse(keyCurvePoints[2]), Single.Parse(keyCurvePoints[3])); }
        //        else
        //        {
        //            Debug.LogError(
        //                "KeyFrameFromString: KeyCurve has wrong dimensions\n" + keyCurvePoints.Length +
        //                "\ninKeys: " + inKeys);
        //            return iniKeys;     //If iniconsistencies are found, return the initial value
        //        }
        //    }
        //    return outKeys;
        //}

        ////gets the id of the named resource found in a part
        //private static int GetResourceID(this Part part, string resourceName)
        //{
        //    PartResourceDefinition resource = PartResourceLibrary.Instance.GetDefinition(resourceName);
        //    return resource.id;
        //}

        #region --------------------------------TESTING---------------------------------------
        [KSPEvent(active = true, guiActive = true, guiActiveEditor = true, guiName = "DEBUG")]
        public void test()
        {
            InitializeSettings();
            EngineCalculations Calc = new EngineCalculations();
            float Density = propList[selectedPropellant].propDensity;




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
                
                for (int j = 0; j < moduleEngine.atmosphereCurve.Curve.length; j++)
                {
                    Debug.Log(moduleEngine.atmosphereCurve.Curve[j].time + ", " + moduleEngine.atmosphereCurve.Curve[j].value);
                }

                //DEBUG
                int i = 0;
                float CurveTimeValue = 0;
                foreach (Keyframe key in moduleEngine.atmosphereCurve.Curve.keys)
                {
                    Debug.Log("atmosphereKey[" + i + "]: " + key.time + " " + key.value + " " + key.inTangent + " " + key.outTangent);
                    if (CurveTimeValue < key.value) { CurveTimeValue = key.value;  }
                    i++;
                }
                Debug.Log(
                    "ISP: " + CurveTimeValue +
                    "\nmaxFuelRate should be: " + Calc.calcFuelFlow(moduleEngine.maxThrust, Density, CurveTimeValue)
                );
                i = 0;
                foreach (Keyframe key in moduleEngine.atmCurve.Curve.keys)
                {
                    Debug.Log("atmKey[" + i + "]: " + key.time + " " + key.value + " " + key.inTangent + " " + key.outTangent);
                    i++;
                }
                i = 0;
                foreach (Keyframe key in moduleEngine.velCurve.Curve.keys)
                {
                    Debug.Log("velKey[" + i + "]: " + key.time + " " + key.value + " " + key.inTangent + " " + key.outTangent);
                    i++;
                }
            }
        }
        #endregion

    } //class EngineClassSwitch : PartModule 
} //namespace Guybrush101
