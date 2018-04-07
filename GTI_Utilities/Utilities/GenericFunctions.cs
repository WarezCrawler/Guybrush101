using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Collections.Generic;
using UnityEngine;
using static GTI.GTIConfig;

namespace GTI
{
    public static class PhysicsUtilities
    {
        //private static float _Thrust, _Density, _ISP, _fuelRate, _fuelFlow, _weightedDensity;
        private static float _gravity = 9.80665f;                                                      //source: http://wiki.kerbalspaceprogram.com/wiki/1.2


        //Equations:
        //Thrust = (fuel rate in L/s) * density * g * Isp
        //density = Thrust / ((fuel rate in L/s) * g * Isp)			, g = 9.81
        //Isp =  Thrust / (g * (fuel rate in L/s) * density)
        //(fuel rate in L/s) = Thrust / (g * Isp * density)			, density = (Mfuel/Vfuel) = [kg/L]

        /// <summary>
        /// Thrust = (fuel rate in L/s) * density * g * Isp
        ///  --> Thrust = L/s * kg/L * m/s^2 *s = kg * m/s^2 = N
        /// </summary>
        /// <param name="fuelRate"></param>
        /// <param name="Density"></param>
        /// <param name="ISP"></param>
        /// <returns></returns>
        public static float calcThrustFromFuelRate(float fuelRate, float Density, float ISP)   // Thrust = L/s * kg/L * m/s^2 *s = kg * m/s^2 = N
        {
            float _Thrust = fuelRate * Density * _gravity * ISP;
            return _Thrust;
        }

        /// <summary>
        /// Thrust = fuelFlow * g * ISP
        ///  --> Thrust = Kg/s * m/s^2 * s = Kg * m/s^2 = N
        /// </summary>
        /// <param name="fuelFlow"></param>
        /// <param name="ISP"></param>
        /// <returns></returns>
        public static float calcTrustFromfuelFlow(float fuelFlow, float ISP)       // Thrust = Kg/s * m/s^2 * s = Kg * m/s^2 = N
        {
            float _Thrust = fuelFlow * _gravity * ISP;
            return _Thrust;
        }

        /// <summary>
        /// Isp =  Thrust / (g * (fuel rate in L/s) * density)
        ///  --> ISP = N / (L/s * kg/L * m/s^2) = (kg * m/s^2)/(L/s * kg/L * m/s^2) = s
        /// </summary>
        /// <param name="Thrust"></param>
        /// <param name="fuelRate"></param>
        /// <param name="Density"></param>
        /// <returns></returns>
        public static float calcISP(float Thrust, float fuelRate, float Density)   //ISP = N / (L/s * kg/L * m/s^2) = (kg * m/s^2)/(L/s * kg/L * m/s^2) = s
        {
            float _ISP = Thrust/(fuelRate * Density * _gravity);
            return _ISP;
        }

        /// <summary>
        /// (fuel rate in L/s) = Thrust / (g * Isp * density)			, density = (Mfuel/Vfuel) = [kg/L]
        ///  --> fuelRate = kg * m/s^2 / (kg/L * m/s^2 * s) = L/s
        /// </summary>
        /// <param name="Thrust"></param>
        /// <param name="Density"></param>
        /// <param name="ISP"></param>
        /// <returns></returns>
        public static float calcFuelRate(float Thrust, float Density, float ISP)   //fuelRate = kg * m/s^2 / (kg/L * m/s^2 * s) = L/s
        {
            float _fuelRate = Thrust / (Density * _gravity * ISP);
            return _fuelRate;
        }

        //engine.maxThrust / (engine.atmosphereCurve.Evaluate(0f) * engine.g)
        /// <summary>
        /// fuelFlow = Thrust / (Gravity * ISP)
        ///  --> fuelFlow = kg * m/s^2 / (m/s^2 * s) = kg/s
        ///  --> engine.maxThrust / (engine.atmosphereCurve.Evaluate(0f) * engine.g)
        /// </summary>
        /// <param name="Thrust"></param>
        /// <param name="Density"></param>
        /// <param name="ISP"></param>
        /// <returns></returns>
        public static float calcFuelFlow(float Thrust, float ISP)   //fuelFlow = kg * m/s^2 / (m/s^2 * s) = kg/s
        {
            float _fuelFlow = Thrust / (_gravity * ISP);                              //fuelFlow = Thrust / (Gravity * ISP)
            return _fuelFlow;
        }
        public static float calcFuelFlow(float Thrust, float ISP, float Gravity)   //fuelFlow = kg * m/s^2 / (m/s^2 * s) = kg/s
        {
            float _fuelFlow = Thrust / (Gravity * ISP);                              //fuelFlow = Thrust / (Gravity * ISP)
            return _fuelFlow;
        }

