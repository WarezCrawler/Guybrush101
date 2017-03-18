//using GTI.Config;
using static GTI.Config.GTIConfig;
using UnityEngine;

namespace GTI.Config
{
    public class GTISettings : GameParameters.CustomParameterNode
    {
        #region CustomParameterNode
        public override string Section { get { return "GTIndustries"; } }
        public override string Title { get { return "Settings"; } }

        public override int SectionOrder { get { return 1; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override bool HasPresets { get { return false; } }
        #endregion

        #region Events
        [GameParameters.CustomStringParameterUI("Events", autoPersistance = true, title = "Event", toolTip = "change GTI custom event settings")]
        public string stringEvents { get; set; } = "";

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

        #region Debugging
        [GameParameters.CustomStringParameterUI("Debug", autoPersistance = true, title = "Debug Settings", toolTip = "change debugging settings")]
        public string stringDebug { get; set; } = "";

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
            set { GTIConfig.DebugLevel = value;  }
        }
        #endregion

        public override void OnLoad(ConfigNode node)
        {
            GTIDebug.Log("GTISettings --> OnLoad() --> GameParameters.CustomParameterNode", iDebugLevel.DebugInfo);
            GTIDebug.Log("DebugActive: " + DebugActive, iDebugLevel.DebugInfo);
            GTIDebug.Log("DebugLevel: " + DebugLevel, iDebugLevel.DebugInfo);
            GTIDebug.Log("initEvent: " + initEvent, iDebugLevel.DebugInfo);
            GTIDebug.Log("EventCheckFreqIdle: " + EventCheckFreqIdle, iDebugLevel.DebugInfo);
            GTIDebug.Log("EventCheckFreqActive: " + EventCheckFreqActive, iDebugLevel.DebugInfo);

            //Apply settings the GTIconfig class
            //if (!GTIConfig.SetDebugActive(DebugActive)) GTIDebug.LogError("Loading of DebugActive Failed");
            //if (!GTIConfig.SetDebugLevel(DebugLevel)) GTIDebug.LogError("Loading of DebugLevel Failed");
        }
    }
}
