//using System;
//using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static GTI.GTIConfig;

namespace GTI
{
    public static class GTIDebug
    {
        private static object ThreadLock = new object();
        private static StringBuilder Buffer = new StringBuilder(1024);

        public static void Log(string message, iDebugLevel useDebugLevel = iDebugLevel.DebugInfo)
        {
            //if (GTIConfig.DebugActive && DebugLevel >= useDebugLevel) Debug.Log("[GTI] " + message);
            //this.GetType().Name
            lock (ThreadLock)
                if (GTIConfig.DebugActive && DebugLevel >= useDebugLevel) Debug.Log(string.Format("[GTI] {0}", message));
            //if (GTIConfig.DebugActive && DebugLevel >= useDebugLevel) Debug.Log("[GTI] " + message);

            //if (GTIConfig.DebugActive && DebugLevel >= useDebugLevel) Debug.Log(string.Format("{0} {1}", "[GTI]".ToString(), args));
        }
        public static void Log(string message, string tag, iDebugLevel useDebugLevel = iDebugLevel.DebugInfo)
        {
            lock (ThreadLock)
                if (GTIConfig.DebugActive && DebugLevel >= useDebugLevel) Debug.Log(string.Format("[{1}] {0}", message, tag));
        }
        public static void LogAppend(iDebugLevel useDebugLevel = iDebugLevel.DebugInfo, params string[] messages)
        {
            if (GTIConfig.DebugActive && DebugLevel >= useDebugLevel)
            {
                lock (ThreadLock)
                {
                    Buffer.Length = 0;
                    Buffer.Append("[GTI] ");
                    for (int i = 0; i < messages.Length; i++)
                        Buffer.Append(messages[i]);
                    Debug.Log(Buffer.ToString());
                }
            }
        }
        public static void LogAppendLine(iDebugLevel useDebugLevel = iDebugLevel.DebugInfo, params string[] messages)
        {
            if (GTIConfig.DebugActive && DebugLevel >= useDebugLevel)
            {
                lock (ThreadLock)
                {
                    Buffer.Length = 0;
                    Buffer.Append("[GTI] ");
                    for (int i = 0; i < messages.Length; i++)
                        Buffer.AppendLine(messages[i]);
                    Debug.Log(Buffer.ToString());
                }
            }
        }

        public static void LogError(string message)
        {
            lock (ThreadLock)
                if (GTIConfig.DebugActive) Debug.LogError("[GTI error] " + message);
        }
        public static void LogWarning(string message, iDebugLevel useDebugLevel = iDebugLevel.Low)
        {
            lock (ThreadLock)
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
