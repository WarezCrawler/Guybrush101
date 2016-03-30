﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Guybrush101.GenericFunctions
{
    class EngineCalculations
    {
        private float _Thrust, _Density, _ISP, _fuelRate, _fuelFlow, _weightedDensity;
        private float _gravity = 9.81f;


        //Equations:
        //Thrust = (fuel rate in L/s) * density * g * Isp
        //density = Thrust / ((fuel rate in L/s) * g * Isp)			, g = 9.81
        //Isp =  Thrust / (g * (fuel rate in L/s) * density)
        //(fuel rate in L/s) = Thrust / (g * Isp * density)			, density = (Mfuel/Vfuel) = [kg/L]

        public float calcThrust(float fuelRate, float Density, float ISP)   // Thrust = L/s * kg/L * m/s^2 *s = kg * m/s^2 = N
        {
            _Thrust = fuelRate * Density * _gravity * ISP;
            return _Thrust;
        }

        public float calcTrustFromfuelFlow(float fuelFlow, float ISP)       // Thrust = Kg/s * m/s^2 * s = Kg * m/s^2 = N
        {
            _Thrust = fuelFlow * _gravity * ISP;
            return _Thrust;
        }

        public float calcISP(float Thrust, float fuelRate, float Density)   //ISP = N / (L/s * kg/L * m/s^2) = (kg * m/s^2)/(L/s * kg/L * m/s^2) = s
        {
            _ISP = Thrust/(fuelRate * Density * _gravity);
            return _ISP;
        }

        public float calcFuelRate(float Thrust, float Density, float ISP)   //fuelRate = kg * m/s^2 / (kg/L * m/s^2 * s) = L/s
        {
            _fuelRate = Thrust / (Density * _gravity * ISP);
            return _fuelRate;
        }

        public float calcFuelFlow(float Thrust, float Density, float ISP)   //fuelFlow = kg * m/s^2 / (m/s^2 * s) = kg/s
        {
            _fuelFlow = Thrust / (_gravity * ISP);                              //fuelFlow = Thrust / (Gravity * ISP)
            return _fuelFlow;
        }

        public float calcFuelRateFromfuelFlow(float fuelFlow, float Density)    //fuelFlow = Kg/s   //FuelRate = L/s    //Density = kg/L
        {
            _fuelRate = fuelFlow / Density;
            return _fuelRate;
        }

        public float calcfuelFlowFromFuelRate(float fuelRate, float Density)    //fuelFlow = Kg/s   //FuelRate = L/s    //Density = kg/L
        {
            _fuelFlow = fuelRate * Density;
            return _fuelRate;
        }

        public float calcDensity(float Thrust, float fuelRate, float ISP)   //Density = kg * m/s^2 / (L/s * m/s^2 * s) = kg/s
        {
            _Density = Thrust / (fuelRate * _gravity * ISP);
            return _Density;
        }
        public float calcWeightedDensity(string Resources, string Ratios)
        {
            string[] arrResources, arrRatios;
            float weightTotal;

            Debug.Log("calcWeightedDensity: split arrays");
            //Parse strings into arrays
            arrResources = Resources.Trim().Split(',');
            arrRatios = Ratios.Trim().Split(',');

            Debug.Log("calcWeightedDensity: Consistency Check");
            //Check consistency of arrays
            if (arrResources.Length != arrRatios.Length)
            {
                Debug.Log("calcWeightedDensity: Return no Density");
                return 0;
            }

            
            try
            {
                Debug.Log("calcWeightedDensity: Sum ratios");
                weightTotal = 0;
                //Calculate total weight for deflation of ratios
                foreach (string ratio in arrRatios)
                {
                    weightTotal = weightTotal + Single.Parse(ratio);    //Ratios should be convertable to floats
                }

                _weightedDensity = 0;
                //Calculate the weighted Density
                for (int i = 0; i<arrResources.Length; i++)
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

        public float gravity        //  m/s^s
        {
            get
            { return _gravity; }
            set
            {
                if (value <= 0)
                {
                    _gravity = 9.82f;
                }
                _gravity = value;
            }
        }
    }

    class HelperFunctions
    {


        //Function for creating correctly formatted KeyFrames from specifically formatted strings
        public Keyframe[] KeyFrameFromString(string inKeys, Keyframe[] iniKeys)
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

                if (keyCurvePoints.Length == 2)
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
    }
}
