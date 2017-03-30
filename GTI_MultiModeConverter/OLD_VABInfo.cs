using System;
using UnityEngine;

namespace GTI
{
    partial class OLD_GTI_MultiModeConverter : PartModule
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
                return "GTI Multi Mode Converter";
            }
            catch (Exception e)
            {
                Debug.LogError("GTI_MultiModeConverter GetInfo Error " + e.Message);
                throw;
            }
        }
        #endregion
    }
}
