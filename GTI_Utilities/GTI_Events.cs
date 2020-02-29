using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static GTI.GTIConfig;


namespace GTI.Events
{
    /// <summary>
    /// Creates the GTI Events
    /// </summary>
    

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class GTI_EventCreator : MonoBehaviour
    {
        //public static EventVoid onThrottleChange;
        public static EventData<float, float> onThrottleChange;  //onThrottleChange<"Current Throttle", "Previous Throttle">

        private void Awake()
        {
            if (onThrottleChange == null)
            {
                GTIDebug.Log("Event 'onThrottleChange' Created", iDebugLevel.Low);
                onThrottleChange = new EventData<float, float>("onThrottleChange");
            } else { GTIDebug.Log("Event 'onThrottleChange' already exists", iDebugLevel.DebugInfo); }

        }
    }

    /// <summary>
    /// Evaluates and raises the GTI Events
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class GTI_Events : MonoBehaviour
    {
        //Threaded Tasks list
        public List<Task> ThreadTasks = new List<Task>();
        //public static EventVoid onThrottleChange;
        private static float savedThrottle;
        //private EventVoid onThrottleChangeEvent;
        private EventData<float, float> onThrottleChangeEvent;
        //private EventData<GameScenes> onSceneChange;
        public static bool EventDetectorRunning = false;
        //private bool initEvent;
        //private ConfigNode EventConfig;
        //private static Vessel CurrentVessel;

        private void Awake()
        {
            //Load Config
            //EventConfig = GetConfigurationsCFG();
            //if (!bool.TryParse(EventConfig.GetValue("initEvent"), out initEvent)) initEvent = true;

            #region Events
            GTIDebug.Log("GTI_Events find onthrottleChangeEvent on Awake()", iDebugLevel.DebugInfo);
            onThrottleChangeEvent = GameEvents.FindEvent<EventData<float, float>>("onThrottleChange");

            #region previous subscription to event and debugging
            //if (onThrottleChangeEvent != null)
            //{
            //    Debug.Log("[GTI] Adding GTI (debug) to onThrottleChange");
            //    onThrottleChangeEvent.Add(EventDebugger);
            //}
            //else Destroy(this.gameObject);


            //onSceneChange = GameEvents.FindEvent<EventData<GameScenes>>("onGameSceneLoadRequested");
            //if (onSceneChange != null)
            //{
            //    Debug.Log("Adding GTI to onGameSceneLoadRequested");
            //    onSceneChange.Add(onEventSceneChange);
            //}

            //Thread EventThread = new Thread(() => UpdateEvent());
            #endregion

            //Starting the thread which will continuously check and raise the Throttle Event if interaction was detected
            startThread();
            #endregion

            // Activate the load fixer, where cheats are temporarily activated to counteract spontaneous explotions on load of scenes.
            if (GTIConfig.ActivateLoadFixer)
                StartCoroutine(SceneLoadFixer());
        }

        //Scene Load Fixer ensures that things does not blow up right after scene load
        internal IEnumerator SceneLoadFixer()
        {
            bool NoCrashDamage = CheatOptions.NoCrashDamage;
            bool UnbreakableJoints = CheatOptions.UnbreakableJoints;

            GTIDebug.Log("Enabled cheat options", "GTI Scene LoadFixer", iDebugLevel.Medium);
            CheatOptions.NoCrashDamage = true;
            CheatOptions.UnbreakableJoints = true;

            //wait 5 seconds
            yield return new WaitForSeconds(5f);

            CheatOptions.NoCrashDamage = NoCrashDamage;
            CheatOptions.UnbreakableJoints = UnbreakableJoints;
        }

        internal void startThread()
        {
            if (GTIConfig.Event.initialize)
            {
                if (!EventDetectorRunning)
                {
                    Thread EventThread = new Thread(() =>
                    {
                        //StartCoroutine(UpdateEvent());
                        EventDetectorRunning = true;
                        GTI_inFlightEventDetector();
                    });
                    GTIDebug.Log("Starting GTI Event thread", iDebugLevel.High);
                    EventThread.Priority = System.Threading.ThreadPriority.BelowNormal;     //new 16/2-2017
                    GTIDebug.Log("GTI_inFlightEventDetector Started in new thread", iDebugLevel.DebugInfo);
                    EventThread.IsBackground = true;
                    EventThread.Start(); 
                    return;
                }
                else GTIDebug.Log("GTI onThrottle event detector allready runnning. New Activation Cancelled.", iDebugLevel.Low);
            }
            else
            {
                GTIDebug.Log("The GTI Event 'onThrottleChange' deactivated. initEvent was set to 'false'", iDebugLevel.Low);
                Destroy(this.gameObject);
            }
        }

        public static Task EventTask;
        internal void startThread_v2()
        {
            if (GTIConfig.Event.initialize)
            {
                GTIDebug.Log("GTI Event thread status " + EventTask.Status, iDebugLevel.High);

                //Check if the task has stopped
                if (EventTask.Status != TaskStatus.Running || EventTask.Status != TaskStatus.WaitingForActivation || EventTask.Status != TaskStatus.WaitingForChildrenToComplete || EventTask.Status != TaskStatus.WaitingToRun)
                {
                    EventDetectorRunning = false;
                    Thread.Sleep(1000); //Wait 1 sec to allow the thread to stop before reactivating
                }

                //force the tread to stop????!!!

                if (!EventDetectorRunning)
                {
                    EventTask =
                    (
                        Task.Factory.StartNew(() =>
                        {
                            //StartCoroutine(UpdateEvent());
                            EventDetectorRunning = true;
                            GTI_inFlightEventDetector();
                        })
                    );
                    GTIDebug.Log("Starting GTI Event thread", iDebugLevel.High);
                    GTIDebug.Log("GTI_inFlightEventDetector Started in new thread", iDebugLevel.DebugInfo);
                    return;
                }
                else GTIDebug.Log("GTI onThrottle event detector allready runnning. New Activation Cancelled.", iDebugLevel.Low);
            }
            else
            {
                GTIDebug.Log("The GTI Event 'onThrottleChange' deactivated. initEvent was set to 'false'", iDebugLevel.Low);
                Destroy(this.gameObject);
            }
        }
        private void GTI_inFlightEventDetector()        //For threaded execution --> Detects the basis for event
        {
            int wait = GTIConfig.Event.CheckFreqIdle;

            //Stopwatch for timing how long the event checking is running
            System.Diagnostics.Stopwatch stopwatch;
            stopwatch = System.Diagnostics.Stopwatch.StartNew();

            //GTIDebug.Log("Event thread GTI_inFlightEventDetector started\n\tEventCheckFreqIdle: " + GTIConfig.Event.CheckFreqIdle + "\n\tEventCheckFreqActive: " + GTIConfig.Event.CheckFreqActive, iDebugLevel.Medium);
            GTIDebug.LogAppend(iDebugLevel.Medium, "Event thread GTI_inFlightEventDetector started\n\tEventCheckFreqIdle: ", GTIConfig.Event.CheckFreqIdle.ToString(), "\n\tEventCheckFreqActive: ", GTIConfig.Event.CheckFreqActive.ToString());
            while (EventDetectorRunning)
            {
                if (savedThrottle != FlightInputHandler.state.mainThrottle)
                {
                    //if (FlightGlobals.ActiveVessel != null)     //Are any vessel active
                    //{
                    //    //CurrentVessel = FlightGlobals.ActiveVessel;
                    //}
                    //run code on throttle change here.
                    onThrottleChangeEvent.Fire(FlightInputHandler.state.mainThrottle, savedThrottle);
                    savedThrottle = FlightInputHandler.state.mainThrottle;
                    //##GTIDebug.Log("Throttle Changed to " + savedThrottle, iDebugLevel.DebugInfo);
                    //onThrottleChangeEvent.Fire();
                    wait = GTIConfig.Event.CheckFreqActive;
                }
                else { wait = GTIConfig.Event.CheckFreqIdle; }

                //if (!HighLogic.LoadedSceneIsFlight) EventDetectorRunning = false;
                Thread.Sleep(wait);
            }

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            GTIDebug.Log("GTI_inFlightEventDetector Stopped.\tRuntime = " + elapsedTime, iDebugLevel.Medium);
        }

        #region Update()
        //deprecated --> Only runs in the primary thread for some reason.
        //private IEnumerator UpdateEvent()
        //{
        //    float wait = 0.2f;
        //    //System.Diagnostics.Stopwatch stopwatch;

        //    Debug.Log("GTI Event thread UpdateEvent() started");
        //    while (true)
        //    {
        //        if (savedThrottle != FlightInputHandler.state.mainThrottle)
        //        {
        //            //run code on throttle change here.
        //            savedThrottle = FlightInputHandler.state.mainThrottle;
        //            //Debug.Log("Throttle Changed to " + savedThrottle);
        //            onThrottleChangeEvent.Fire();
        //            wait = 0.01f;
        //        }
        //        //Debug.Log("UpdateEvent() --> WaitForSeconds " + wait);
        //        Debug.Log("Before loop" + Time.time);
        //        for (int i = 0; i < 1000000000; i++)
        //        {
        //            int a = 5;
        //            a = a + 5;
        //            //do nothing
        //        }
        //        Debug.Log("After loop" + Time.time);
        //        yield return new WaitForSeconds(wait);
        //    }
        //}


        //public void Update()
        //{
        //    //Debug.Log("GTI Event Update() started");

        //    if (HighLogic.LoadedSceneIsFlight)
        //    {
        //        if (savedThrottle != FlightInputHandler.state.mainThrottle)
        //        {
        //            //run code on throttle change here.
        //            savedThrottle = FlightInputHandler.state.mainThrottle;
        //            Debug.Log("Throttle Changed to " + savedThrottle);
        //            if (onThrottleChangeEvent != null)
        //            {
        //                Debug.Log("Fire Event 'onThrottleChange'");
        //                onThrottleChangeEvent.Fire();
        //            }
        //        }
        //    }
        //}
        #endregion

        #region Event Call functions
        ///// <summary>
        ///// Start event detection if scene is changed to Flight
        ///// </summary>
        ///// <param name="newScene"></param>
        //public void onEventSceneChange(GameScenes newScene = GameScenes.FLIGHT)
        //{
        //    Debug.Log("SceneChange detected --> Trying to restart GTI onThrottle");
        //    if (newScene == GameScenes.FLIGHT)
        //    {
        //        Debug.Log("Restarting thread for event detection of GTI onThrottle");
        //        startThread();
        //    }
        //    else EventDetectorRunning = false;
        //}

        private void EventDebugger(float newThrottle, float OrigThrottle)
        {
            GTIDebug.Log("onThrottle Event Raised", iDebugLevel.DebugInfo);
        }
        #endregion

        private void OnDestroy()
        {
            GTIDebug.Log("GTI_Events destroyed", iDebugLevel.Medium);
            EventDetectorRunning = false;
            if (onThrottleChangeEvent != null)
                onThrottleChangeEvent.Remove(EventDebugger);
            //onSceneChange.Remove(onEventSceneChange);
        }

        //private ConfigNode GetConfigurationsCFG()
        //{
        //    ConfigNode node;
        //    try
        //    {
        //        //node = ConfigNode.Load(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/GTI_Config.cfg");
        //        //node = GTIConfig.GTIConfigurationNode;

        //        //Debug.Log("GTI ConfigNode\n" + node.ToString());
        //        node = GTIConfig.GTIConfigurationNode.GetNode("EventConfig");
        //        //Debug.Log("GTI ConfigNode\n" + node.ToString());
        //    }
        //    catch
        //    {
        //        Debug.LogError("[GTI] " + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/GTI_Config.cfg NOT FOUND");
        //        node = new ConfigNode();
        //    }

        //    return node;
        //}
    }
}