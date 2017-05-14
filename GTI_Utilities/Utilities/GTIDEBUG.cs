//using System;
//using System.Collections.Generic;
//using System.Text;
using UnityEngine;
using static GTI.GTIConfig;

namespace GTI
{
    public static class GTIDebug
    {
        public static void Log(object message, iDebugLevel useDebugLevel = iDebugLevel.Low)
        {
            //if (GTIConfig.DebugActive && DebugLevel >= useDebugLevel) Debug.Log("[GTI] " + message);
            //this.GetType().Name
            if (GTIConfig.DebugActive && DebugLevel >= useDebugLevel) Debug.Log("[GTI] " + message);
        }
        public static void Log(object message, string tag, iDebugLevel useDebugLevel = iDebugLevel.Low)
        {
            //if (GTIConfig.DebugActive && DebugLevel >= useDebugLevel) Debug.Log("[GTI] " + message);
            //this.GetType().Name
            if (GTIConfig.DebugActive && DebugLevel >= useDebugLevel) Debug.Log("[" + tag + "] " + message);
        }
        public static void LogError(object message)
        {
            if (GTIConfig.DebugActive) Debug.LogError("[GTI error] " + message);
        }
        public static void LogWarning(object message, iDebugLevel useDebugLevel = iDebugLevel.Low)
        {
            if (GTIConfig.DebugActive && DebugLevel >= useDebugLevel) Debug.LogWarning("[GTI warning] " + message);
        }

        public static string GetVesselName(Part part)
        {
            try
            {
                return part.vessel.GetName();
            }
            catch
            {
                return "'vessel name null'";
            }
            
        }

        public static void LogEvents(PartModule module, object message = null)
        {
            GTIDebug.Log("--- PartModule Events --- " + module.moduleName, iDebugLevel.None);
            if (message != null) Debug.Log("[GTI] " + message);
            for (int i = 0; i < module.Events.Count; i++)
            {
                Debug.Log("[GTI]" +
                     "\ni: [" + i + "]" +
                     "\nGUIName: " + module.Events[i].GUIName +
                     "\nid: " + module.Events[i].id +
                     "\nname: " + module.Events[i].name +
                     "\nactive: " + module.Events[i].active +
                     "\nassigned: " + module.Events[i].assigned +
                     "\ncategory: " + module.Events[i].category +
                     "\nexternalToEVAOnly: " + module.Events[i].externalToEVAOnly +
                     "\nguiActive: " + module.Events[i].guiActive +
                     "\nguiActiveEditor: " + module.Events[i].guiActiveEditor +
                     "\nguiActiveUncommand: " + module.Events[i].guiActiveUncommand +
                     "\nguiActiveUnfocused: " + module.Events[i].guiActiveUnfocused +
                     "\nguiIcon: " + module.Events[i].guiIcon +
                     "\nunfocusedRange: " + module.Events[i].unfocusedRange +
                     "\n");
            }
        }
    }
}
