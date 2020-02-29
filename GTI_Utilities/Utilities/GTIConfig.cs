using System;
using System.IO;
using System.Reflection;
using UnityEngine;
//using static GTI.GTIConfig;
//using static GTI.GTISettings;

namespace GTI
{
    public static class GTIConfig
    {
        //public struct KSP_Version
        //{
        //    public int Major;
        //    public int Minor;
        //    public int Revision;
        //}

        private static object _syncLock = new object();
        internal static object syncLock => _syncLock;
        public static ConfigNode GTIConfigurationNode;
        
        //private static EventVoid onGameSettingsApplied;

        #region Event Settings properties
        public struct Event
        {
            public static bool initialize { get; internal set; }
            public static int CheckFreqIdle { get; internal set; }
            public static int CheckFreqActive { get; internal set; }
        }
        #endregion

        #region Debug Settings
        public static bool DebugActive { get; internal set; } = false;
        public static iDebugLevel DebugLevel { get; internal set; } = iDebugLevel.DebugInfo;

        public enum iDebugLevel { None, Low, Medium, High, VeryHigh, DebugInfo };
        #endregion

        #region MISCELLANEOUS Settings
        public static bool ActivateLoadFixer { get; internal set; } = false;
        public struct NavBallDockingIndicator
        {
            public static bool Activate { get; internal set; } = false;      //Is DockingAlignmentIndicator activated
            private static bool _Active = false;                             //Is DockingAlignmentIndicator active
            /// <summary>
            /// Threading safe
            /// </summary>
            public static bool Active                                        //Expose DockingAlignmentIndicator avtive information
            {
                get
                {
                    lock (_syncLock)
                        return _Active;
                }

                internal set
                {
                    lock (_syncLock)
                        _Active = value;
                }
            }
        }
        //ProjectManager
        public struct ProjectManager
        {
            public static bool Activate { get; internal set; } = false;      //Is DockingAlignmentIndicator activated
            private static bool _Active = false;                             //Is DockingAlignmentIndicator active
            /// <summary>
            /// Threading safe
            /// </summary>
            public static bool Active                                        //Expose DockingAlignmentIndicator avtive information
            {
                get
                {
                    lock (_syncLock)
                        return _Active;
                }
                internal set
                {
                    lock (_syncLock)
                        _Active = value;
                }
            }
        }
        public struct ProjectManager_v2
        {
            public static bool Activate { get; internal set; } = false;      //Is DockingAlignmentIndicator activated
            private static bool _Active = false;                             //Is DockingAlignmentIndicator active
            /// <summary>
            /// Threading safe
            /// </summary>
            public static bool Active                                        //Expose DockingAlignmentIndicator avtive information
            {
                get
                {
                    lock (_syncLock)
                        return _Active;
                }
                internal set
                {
                    lock (_syncLock)
                        _Active = value;
                }
            }
        }
        public struct BrakeLock
        {
            private static bool _doubleTabActive = false;
            public static bool doubleTabActive
            {
                get
                {
                    lock (_syncLock)
                        return _doubleTabActive;
                }

                internal set
                {
                    lock (_syncLock)
                        _doubleTabActive = value;
                }
            }
        }       
        public struct CameraFocusChanger
        {
            public static bool Activate { get; internal set; } = false;
        }
        public struct CrowdSourcedScienceFixer
        {
            public static bool Activate { get; internal set; } = false;
        }
        #endregion