        /// <summary>
        /// fuelRate = fuelFlow / Density
        /// --> fuelFlow = Kg/s -- FuelRate = L/s -- Density = kg/L
        /// </summary>
        /// <param name="fuelFlow">fuelFlow = Kg/s</param>
        /// <param name="Density">Density = kg/L</param>
        /// <returns></returns>
        public static float calcFuelRateFromFuelFlow(float fuelFlow, float Density)    //fuelFlow = Kg/s   //FuelRate = L/s    //Density = kg/L
        {
            float _fuelRate = fuelFlow / Density;
            return _fuelRate;
        }

        /// <summary>
        /// fuelFlow = fuelRate * Density
        /// --> fuelFlow = Kg/s -- FuelRate = L/s -- Density = kg/L
        /// </summary>
        /// <param name="fuelRate"></param>
        /// <param name="Density"></param>
        /// <returns></returns>
        public static float calcFuelFlowFromFuelRate(float fuelRate, float Density)    //fuelFlow = Kg/s   //FuelRate = L/s    //Density = kg/L
        {
            float _fuelFlow = fuelRate * Density;
            return _fuelFlow;
        }

        /// <summary>
        /// Density = Thrust / (fuelRate * _gravity * ISP)
        /// --> Density = kg * m/s^2 / (L/s * m/s^2 * s) = kg/s
        /// </summary>
        /// <param name="Thrust"></param>
        /// <param name="fuelRate"></param>
        /// <param name="ISP"></param>
        /// <returns></returns>
        public static float calcDensity(float Thrust, float fuelRate, float ISP)   //Density = kg * m/s^2 / (L/s * m/s^2 * s) = kg/s
        {
            float _Density = Thrust / (fuelRate * _gravity * ISP);
            return _Density;
        }
        /// <summary>
        /// Density = kg/L
        /// </summary>
        /// <param name="Resources">resource name</param>
        /// <param name="Ratios">resource ratio</param>
        /// <returns></returns>
        public static float calcWeightedDensity(string Resources, string Ratios)
        {
            string[] arrResources, arrRatios;
            float weightTotal, _weightedDensity;

            //Debug.Log("calcWeightedDensity: split arrays");
            //Parse strings into arrays
            arrResources = Resources.Trim().Split(',');
            arrRatios = Ratios.Trim().Split(',');

            //Debug.Log("calcWeightedDensity: Consistency Check");
            //Check consistency of arrays
            if (arrResources.Length != arrRatios.Length)
            {
                Debug.Log("calcWeightedDensity: Return no Density");
                return 0;
            }

            try
            {
                //Debug.Log("calcWeightedDensity: Sum ratios");
                weightTotal = 0;
                //Calculate total weight for deflation of ratios that does not sum to 1
                foreach (string ratio in arrRatios)
                {
                    weightTotal = weightTotal + Single.Parse(ratio);    //Ratios should be convertable to floats
                }

                _weightedDensity = 0;
                //Calculate the weighted Density
                for (int i = 0; i < arrResources.Length; i++)
                {
                    if(weightTotal == 0)
                    {
                        _weightedDensity = 0; 
                    }
                    else
                    { 
                        _weightedDensity = _weightedDensity + PartResourceLibrary.Instance.GetDefinition(arrResources[i]).density * Single.Parse(arrRatios[i])/weightTotal;
                    }
                }
            }
            catch
            {
                _weightedDensity = 0;
            }

            return _weightedDensity;
        }

        /// <summary>
        /// Used to override the default gravity constant of 9.80665. If set below 0 the this returns the default.
        /// Therefore it is easy enough to reset to default by setting "gravity = -1".
        /// </summary>
        public static float gravity        //  m/s^s
        {
            get
            { return _gravity; }
            set
            {
                if (value <= 0)
                {
                    _gravity = 9.80665f;                                                      //source: http://wiki.kerbalspaceprogram.com/wiki/1.2
                }
                _gravity = value;
            }
        }
    }

    public static partial class Utilities
    {
        /// <summary>
        /// Checks if a specific mod/plugin is loaded and return true/false
        /// </summary>
        /// <param name="ModName"></param>
        /// <returns></returns>
        public static bool PluginExists(string ModName)
        {
            //return AssemblyLoader.loadedAssemblies.Any(a => a.name == ModName);
            //foreach (var assembly in AssemblyLoader.loadedAssemblies)

            for (int i = 0; i < AssemblyLoader.loadedAssemblies.Count; i++)
            {
                //var name = assembly.assembly.ToString().Split(',')[0];
                //if (AssemblyLoader.loadedAssemblies[i].name == ModName)
                //GTIDebug.Log("ModName (split)" + AssemblyLoader.loadedAssemblies[i].assembly.ToString(),iDebugLevel.DebugInfo);
                //GTIDebug.Log("ModName " + AssemblyLoader.loadedAssemblies[i].name, iDebugLevel.DebugInfo);
                if (AssemblyLoader.loadedAssemblies[i].name == ModName)
                {
                    GTIDebug.Log("ModName '" + AssemblyLoader.loadedAssemblies[i].name + "' found", iDebugLevel.DebugInfo);
                    return true;
                }
                    
            }
            return false;
        }
        public static AssemblyLoader.LoadedAssembyList PluginsListOf()
        {
            return AssemblyLoader.loadedAssemblies;
        }

