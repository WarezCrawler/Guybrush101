using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace GTI.Config
{
    public static class GTIConfig
    {
        public static ConfigNode GTIConfigurationNode;

        #region Event Settings properties
        public static bool initEvent { get; private set; }
        public static int EventCheckFreqIdle { get; private set; }
        public static int EventCheckFreqActive { get; private set; }
        #endregion

        #region Debug Settings
        public static bool DebugActive { get; private set; }
        public static iDebugLevel DebugLevel { get; private set; } = iDebugLevel.Low;
        public enum iDebugLevel { None, Low, Medium, High, VeryHigh, DebugInfo };
        #endregion

        static GTIConfig()
        {
            int myinteger;
            bool mybool;
            string mystring = string.Empty;
            ConfigNode myConfigNode;

            try
            {
                GTIConfigurationNode = new ConfigNode();
                GTIConfigurationNode = ConfigNode.Load(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/GTI_Config.cfg");
                Debug.Log("[GTI] Loading Settings -- public static class GTIConfig");
            }
            catch
            {
                Debug.LogError("[GTI] " + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\GTI_Config.cfg NOT FOUND");
            }
            DebugLevel = iDebugLevel.DebugInfo;

            #region Load Debug settings
            myConfigNode = GTIConfigurationNode.GetNode("Debug");
            if (!bool.TryParse(myConfigNode.GetValue("DebugActive"), out mybool)) DebugActive = false; else DebugActive = mybool;
            try
            {
                if (myConfigNode.TryGetValue("DebugLevel", ref mystring))
                {
                    DebugLevel = (iDebugLevel)Enum.Parse(typeof(iDebugLevel), myConfigNode.GetValue("DebugLevel"), false);
                    if (!Enum.IsDefined(typeof(iDebugLevel), DebugLevel)) DebugLevel = iDebugLevel.Low;
                }
            }
            catch
            {
                Debug.LogError("[GTI DEBUG] Loading DebugLevel failed. Reverting to default level.");
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
                , iDebugLevel.Low);
        }
    }
}