        static GTIConfig()
        {
            int myinteger;
            bool mybool;
            string mystring = string.Empty;
            ConfigNode myConfigNode = new ConfigNode();

            //Versioning v = Versioning.Instance as Versioning;
            //if (v.versionMajor == 1 && v.versionMinor == 4 && v.revision == 2)

            try
            {
                GTIConfigurationNode = new ConfigNode();
                GTIConfigurationNode = ConfigNode.Load(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/GTI_Config.cfg");
                Debug.Log("[GTI] Loading Settings -- public static class GTIConfig");
                Debug.Log(GTIConfigurationNode.ToString());
            }
            catch
            {
                Debug.LogError("[GTI] " + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\GTI_Config.cfg NOT FOUND");
            }
            DebugLevel = iDebugLevel.DebugInfo;

            #region Load Debug settings
            //myConfigNode = GTIConfigurationNode.GetNode("Debug");
            if (GTIConfigurationNode.TryGetNode("Debug", ref myConfigNode))
            {
                if (!bool.TryParse(myConfigNode.GetValue("DebugActive"), out mybool)) DebugActive = true; else DebugActive = mybool;
                try
                {
                    if (myConfigNode.TryGetValue("DebugLevel", ref mystring))
                    {
                        //DebugLevel = (iDebugLevel)Enum.Parse(typeof(iDebugLevel), myConfigNode.GetValue("DebugLevel"), false);
                        DebugLevel = (iDebugLevel)Enum.Parse(typeof(iDebugLevel), mystring, false);
                        if (!Enum.IsDefined(typeof(iDebugLevel), DebugLevel)) DebugLevel = iDebugLevel.DebugInfo;
                    }
                }
                catch
                {
                    Debug.LogError("[GTI DEBUG] Loading DebugLevel failed. Reverting to default level.");
                    DebugLevel = iDebugLevel.Low;
                }
            }
            else
            {
                //Default values
                DebugActive = false;
                DebugLevel = iDebugLevel.Low;
            }
            #endregion

            #region Load Event settings
            myConfigNode = GTIConfigurationNode.GetNode("EventConfig");
            //if (!bool.TryParse(myConfigNode.GetValue("initEvent"), out mybool)) initEvent = true; else initEvent = mybool;
            //if (!int.TryParse(myConfigNode.GetValue("EventCheckFreqIdle"), out myinteger)) EventCheckFreqIdle = 500; else EventCheckFreqIdle = myinteger;
            //if (!int.TryParse(myConfigNode.GetValue("EventCheckFreqActive"), out myinteger)) EventCheckFreqActive = 100; else EventCheckFreqActive = myinteger;

            if (!bool.TryParse(myConfigNode.GetValue("initEvent"), out mybool)) Event.initialize = true; else Event.initialize = mybool;
            if (!int.TryParse(myConfigNode.GetValue("EventCheckFreqIdle"), out myinteger)) Event.CheckFreqIdle = 500; else Event.CheckFreqIdle = myinteger;
            if (!int.TryParse(myConfigNode.GetValue("EventCheckFreqActive"), out myinteger)) Event.CheckFreqActive = 100; else Event.CheckFreqActive = myinteger;
            #endregion

            #region MISCELLANEOUS
            myConfigNode = GTIConfigurationNode.GetNode("MISCELLANEOUS");
            if (!bool.TryParse(myConfigNode.GetValue("ActivateLoadFixer"), out mybool)) ActivateLoadFixer = false; else ActivateLoadFixer = mybool;
            if (!bool.TryParse(myConfigNode.GetValue("ActivateDAI"), out mybool)) NavBallDockingIndicator.Activate = false; else NavBallDockingIndicator.Activate = mybool;
            if (!bool.TryParse(myConfigNode.GetValue("ActivateDoubleTabForBrakeLock"), out mybool)) BrakeLock.doubleTabActive = false; else BrakeLock.doubleTabActive = mybool;
            if (!bool.TryParse(myConfigNode.GetValue("ActivateCameraFocusChanger"), out mybool)) CameraFocusChanger.Activate = false; else CameraFocusChanger.Activate = mybool;
            if (!bool.TryParse(myConfigNode.GetValue("ActivateCrowdSourcedScienceFixer"), out mybool)) CrowdSourcedScienceFixer.Activate = false; else CrowdSourcedScienceFixer.Activate = mybool;
            if (!bool.TryParse(myConfigNode.GetValue("ActivateProjectManager"), out mybool)) ProjectManager.Activate = false; else ProjectManager.Activate = mybool;


            #endregion

            //DebugLevel = (iDebugLevel)Enum.Parse(typeof(iDebugLevel), myConfigNode.GetValue("DebugLevel"), false);

            GTIDebug.Log(
                "\nEvent Settings" +
                //"\n- initEvent: " + initEvent +
                //"\n- EventCheckFreqIdle: " + EventCheckFreqIdle + " ms" +
                //"\n- EventCheckFreqActive: " + EventCheckFreqActive + " ms" +
                "\n- EventSettings.initialize: " + Event.initialize +
                "\n- EventSettings.CheckFreqIdle: " + Event.CheckFreqIdle + " ms" +
                "\n- EventSettings.CheckFreqActive: " + Event.CheckFreqActive + " ms" +
                "\nMISCELLANEOUS" +
                "\n- LoadFixerEnabled: " + ActivateLoadFixer +
                "\n- ActivateDAI: " + NavBallDockingIndicator.Activate +
                "\n- ActivateDoubleTabForBrakeLock: " + BrakeLock.doubleTabActive +
                "\nDebug Settings" +
                "\n- DebugActive: " + DebugActive +
                "\n- DebugLevel: " + DebugLevel.ToString()
                , iDebugLevel.DebugInfo);

        }
    }
}