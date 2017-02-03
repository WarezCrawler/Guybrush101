using System.Collections;
using System.Threading;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace GTI.Events
{
    /// <summary>
    /// Creates the GTI Events
    /// </summary>
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class GTI_EventCreator : MonoBehaviour
    {
        public static EventVoid onThrottleChange;

        private void Awake()
        {
            Debug.Log("[GTI] Event 'onThrottleChange' Created");
            if (onThrottleChange == null) onThrottleChange = new EventVoid("onThrottleChange");
        }
    }

    /// <summary>
    /// Evaluates and raises the GTI Events
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class GTI_Events : MonoBehaviour
    {
        //public static EventVoid onThrottleChange;
        private static float savedThrottle;
        private EventVoid onThrottleChangeEvent;
        //private EventData<GameScenes> onSceneChange;
        public static bool EventDetectorRunning = false;
        private bool initEvent;
        private ConfigNode EventConfig;

        private void Awake()
        {
            //Load Config
            EventConfig = GetConfigurationsCFG();
            if (!bool.TryParse(EventConfig.GetValue("initEvent"), out initEvent)) initEvent = true;

            onThrottleChangeEvent = GameEvents.FindEvent<EventVoid>("onThrottleChange");
            
            //if (onThrottleChangeEvent != null)
            //{
            //    Debug.Log("[GTI] Adding GTI (debug) to onThrottleChange");
            //    onThrottleChangeEvent.Add(EventDebugger);
            //}
            //else Destroy(this.gameObject);

            #region previous subscription to event
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
        }

        private void startThread()
        {
            if (initEvent)
            {
                if (!EventDetectorRunning)
                {
                    Thread EventThread = new Thread(() =>
                    {
                        //StartCoroutine(UpdateEvent());
                        EventDetectorRunning = true;
                        GTI_inFlightEventDetector();
                    });
                    Debug.Log("[GTI] Starting GTI Event thread");
                    EventThread.Start();
                }
                else Debug.Log("[GTI] GTI onThrottle event detector allready runnning. New Activation Cancelled.");
            }
            else
            {
                Debug.Log("[GTI] The GTI Event 'onThrottleChange' deactivated. initEvent was set to 'false'");
                Destroy(this.gameObject);
            }
        }
        private void GTI_inFlightEventDetector()        //For threaded execution --> Detects the basis for event
        {
            Debug.Log("[GTI] GTI_inFlightEventDetector Started in new thread");
            int EventCheckFreqIdle, EventCheckFreqActive;
            if (!int.TryParse(EventConfig.GetValue("EventCheckFreqIdle"),   out EventCheckFreqIdle  ))      EventCheckFreqIdle   = 500;
            if (!int.TryParse(EventConfig.GetValue("EventCheckFreqActive"), out EventCheckFreqActive))      EventCheckFreqActive = 100;
            int wait = EventCheckFreqIdle;

            //Stopwatch for timing how long the event checking is running
            System.Diagnostics.Stopwatch stopwatch;
            stopwatch = System.Diagnostics.Stopwatch.StartNew();

            Debug.Log("[GTI] Event thread GTI_inFlightEventDetector started\nEventCheckFreqIdle: " + EventCheckFreqIdle + "\nEventCheckFreqActive: " + EventCheckFreqActive);
            while (EventDetectorRunning)
            {
                if (savedThrottle != FlightInputHandler.state.mainThrottle)
                {
                    //run code on throttle change here.
                    savedThrottle = FlightInputHandler.state.mainThrottle;
                    //Debug.Log("[GTI] Throttle Changed to " + savedThrottle);
                    onThrottleChangeEvent.Fire();
                    wait = EventCheckFreqActive;
                }
                else { wait = EventCheckFreqIdle; }

                //if (!HighLogic.LoadedSceneIsFlight) EventDetectorRunning = false;
                Thread.Sleep(wait);
            }
            stopwatch.Stop();
            var ts = stopwatch.Elapsed;
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Debug.Log("[GTI] GTI_inFlightEventDetector Stopped.\tRuntime = " + elapsedTime);
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

        private void EventDebugger()
        {
            Debug.Log("[GTI] onThrottle Event Raised");
        }
        #endregion
        
        private void OnDestroy()
        {
            Debug.Log("[GTI] GTI_Events destroyed");
            EventDetectorRunning = false;
            if (onThrottleChangeEvent != null) onThrottleChangeEvent.Remove(EventDebugger);
            //onSceneChange.Remove(onEventSceneChange);
        }

        private ConfigNode GetConfigurationsCFG()
        {
            ConfigNode node = ConfigNode.Load(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/GTI_Config.cfg");

            //Debug.Log("GTI ConfigNode\n" + node.ToString());
            node = node.GetNode("EventConfig");
            //Debug.Log("GTI ConfigNode\n" + node.ToString());

            return node;
        }
    }
}