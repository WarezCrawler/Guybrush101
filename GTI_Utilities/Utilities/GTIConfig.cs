using System;
using System.IO;
using System.Reflection;
using UnityEngine;
//using static GTI.Config.GTIConfig;
//using static GTI.Config.GTISettings;

namespace GTI.Config
{
    public static class GTIConfig
    {
        public static ConfigNode GTIConfigurationNode;

        //private static EventVoid onGameSettingsApplied;

        #region Event Settings properties
        public static bool initEvent { get; internal set; }
        public static int EventCheckFreqIdle { get; internal set; }
        public static int EventCheckFreqActive { get; internal set; }
        #endregion

        #region Debug Settings
        //private static bool _DebugActive;
        public static bool DebugActive { get; internal set; } = false;
        //private static iDebugLevel _DebugLevel;
        public static iDebugLevel DebugLevel { get; internal set; } = iDebugLevel.DebugInfo;

        public enum iDebugLevel { None, Low, Medium, High, VeryHigh, DebugInfo };
        #endregion

        static GTIConfig()
        {
            int myinteger;
            bool mybool;
            string mystring = string.Empty;
            ConfigNode myConfigNode = new ConfigNode();

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
            if (!bool.TryParse(myConfigNode.GetValue("initEvent"), out mybool)) initEvent = true; else initEvent = mybool;
            if (!int.TryParse(myConfigNode.GetValue("EventCheckFreqIdle"), out myinteger)) EventCheckFreqIdle = 500; else EventCheckFreqIdle = myinteger;
            if (!int.TryParse(myConfigNode.GetValue("EventCheckFreqActive"), out myinteger)) EventCheckFreqActive = 100; else EventCheckFreqActive = myinteger;
            #endregion

            //DebugLevel = (iDebugLevel)Enum.Parse(typeof(iDebugLevel), myConfigNode.GetValue("DebugLevel"), false);

            GTIDebug.Log(
                "\nEvent Settings" +
                "\n- initEvent: " + initEvent +
                "\n- EventCheckFreqIdle: " + EventCheckFreqIdle + " ms" +
                "\n- EventCheckFreqActive: " + EventCheckFreqActive + " ms" +
                "\nDebug Settings" +
                "\n- DebugActive: " + DebugActive +
                "\n- DebugLevel: " + DebugLevel.ToString()
                , iDebugLevel.DebugInfo);

            //Activate loading of settings from the in game settings menu
            //SetOnGameSettingsApplied();
        }

        //public static bool DebugActive
        //{
        //    get
        //    {
        //        try
        //        {
        //            //if (HighLogic.LoadedScene == GameScenes.FLIGHT || HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.EDITOR)
        //            if (HighLogic.LoadedSceneIsGame)
        //            {
        //                GTISettings settings = HighLogic.CurrentGame.Parameters.CustomParams<GTISettings>();
        //                return settings.DebugActive;
        //            }
        //            else
        //            {
        //                return _DebugActive;
        //            }

        //        }
        //        catch
        //        {
        //            Debug.Log("[GTI] CustomParams<GTISettings>().DebugActive NOT FOUND - Returning Default");
        //            return _DebugActive;
        //        }
        //    }
        //    set { _DebugActive = value;  }
        //}
        //public static iDebugLevel DebugLevel { get; private set; } = iDebugLevel.DebugInfo;
        //public static iDebugLevel DebugLevel
        //{
        //    get
        //    {
        //        try
        //        {
        //            //if (HighLogic.LoadedScene == GameScenes.FLIGHT || HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.EDITOR)
        //            if (HighLogic.LoadedSceneIsGame)
        //            {
        //                GTISettings settings = HighLogic.CurrentGame.Parameters.CustomParams<GTISettings>();
        //                return settings.DebugLevel;
        //            }
        //            else
        //            {
        //                return _DebugLevel;
        //            }
                    
        //        }
        //        catch
        //        {
        //            Debug.Log("[GTI] CustomParams<GTISettings>().DebugLevel NOT FOUND - Returning Default");
        //            return _DebugLevel;
        //        }
        //    }
        //    private set { _DebugLevel = value; }
        //}

        ///*
        //private static void SetOnGameSettingsApplied()
        //{
        //    ////EventVoid onGameSettingsApplied;

        //    //GTIDebug.Log("Before adding GTI_MultiModeEngine to onThrottleChange Event", iDebugLevel.None);
        //    ////onGameSettingsApplied = GameEvents.OnGameSettingsApplied;
        //    //onGameSettingsApplied = GameEvents.FindEvent<EventVoid>("OnGameSettingsApplied");
        //    //GTIDebug.Log("Before adding GTI_MultiModeEngine to onThrottleChange Event", iDebugLevel.None);
        //    //if (onGameSettingsApplied != null)
        //    //{
        //    //    GTIDebug.Log("Adding GTIConfig to GameEvents.OnGameSettingsApplied Event", iDebugLevel.None);
        //    //    onGameSettingsApplied.Add(OnGameSettingsApplied);
        //    //    GTIDebug.Log("GTIConfig to GameEvents.OnGameSettingsApplied Event added", iDebugLevel.None);
        //    //}
        //    //else
        //    //{
        //    //    GTIDebug.Log("GTIConfig failed to be added to GameEvents.OnGameSettingsApplied Event : Event was null", iDebugLevel.Low);
        //    //}
        //}
        //*/
        //private static void OnDestroy()
        //{
        //    GTIDebug.Log("GTIConfig destroyed", iDebugLevel.Medium);
        //    if (GameEvents.OnGameSettingsApplied != null)
        //    {
        //        GTIDebug.Log("[GTI] Removing GTIConfig from OnGameSettingsApplied Event", iDebugLevel.Medium);
        //        GameEvents.OnGameSettingsApplied.Remove(OnGameSettingsApplied);
        //    }
        //}
        //private static void OnGameSettingsApplied()
        //{
        //    GTIDebug.Log("New GTI Game Settings Applied. Updating the config module.", iDebugLevel.None);
        //    SetDebugActive();
        //    SetDebugLevel();
        //}

        /*
        public static bool SetDebugActive()
        {
            try
            {
                GTISettings settings = HighLogic.CurrentGame.Parameters.CustomParams<GTISettings>();
                DebugActive = settings.DebugActive;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool SetDebugActive(bool newValue)
        {
            try
            {
                DebugActive = newValue;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool SetDebugLevel()
        {
            try
            {
                GTISettings settings = HighLogic.CurrentGame.Parameters.CustomParams<GTISettings>();
                DebugLevel = settings.DebugLevel;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool SetDebugLevel(iDebugLevel newValue)
        {
            try
            {
                DebugLevel = newValue;
                return true;
            }
            catch
            {
                return false;
            }
        }
        */
    }
}
