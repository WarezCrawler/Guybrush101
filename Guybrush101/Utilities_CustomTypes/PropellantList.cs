﻿using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI.CustomTypes
{
    public class PropellantList
    {
        //private string _names;            //New name of part in event of new propellant
        private string _propellants;        //The propellants
        private int _propAmount;
        private string _propRatios;         //The propellant ratios
        private float _propDensity;
        private string _ignoreForIsp = string.Empty;
        private string _drawGauge;

        private string _heatProd;
        private string _engineType;


        private string _atmosphereCurve;
        private string _velCurve;
        private string _atmCurve;

        //arrPropIgnoreForISP, arrPropDrawGauge, arrHeatProd, arrEngineTypes;

        //find ISP Turbine = 1 atm, LiquidFuel = 0 atm and use the function for FuelFlow to ensure right flow rate

        private float _maxThrust;           //  Kg * m/s^2 = N
        //private float _maxFuelFlow;         //  Kg/s

        //For storing and retrieving propellants
        public string Propellants
        {
            get { return _propellants; }
            set { _propellants = value; CalcDensity(_propellants, _propRatios, _ignoreForIsp); }
        }

        //For storing and retrieving propellant ratios
        public string PropRatios
        {
            get { return _propRatios; }
            set
            {
                string[] arrInString;
                bool booparse;

                _propRatios = value;
                
                //Evaluate if multi propellants are in the string, put result to _propAmount
                arrInString = value.Split(',');
                _propAmount = arrInString.Length;
                try
                {
                    foreach(string item in arrInString)
                    {
                        float numvalue;
                        booparse = Single.TryParse(item, out numvalue);
                        if (!booparse)
                                Debug.LogWarning("CustomTypes.PropellantList -> Could not parse propellant ratio " + item + " into integer.");
                    }
                }
                catch (Exception e) { Debug.LogError("CustomTypes.PropellantList -> Could not parse propellant ratio into integer.\n" + value + "\nError trown:\n" + e); }
                CalcDensity(_propellants,_propRatios, _ignoreForIsp);
            }
        }
        public float propDensity
        {
            get { return _propDensity; }
        }
        private bool CalcDensity(string inPropellants, string inRatios, string inIgnoreForIsp)
        {
            bool returnSuccessStatus = false, useIgnoreForISP = false, IgnoreForISP;
            string[] arrInPropellants, arrInRatios, arrIgnoreForIsp;

            try
            {
                arrInPropellants = inPropellants.Trim().Split(',');
                arrInRatios = inRatios.Trim().Split(',');

                if (arrInPropellants.Length != arrInRatios.Length) { return false; }

                //Decide if ignoreForISP property should be used for density calculation
                arrIgnoreForIsp = inIgnoreForIsp.Trim().Split(',');
                if ((string.IsNullOrEmpty(inIgnoreForIsp) || inIgnoreForIsp.Trim().Length == 0))
                { useIgnoreForISP = false; } 
                else
                { 
                    //arrIgnoreForIsp = inIgnoreForIsp.Trim().Split(',');
                    if ( arrIgnoreForIsp.Length == arrInPropellants.Length ) { useIgnoreForISP = true; }
                }
            }
            catch
            {
                //If split fails, return no success
                return false;
            }

            try
            { 
                PhysicsUtilities fx = new PhysicsUtilities();
                //Debug.Log("Running _propDensity = fx.calcWeightedDensity(_propellants, _propRatios)");

                //Create strings for the calculation
                if (useIgnoreForISP)                    //Is IgnoreForISP to be used
                { 
                    inPropellants = string.Empty;
                    inRatios = string.Empty;
                    //loop the arrays and recreate cleaned arrays
                    for (int i = 0; i < arrInPropellants.Length; i++)
                    {
                        Debug.Log("if ( !bool.TryParse(arrIgnoreForIsp[i], out IgnoreForISP) || IgnoreForISP == false)");
                        if ( !bool.TryParse(arrIgnoreForIsp[i], out IgnoreForISP) || IgnoreForISP == false)
                        {
                            inPropellants = inPropellants + "," + arrInPropellants[i];
                            inRatios = inRatios + "," + arrInRatios[i];
                        }
                    }
                }

                //Calculate the weighted density of the propellants
                _propDensity = fx.calcWeightedDensity(inPropellants, inRatios);
                if ( _propDensity > 0 ) { returnSuccessStatus = true; } else { returnSuccessStatus = false; }

                //Debug.Log("_propDensity = fx.calcWeightedDensity(_propellants, _propRatios) is successfull");
            }
            catch
            {
                Debug.LogError("Guybrush101.CustomTypes.CalcDensity Failed By Exception");
                returnSuccessStatus = false;
                //throw;
            }

            return returnSuccessStatus;
        }
        //public void setMaxThrust(string inMaxThrust)
        //{
        //    float outMaxThrust = 0;
        //    Single.TryParse(inMaxThrust, out outMaxThrust);
        //    _maxThrust = outMaxThrust;
        //}
        public string setMaxThrust
        {
            set
            {
                float outMaxThrust = 0;
                Single.TryParse(value, out outMaxThrust);
                _maxThrust = outMaxThrust;
            }
        }
        public float maxThrust
        {
            get { return _maxThrust; }
        }
        public string propIgnoreForISP
        {
            get { return _ignoreForIsp; }
            set { _ignoreForIsp = value; }
        }
        public string propDrawGauge
        {
            get { return _drawGauge; }
            set { _drawGauge = value; }
        }
        public string heatProduction
        {
            get { return _heatProd; }
            set { _heatProd = value; }
        }
        public string engineType
        {
            get { return _engineType; }
            set { _engineType = value; }
        }







        public int propAmount
        {
            get { return _propAmount; }
            //set { _propAmount = value; }  //Internally calculated
        }
        public string atmosphereCurve
        {
            get { return _atmosphereCurve; }
            set { _atmosphereCurve = value; }
        }
        public string velCurve
        {
            get { return _velCurve; }
            set { _velCurve = value; }
        }
        public string atmCurve
        {
            get { return _atmCurve; }
            set { _atmCurve = value; }
        }
    }
}
