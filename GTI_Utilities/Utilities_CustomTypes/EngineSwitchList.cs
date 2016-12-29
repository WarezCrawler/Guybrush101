//using System;
using System.Collections;
//using UnityEngine;
//using GTI.GenericFunctions;

namespace GTI.CustomTypes
{
    public class EngineSwitchList : CollectionBase
    {
        //private string _names;            //New name of part in event of new propellant
        public string engineID = string.Empty;        //The propellants

        public bool engineAvailable = false;
        public string minReqTech = string.Empty;
        public string maxReqTech = string.Empty;
        public string GUIengineID = string.Empty;
        
        



        //public string GUIpropellantNames
        //{
        //    get { return _GUIpropellantNames; }
        //    set { _GUIpropellantNames = value; }
        //}



    }
}