        /// <summary>
        /// Retrieves the resource Definition from the PartResourceLibrary.Instance.GetDefinition using the name of the resource
        /// </summary>
        /// <param name="ResourceName"></param>
        /// <returns>Resource Definition</returns>
        public static PartResourceDefinition GetResourceDefinition(string ResourceName)
        {
            return PartResourceLibrary.Instance.GetDefinition(ResourceName);
        }

        /// <summary>
        /// Retrieves the resource ID from the PartResourceLibrary.Instance.GetDefinition using the name of the resource
        /// </summary>
        /// <param name="ResourceName"></param>
        /// <returns>Resource ID (integer)</returns>
        public static int GetResourceID(string ResourceName)
        {
            return PartResourceLibrary.Instance.GetDefinition(ResourceName).id;
        }

        /// <summary>
        /// Function for creating correctly formatted KeyFrames from specifically formatted strings. Format of inKey is "0 0 0 0;1 1 1 1;2 2 2 2" -- (float time, float value, float inTangent, float outTangent)
        /// </summary>
        /// <param name="inKeys"></param>
        /// <param name="iniKeys"></param>
        /// <returns></returns>
        public static Keyframe[] KeyFrameFromString(string inKeys, Keyframe[] iniKeys)
        {
            //Format of inKey is
            // "0 0 0 0;1 1 1 1;2 2 2 2"
            // Where "white space" delimits individual parameters for the float curve (float time, float value, float inTangent, float outTangent)
            // Where ";" delimits the keys in the curve (no spaces around ";" is allowed, since these are used as parameter delimiter. No trimming possible because of same fact)
            // All keys in the string have to belong together

            Keyframe[] outKeys;
            int keyCount;
            string[] keyArray, keyCurvePoints;

            keyArray = inKeys.Split(';');
            keyCount = keyArray.Length;

            outKeys = new Keyframe[keyCount];

            //Debug.Log("inKeys: " + inKeys);
            //Debug.Log("keyCount: " + keyCount);
            //Assign values to KeyFrames
            for (int i = 0; i < keyCount; i++)      //keyCount = keyArray.Length;
            {
                keyCurvePoints = keyArray[i].Split(new char[0]);   //split by white space

                //Debug.Log("keyCurvePoint.Length: " + keyCurvePoint.Length);
                //Debug.Log("keyArray[i]: " + keyArray[i]);

                if (keyCurvePoints.Length != 4)
                { outKeys[i] = new Keyframe(Single.Parse(keyCurvePoints[0]), Single.Parse(keyCurvePoints[1])); }
                else if (keyCurvePoints.Length == 4)
                { outKeys[i] = new Keyframe(Single.Parse(keyCurvePoints[0]), Single.Parse(keyCurvePoints[1]), Single.Parse(keyCurvePoints[2]), Single.Parse(keyCurvePoints[3])); }
                else
                {
                    Debug.LogError(
                        "KeyFrameFromString: KeyCurve has wrong dimensions\n" + keyCurvePoints.Length +
                        "\ninKeys: " + inKeys);
                    return iniKeys;     //If iniconsistencies are found, return the initial value
                }
            }
            return outKeys;
        }
        /// <summary>
        /// Write a KeyFrame to the same format as used in CFG files. Use the heading if debugging KeyFrames
        /// </summary>
        /// <param name="inKeyFrameKeys"></param>
        /// <param name="Heading"></param>
        /// <returns></returns>
        public static string KeyFrameGetToCFG(Keyframe[] inKeyFrameKeys, string Heading = "")
        {
            System.Text.StringBuilder BuildString = new System.Text.StringBuilder();

            BuildString.AppendLine("");
            //Set heading if present
            if (!StringEvaluate(Heading, out Heading))
            {
                BuildString.AppendLine(Heading);
            }

            //Write keys to string
            foreach (Keyframe keys in inKeyFrameKeys)
            {
                BuildString.AppendLine("key = " + keys.time.ToString("0.#########") + " " + keys.value.ToString("0.#########") + " " + keys.inTangent.ToString("0.#########") + " " + keys.outTangent.ToString("0.#########"));
            }

            return BuildString.ToString();
        }
        /// <summary>
        /// Get the maximum value from a KeyFrame object. 
        /// TIME VALUE INTANGENT OUTTANGENT
        /// </summary>
        /// <param name="inKeyFrameKeys"></param>
        /// <returns></returns>
        public static float KeyFrameGetMaxValue(Keyframe[] inKeyFrameKeys)
        {
            float MaxValue = 0;

            foreach (Keyframe key in inKeyFrameKeys)
            {
                //Debug.Log("inKeyFrameKeys: " + key.time + " " + key.value + " " + key.inTangent + " " + key.outTangent);
                if (MaxValue < key.value) { MaxValue = key.value; }
            }

            return MaxValue;
        }



