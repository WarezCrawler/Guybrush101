/*Licence https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode*/
/*Credit goes to: codepants (Creator of Crowd-Sourced Science Logs), DuoDex.*/

using static GTI.GTIConfig;

namespace GTI
{
    public abstract class GTISettingsBase : GameParameters.CustomParameterNode
    {
        #region CustomParameterNode
        public override string Section => "GTIndustries";
        public override string DisplaySection => Section;
        public override GameParameters.GameMode GameMode => GameParameters.GameMode.ANY;
        public override bool HasPresets => false;
        #endregion
    }
    
    public class GTISettings : GTISettingsBase
    {
        #region CustomParameterNode
        public override string Title => "Settings";
        public override int SectionOrder => 0;
        #endregion

        #region Events
        //[GameParameters.CustomStringParameterUI("Events", autoPersistance = true, title = "Event", toolTip = "change GTI custom event settings")]
        //public string stringEvents { get; set; } = "";

        [GameParameters.CustomParameterUI("Activate Events", toolTip = "Activate GTI custom events.", autoPersistance = true)]
        public bool initEvent
        {
            get => GTIConfig.Event.initialize;
            set => GTIConfig.Event.initialize = value;
        }

        [GameParameters.CustomIntParameterUI("Check Frequence Idle", toolTip = "Set interval in ms in which the event is checked for when idle.", minValue = 50 , maxValue = 1250, stepSize = 50, autoPersistance = true)]
        public int EventCheckFreqIdle
        {
            get => GTIConfig.Event.CheckFreqIdle;
            set => GTIConfig.Event.CheckFreqIdle = value;
        }

        [GameParameters.CustomIntParameterUI("Check Frequence Active", toolTip = "Set interval in ms in which the event is checked for when it has just fired.", minValue = 1, maxValue = 500, stepSize = 10, autoPersistance = true)]
        public int EventCheckFreqActive
        {
            get => GTIConfig.Event.CheckFreqActive;
            set => GTIConfig.Event.CheckFreqActive = value;
        }
        #endregion


        //** NEW 17-06-2017
        #region Other Settings
        [GameParameters.CustomIntParameterUI("Activate Load Fixer", toolTip = "Temporarily enable cheats on scene load.", autoPersistance = true)]
        public bool LoadFixerEnabled
        {
            get => GTIConfig.ActivateLoadFixer;
            set => GTIConfig.ActivateLoadFixer = value;
        }
        [GameParameters.CustomIntParameterUI("Activate CrowdSourcedScienceFixer", toolTip = "CrowdSourcedScienceFixer.", autoPersistance = true)]
        public bool GTI_ActivateCrowdSourcedScienceFixer
        {
            get => GTIConfig.CrowdSourcedScienceFixer.Activate;
            set => GTIConfig.CrowdSourcedScienceFixer.Activate = value;
        }
        [GameParameters.CustomIntParameterUI("Activate CameraFocusChanger", toolTip = "CameraFocusChanger.", autoPersistance = true)]
        public bool GTI_CameraFocusChanger
        {
            get => GTIConfig.CameraFocusChanger.Activate;
            set => GTIConfig.CameraFocusChanger.Activate = value;
        }
        [GameParameters.CustomIntParameterUI("Activate Docking Alignment Indicator", toolTip = "Docking Alignment Indicator.", autoPersistance = true)]
        public bool GTI_NavBallDockingAlignmentIndicator
        {
            get => GTIConfig.NavBallDockingIndicator.Activate;
            set => GTIConfig.NavBallDockingIndicator.Activate = value;
        }
        [GameParameters.CustomIntParameterUI("Activate Project Manager", toolTip = "Project Manager.", autoPersistance = true)]
        public bool GTI_ProjectManagerScenario
        {
            get => GTIConfig.ProjectManager.Activate;
            set
            {
                GTIConfig.ProjectManager.Activate = value;
                GTIConfig.ProjectManager_v2.Activate = !GTIConfig.ProjectManager.Activate;
            }
        }
        [GameParameters.CustomIntParameterUI("Activate Project Manager v2", toolTip = "Project Manager.", autoPersistance = true)]
        public bool GTI_ProjectManagerScenario_v2
        {
            get => GTIConfig.ProjectManager_v2.Activate;
            set
            {
                GTIConfig.ProjectManager_v2.Activate = value;
                GTIConfig.ProjectManager.Activate = !GTIConfig.ProjectManager_v2.Activate;
            }
        }
        [GameParameters.CustomIntParameterUI("Activate double tabing for brake lock", toolTip = "Brake Lock.", autoPersistance = true)]
        public bool GTI_doubleTabForBrakeLock
        {
            get => GTIConfig.BrakeLock.doubleTabActive;
            set => GTIConfig.BrakeLock.doubleTabActive = value;
        }
        #endregion


        public override void OnLoad(ConfigNode node)
        {
            GTIDebug.Log("GTISettings --> OnLoad() --> GameParameters.CustomParameterNode", iDebugLevel.DebugInfo);
            GTIDebug.Log("initEvent: " + initEvent, iDebugLevel.Medium);
            GTIDebug.Log("EventCheckFreqIdle: " + EventCheckFreqIdle, iDebugLevel.Medium);
            GTIDebug.Log("EventCheckFreqActive: " + EventCheckFreqActive, iDebugLevel.Medium);
            GTIDebug.Log("LoadFixerEnabled: " + LoadFixerEnabled, iDebugLevel.Medium);
            GTIDebug.Log("ActivateDAI: " + NavBallDockingIndicator.Activate, iDebugLevel.Medium);
            GTIDebug.Log("ActivateProjectManager: " + ProjectManager.Activate, iDebugLevel.Medium);
        }
    }
    public class GTISettingsDebug : GTISettingsBase
    {
        #region CustomParameterNode
        public override string Title => "Debugging";     //{ get { return "Debugging"; } }
        public override int SectionOrder => 2;           //{ get { return 2; } }
        #endregion

        #region Debugging
        //[GameParameters.CustomStringParameterUI("Debug", autoPersistance = true, title = "Debug Settings", toolTip = "change debugging settings")]
        //public string stringDebug { get; set; } = "";

        [GameParameters.CustomParameterUI("Activate Debugging", toolTip = "If enabled, debugging logs will be generated.", autoPersistance = true)]
        public bool DebugActive
        {
            get => GTIConfig.DebugActive;
            set => GTIConfig.DebugActive = value;
        }

        [GameParameters.CustomParameterUI("Debug Level", toolTip = "Set level of debugging messages in the log file.", autoPersistance = true)]
        public iDebugLevel DebugLevel
        {
            get => GTIConfig.DebugLevel;
            set => GTIConfig.DebugLevel = value;
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
