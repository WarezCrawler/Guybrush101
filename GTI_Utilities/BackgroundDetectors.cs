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
            get { return GTIConfig.DAI_NavBallDockingActive; }
            set { GTIConfig.DAI_NavBallDockingActive = value; }
        }

        private void Awake()
        {
            if (GTIConfig.ActivateDAI)
            {
                GTIDebug.Log("BackgroundDetector_Flight Awake", GTIConfig.iDebugLevel.High);
                Thread InFlightThread = new Thread(DetectInFlight);
                InFlightThread.IsBackground = true;
                InFlightThread.Priority = System.Threading.ThreadPriority.BelowNormal;
                InFlightThread.Start();
            }
        }

        private void DetectInFlight()
        {
            //If already running, then stop it before restarting
            if (Active) { Active = false; Thread.Sleep(100); }
            if (!GTIConfig.ActivateDAI)
                return;

            GTIDebug.Log("DetectInFlight() Started -- " + HighLogic.LoadedScene.ToString(), GTIConfig.iDebugLevel.High);
            Active = true;
            while (Active)
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
            Active = false;
        }
    }
}