        /// <summary>
        /// Return if the input is empty and an output array if non-empty
        /// </summary>
        /// <param name="_input"></param>
        /// <param name="_outputArray"></param>
        /// <param name="_separator"></param>
        /// <returns></returns>
        public static bool ArraySplitEvaluate(string _input, out string[] _outputArray, char _separator)
        {
            bool isEmpty;

            //Check if the input is an empty or null string
            isEmpty = ((string.IsNullOrEmpty(_input) || _input.Trim().Length == 0));

            //If not empty or null string, split the array based on the specified separator
            if (!isEmpty) { _outputArray = _input.Trim().Split(_separator); }
            else
            {
                //Since it is required we return an array with a single empty string if there was no string
                _outputArray = new string[] { string.Empty };
            }
            //Return of the input is an empty or null string
            return isEmpty;
        }
        /// <summary>
        /// Evaluates if a string is nothing, empty, null
        /// </summary>
        /// <param name="_input"></param>
        /// <param name="_output"></param>
        /// <returns></returns>
        public static bool StringEvaluate(string _input, out string _output)
        {
            bool isEmpty;

            //Check if the input is an empty or null string
            isEmpty = ((string.IsNullOrEmpty(_input) || _input.Trim().Length == 0));

            //If not empty or null string, split the array based on the specified separator
            if (!isEmpty) { _output = _input; }
            else
            {
                //Since it is required we return an array with a single empty string if there was no string
                _output = string.Empty;
            }
            //Return of the input is an empty or null string
            return isEmpty;
        }

        public static EngineType GetEngineType(string engineType)
        {
            switch (engineType)
            {
                case "LiquidFuel":
                    return EngineType.LiquidFuel;
                case "Nuclear":
                    return EngineType.Nuclear;
                case "SolidBooster":
                    return EngineType.SolidBooster;
                case "Turbine":
                    return EngineType.Turbine;
                case "MonoProp":
                    return EngineType.MonoProp;
                case "ScramJet":
                    return EngineType.ScramJet;
                case "Electric":
                    return EngineType.Electric;
                case "Generic":
                    return EngineType.Generic;
                case "Piston":
                    return EngineType.Piston;
                default:
                    //Do nothing
                    return EngineType.LiquidFuel;
            }
        }

        public static void HookModule(string targetModule, string attachModule)
        {
            for (int iPart = 0; iPart < PartLoader.LoadedPartsList.Count; iPart++)
            {
                AvailablePart currentAP = PartLoader.LoadedPartsList[iPart];
                Part currentPart = currentAP.partPrefab;

                for (int iModule = 0; iModule < currentPart.Modules.Count; iModule++)
                {
                    if (targetModule == currentPart.Modules[iModule].moduleName)
                    {
                        if (!ModuleAttached(currentPart, attachModule))
                        {
                            GTIDebug.Log(targetModule + " found - Attaching " + attachModule, iDebugLevel.Medium);
                            PartModule newModule = currentPart.AddModule(attachModule);
                            if (null == newModule)
                            {
                                Debug.LogError(attachModule + " attachment failed.");
                            }
                            newModule.moduleName = attachModule;
                        }
                        break;
                    }
                }
            }
        }

        private static bool ModuleAttached(Part part, string moduleName)
        {
            for (int iModules = 0; iModules < part.Modules.Count; iModules++)
            {
                if (moduleName == part.Modules[iModules].moduleName)
                {
                    return (true);
                }
            }
            return (false);
        }
    }

    public static class Extentions
    {
        /// <summary>
        /// Convert an array of ConfigNodes to a string, as opposed to stock just writing "ConfigNode[]".
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static string ToStringExt(this ConfigNode[] nodes)
        {
            StringBuilder strConfigNodes = new StringBuilder();
            foreach (ConfigNode node in nodes)
            {
                strConfigNodes.AppendLine(node.ToString());
            }

            return strConfigNodes.ToString();
        }
    }

}
