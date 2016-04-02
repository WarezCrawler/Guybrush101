using System;
using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    partial class GTI_EngineClassSwitch : PartModule
    {
        #region KSPFields and supporting settings
            //GUI fields for information
            //,UI_Toggle(affectSymCounterparts = UI_Scene.Editor, controlEnabled = true, disabledText = "Show Engine Switcher", enabledText = "Hide Engine Switcher")
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Propellants")]
        private string GUIpropellantNames = String.Empty;
        [KSPField]
        public string iniGUIpropellantNames = string.Empty;

        //Availability of the functionality
        [KSPField]
        public bool availableInFlight = false;
        [KSPField]
        public bool availableInEditor = true;

        private bool RightClickUI_onoff = false;
        #endregion

        #region User_Interface
        //[UI_Toggle(controlEnabled = true, disabledText = "Close Engine Switcher", enabledText = "Open Engine Switcher")]
        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Show Engine Switcher")]
        public void toggleRightClickUI()
        {
            RightClickUI_onoff = !RightClickUI_onoff;
            if (RightClickUI_onoff)
            {
                var togglerightclickUI = Events["toggleRightClickUI"];
                togglerightclickUI.guiName = "Hide Engine Switcher";

                var nextEvent = Events["nextPropellantEvent"];
                nextEvent.guiActive = availableInFlight;
                nextEvent.guiActiveEditor = availableInEditor;
                //nextEvent.guiName = nextTankSetupText;

                var previousEvent = Events["previousPropellantEvent"];
                previousEvent.guiActive = availableInFlight;
                previousEvent.guiActiveEditor = availableInEditor;

                //foreach (var moduleEngine in this.ModuleEngines)
                //{
                //    //Get the Ignition state, i.e. is the engine shutdown or activated
                //    engineState = moduleEngine.getIgnitionState;
                //    //Shutdown the engine --> Removes the gauges, and make sense to do before changing propellant
                //    moduleEngine.Shutdown();
                //}
            }
            else
            {
                var togglerightclickUI = Events["toggleRightClickUI"];
                togglerightclickUI.guiName = "Show Engine Switcher";

                var nextEvent = Events["nextPropellantEvent"];
                nextEvent.guiActive = false;
                nextEvent.guiActiveEditor = false;

                var previousEvent = Events["previousPropellantEvent"];
                previousEvent.guiActive = false;
                previousEvent.guiActiveEditor = false;
            }
        }





        //START - Events for selection of propellants
        //NEXT
        [KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "Next propellant setup")]
        public void nextPropellantEvent()
        {
            //InitializeSettings();
            selectedPropellant++;
            if (selectedPropellant > arrPropellantNames.GetUpperBound(0))
            {
                //if we move from last propellant, then the next one is the first one - aka 0
                selectedPropellant = 0;
            }
            updateEngineModule(true);
        }
        //PREVIOUS
        [KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "Previous propellant setup")]
        public void previousPropellantEvent()
        {
            //InitializeSettings();
            selectedPropellant--;
            if (selectedPropellant < 0)
            {
                //if we move from the first propellant, then the previous is the last - aka the upperbound
                selectedPropellant = arrPropellantNames.GetUpperBound(0);
            }
            updateEngineModule(true);
        }
        //END - Events for selection of propellants
        #endregion

        [KSPAction("Next propellant")]
        public void nextPropellantAction(KSPActionParam param)
        {
            nextPropellantEvent();
        }
        [KSPAction("Previous propellant")]
        public void previousPropellantAction(KSPActionParam param)
        {
            previousPropellantEvent();
        }

    }
}
