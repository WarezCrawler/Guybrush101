using System.Threading;
using UnityEngine;

namespace GTI
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class BackgroundDetector_Flight : MonoBehaviour
    {
        private bool Active = false;

        private static bool DAI_NavBallDockingActive
        {
            get => GTIConfig.NavBallDockingIndicator.Active;
            set => GTIConfig.NavBallDockingIndicator.Active = value;
        }

        //KeyBinding brakeKey;

        private void Awake()
        {
            if (GTIConfig.NavBallDockingIndicator.Activate)
            {
                GTIDebug.Log("BackgroundDetector_Flight Awake", GTIConfig.iDebugLevel.High);
                Thread InFlightThread = new Thread(DetectInFlight_250ms);
                InFlightThread.IsBackground = true;
                InFlightThread.Priority = System.Threading.ThreadPriority.BelowNormal;
                InFlightThread.Start();
            }
        }

        //private void Start()
        //{
        //    //brakeKey = GameSettings.BRAKES;
        //}

        private void Update()
        {
            //Double tab for locking brakes
            if (GTIConfig.BrakeLock.doubleTabActive)
                if (GameSettings.BRAKES.GetDoubleTapUp(false)) FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, true);
        }

        private void DetectInFlight_250ms()
        {
            //If already running, then stop it before restarting
            if (Active) { Active = false; Thread.Sleep(100); }
            if (!GTIConfig.NavBallDockingIndicator.Activate)
                return;

            GTIDebug.Log("DetectInFlight() Started -- " + HighLogic.LoadedScene.ToString(), GTIConfig.iDebugLevel.High);
            Active = true;
            while (Active == true)
            {
                if (!HighLogic.LoadedSceneIsFlight)
                {
                    Active = false;
                    break;
                }

                //Detect if active vessel is in docking procedure mode
                DAI_NavBallDockingActive =
                       FlightGlobals.fetch != null
                    && FlightGlobals.ready
                    && FlightGlobals.fetch.activeVessel != null
                    && FlightGlobals.fetch.VesselTarget != null
                    && FlightGlobals.fetch.VesselTarget.GetTargetingMode() == VesselTargetModes.DirectionVelocityAndOrientation;
                Thread.Sleep(250);
            }
            GTIDebug.Log("DetectInFlight() ended -- " + HighLogic.LoadedScene.ToString(), GTIConfig.iDebugLevel.High);
        }

        private void OnDestroy()
        {
            GTIDebug.Log("BackgroundDetector_Flight -- OnDestroy()", GTIConfig.iDebugLevel.DebugInfo);
            Active = false;
        }
    }
}
