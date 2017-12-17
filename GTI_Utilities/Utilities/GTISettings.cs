using System;
using static GTI.GTIConfig;

namespace GTI
{
    public abstract class GTISettingsBase : GameParameters.CustomParameterNode
    {
        #region CustomParameterNode
        public override string Section { get { return "GTIndustries"; } }
        public override string DisplaySection { get { return Section; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override bool HasPresets { get { return false; } }
        #endregion
    }
    
    public class GTISettings : GTISettingsBase
    {
        #region CustomParameterNode
        public override string Title { get { return "Settings"; } }
        public override int SectionOrder { get { return 0; } }
        #endregion

        #region Events
        //[GameParameters.CustomStringParameterUI("Events", autoPersistance = true, title = "Event", toolTip = "change GTI custom event settings")]
        //public string stringEvents { get; set; } = "";

        [GameParameters.CustomParameterUI("Activate Events", toolTip = "Activate GTI custom events.", autoPersistance = true)]
        public bool initEvent
        {
            get { return GTIConfig.initEvent; }
            set { GTIConfig.initEvent = value; }
        }

        [GameParameters.CustomIntParameterUI("Check Frequence Idle", toolTip = "Set interval in ms in which the event is checked for when idle.", minValue = 50 , maxValue = 1250, stepSize = 50, autoPersistance = true)]
        public int EventCheckFreqIdle
        {
            get { return GTIConfig.EventCheckFreqIdle; }
            set { GTIConfig.EventCheckFreqIdle = value; }
        }

        [GameParameters.CustomIntParameterUI("Check Frequence Active", toolTip = "Set interval in ms in which the event is checked for when it has just fired.", minValue = 1, maxValue = 500, stepSize = 10, autoPersistance = true)]
        public int EventCheckFreqActive
        {
            get { return GTIConfig.EventCheckFreqActive; }
            set { GTIConfig.EventCheckFreqActive = value; }
        }
        #endregion


        //** NEW 17-06-2017
        #region Other Settings
        [GameParameters.CustomIntParameterUI("Activate Load Fixer", toolTip = "Temporarily enable cheats on scene load.", autoPersistance = true)]
        public bool LoadFixerEnabled
        {
            get { return GTIConfig.LoadFixerEnabled; }
            set { GTIConfig.LoadFixerEnabled = value; }
        }
        #endregion


        public override void OnLoad(ConfigNode node)
        {
            GTIDebug.Log("GTISettings --> OnLoad() --> GameParameters.CustomParameterNode", iDebugLevel.DebugInfo);
            GTIDebug.Log("initEvent: " + initEvent, iDebugLevel.Medium);
            GTIDebug.Log("EventCheckFreqIdle: " + EventCheckFreqIdle, iDebugLevel.Medium);
            GTIDebug.Log("EventCheckFreqActive: " + EventCheckFreqActive, iDebugLevel.Medium);
        }
    }
    public class GTISettingsDebug : GTISettingsBase
    {
        #region CustomParameterNode
        public override string Title { get { return "Debugging"; } }
        public override int SectionOrder { get { return 2; } }
        #endregion

        #region Debugging
        //[GameParameters.CustomStringParameterUI("Debug", autoPersistance = true, title = "Debug Settings", toolTip = "change debugging settings")]
        //public string stringDebug { get; set; } = "";

        [GameParameters.CustomParameterUI("Activate Debugging", toolTip = "If enabled, debugging logs will be generated.", autoPersistance = true)]
        public bool DebugActive
        {
            get { return GTIConfig.DebugActive; }
            set { GTIConfig.DebugActive = value; }
        }

        [GameParameters.CustomParameterUI("Debug Level", toolTip = "Set level of debugging messages in the log file.", autoPersistance = true)]
        public iDebugLevel DebugLevel
        {
            get { return GTIConfig.DebugLevel; }
            set { GTIConfig.DebugLevel = value; }
        }
        #endregion

        public override void OnLoad(ConfigNode node)
        {
            GTIDebug.Log("GTISettingsDebug --> OnLoad()", iDebugLevel.DebugInfo);
            GTIDebug.Log("DebugActive: " + DebugActive, iDebugLevel.Medium);
            GTIDebug.Log("DebugLevel: " + DebugLevel, iDebugLevel.Medium);
        }
    }
}
