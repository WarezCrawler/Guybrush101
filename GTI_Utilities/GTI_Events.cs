using System.Collections;
using System.Threading;
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
            Debug.Log("GTI Event 'onThrottleChange' Created");
            if (onThrottleChange == null) onThrottleChange = new EventVoid("onThrottleChange");
        }
    }

    /// <summary>
    /// Evaluates and raises the GTI Events
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, true)]
    public class GTI_Events : MonoBehaviour
    {
        //public static EventVoid onThrottleChange;
        private static float savedThrottle;
        private EventVoid onThrottleChangeEvent;

        private void Awake()
        {
            onThrottleChangeEvent = GameEvents.FindEvent<EventVoid>("onThrottleChange");
            if (onThrottleChangeEvent != null)
            {
                //onThrottleChangeEvent.Add(EventDebugger);
            }
            else Destroy(this.gameObject);

            //Thread EventThread = new Thread(() => UpdateEvent());
            
            //Starting the thread which will continuously check and raise the Throttle Event if interaction was detected
            Thread EventThread = new Thread(() =>
            {
                StartCoroutine(UpdateEvent());
            });
            Debug.Log("Starting GTI Event thread");
            EventThread.Start();
        }

        private IEnumerator UpdateEvent()
        {
            float wait = 0.2f;

            Debug.Log("GTI Event thread UpdateEvent() started");
            while (true)
            {
                if (savedThrottle != FlightInputHandler.state.mainThrottle)
                {
                    //run code on throttle change here.
                    savedThrottle = FlightInputHandler.state.mainThrottle;
                    //Debug.Log("Throttle Changed to " + savedThrottle);
                    onThrottleChangeEvent.Fire();
                    wait = 0.01f;
                }
                Debug.Log("UpdateEvent() --> WaitForSeconds " + wait);
                yield return new WaitForSeconds(wait);
            }
        }

        #region Update()
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

        private void EventDebugger()
        {
            Debug.Log("GTI Throttle Event Raised");
        }

        private void OnDestroy()
        {
            Debug.Log("GTI_Events destroyed");
            if (onThrottleChangeEvent != null) onThrottleChangeEvent.Remove(EventDebugger);
        }
    }
}
