using System;
using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    partial class GTI_MultiModeEngineFX : PartModule
    {
        #region VAB Information
        public override string GetInfo()
        {
            try
            {
                //we need to run the InitializeSettings here, because the OnStart does not run before this.
                initializeSettings();

                //string strOutInfo = string.Empty;
                //System.Text.StringBuilder strOutInfo = new System.Text.StringBuilder();
                //string[] _propellants, _propratios;

                //strOutInfo.AppendLine("Propellants available");
                //foreach (CustomTypes.PropellantList item in propList)
                //{
                //    strOutInfo.AppendLine(item.Propellants.Replace(",",", "));
                //}
                //strOutInfo.AppendLine(propellantNames.Replace(";", "; "));
                return "GTI Multi Mode Engine FX";
            }
            catch (Exception e)
            {
                Debug.LogError("GTI_MultiModeEngineFX GetInfo Error " + e.Message);
                throw;
            }
        }
        #endregion
    }
    partial class GTI_MultiModeEngine : PartModule
    {
        #region VAB Information
        public override string GetInfo()
        {
            try
            {
                //we need to run the InitializeSettings here, because the OnStart does not run before this.
                //initializeSettings(false);

                

                //string strOutInfo = string.Empty;
                //System.Text.StringBuilder strOutInfo = new System.Text.StringBuilder();
                //string[] _propellants, _propratios;

                //strOutInfo.AppendLine("Propellants available");
                //foreach (CustomTypes.PropellantList item in propList)
                //{
                //    strOutInfo.AppendLine(item.Propellants.Replace(",",", "));
                //}
                //strOutInfo.AppendLine(propellantNames.Replace(";", "; "));
                return "GTI Multi Mode Engine";
            }
            catch (Exception e)
            {
                Debug.LogError("GTI_MultiModeEngine GetInfo Error " + e.Message);
                throw;
            }
        }
        #endregion
    }
}
