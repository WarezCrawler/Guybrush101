using System;
using System.Collections.Generic;
using static GTI.GTIConfig;


namespace GTI
{
    public interface IMultiMode
    {
        int moduleIndex { get; set; }
        string ID { get; set; }
        string Name { get; set; }

        string ToString();
    }

    //Default MultiMode object
    public class MultiMode : IMultiMode
    {
        public int moduleIndex { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return moduleIndex + "\t" + ID + "\t" + Name;
        }
        //public bool modeDisabled = false;
    }
    //public class MultiMode2
    //{
    //    public List<modeProperties> modes;
    //    public MultiMode2(int capacity)
    //    {
    //        List<modeProperties> mode = new List<modeProperties>(capacity);
    //    }
    //    public class modeProperties
    //    {
    //        public int moduleIndex;
    //        public string ID;
    //        public string Name;
    //        public bool modeDisabled = false;
    //    }      
    //}

    public abstract class GTI_MultiMode<T> : PartModule where T : IMultiMode
    {
        protected bool _settingsInitialized = false;
        protected bool _GUIsettingsInitialized = false;
        protected bool _MAGsettingsInitialized = false;

        //public string[] MultiModeID;
        //public string[] MultiModeNames;

        //public List<MultiMode> mode { get; protected set; }
        public List<T> modes { get; protected set; }

        //protected string txtDeploy = "Deploy";
        //protected string txtRetract = "Retract";

        //Availability of the functionality
        [KSPField]
        public bool availableInFlight = false;
        [KSPField]
        public bool availableInEditor = true;
        [KSPField]
        public bool externalToEVAOnly = false;

        [KSPField]
        public bool useModuleAnimationGroup = false;
        //protected Animation Anim;
        protected ModuleAnimationGroup MAG;

        [KSPField]
        public bool affectSymCounterpartsInFlight = false;

        [KSPField]
        public string messagePosition = string.Empty;

        /// <summary>
        /// Currently selected mode in integer format for use in the arrays
        /// </summary>
        public int selectedMode { get; protected set; } = 0;

        public override void OnStart(PartModule.StartState state)
        {
            GTIDebug.Log("GTI_MultiMode baseclass --> OnStart()", iDebugLevel.DebugInfo);

            //Assign update method to delegate
            OnUpdateMultiMode = FindSelectedMode;
            OnUpdateMultiMode += updateMultiMode;
            
            initialize();

            this.Events["EVAChangeMode"].active = externalToEVAOnly;
        }

        /// <summary>
        /// After onstart, the mode can be updated. This cannot be done sooner, since all settings does not seem to be finished loading at that point.
        /// </summary>
        /// <param name="state"></param>
        public override void OnStartFinished(StartState state)
        {
            //updateMultiMode(silentUpdate: true);
            OnUpdateMultiMode.Invoke(silentUpdate: true);
        }
        protected void InvokeOnUpdateMultiMode(bool silentUpdate = false)
        {
            OnUpdateMultiMode.Invoke(silentUpdate);
        }

        /// <summary>
        /// Updates the module with new selections. Deactivating the inactive ones, and activating the selected one.
        /// </summary>
        public abstract void updateMultiMode(bool silentUpdate = false);

        public delegate void OnUpdateAction(bool silentUpdate = false);
        public event OnUpdateAction OnUpdateMultiMode;
        //public updateMultiModeModule updateMultiMode;

        /// <summary>
        /// initializeSettings() method is for custom settings initialization, and will automatically be called from the OnStart() method unless it is overridden.
        /// This method runs before the generic ones kicks in
        /// </summary>
        protected abstract void initializeSettings();


        /// <summary>
        /// initialize() is the internal method for loading all initialization methods
        /// </summary>
        private void initialize()
        {
            if (!_settingsInitialized)
            {
                initializeSettings();
                //Handle ModuleAnimationGroup
                initializeModuleAnimationGroup();
                initializeGUI();
                //initializeEventSubscriptions();       //UI_CHOOSEOPTION is not visible on EVA :o(

                _settingsInitialized = true;
            }
        }

        protected void initializeEventSubscriptions()
        {
            //if (externalToEVAOnly)
            //{
                if (GameEvents.onCrewOnEva != null) GameEvents.onCrewOnEva.Add(onEVA);
                if (GameEvents.onCrewBoardVessel != null) GameEvents.onCrewBoardVessel.Add(onEVABoard);
                if (GameEvents.onVesselSwitching != null) GameEvents.onVesselSwitching.Add(onVesselSwitching);
            //}
        }

        #region UI
        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "MultiMode")]
        [UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, scene = UI_Scene.All, suppressEditorShipModified = false, options = new[] { "None" }, display = new[] { "None" })]
        public string ChooseOption = string.Empty;

        [KSPEvent(name = "EVAChangeMode", guiName = "Change mode", active = true, externalToEVAOnly = true, guiActiveUnfocused = true, unfocusedRange = 5f, guiActive = false, guiActiveEditor = false)]
        public void EVAChangeMode()
        {
            ActionNextMode();
        }

        protected virtual void initializeGUI()
        {
            GTIDebug.Log("GTI_MultiMode: initializeGUI() --> start", iDebugLevel.DebugInfo);
            if (!_GUIsettingsInitialized)
            {
                BaseField chooseField;
                string[] Options;
                string[] OptionsDisplay;

                GTIDebug.Log("GTI_MultiMode: chooseField", iDebugLevel.DebugInfo);
                chooseField = Fields[nameof(ChooseOption)];
                chooseField.guiName = "Mode";
                chooseField.guiActiveEditor = availableInEditor;
                chooseField.guiActive = availableInFlight;

                //Detect state of ModuleAnimationGroup and set UI accordingly
                if (useModuleAnimationGroup && _MAGsettingsInitialized)
                {
                    GTIDebug.Log("Detect ModuleAnimationGroup state, and adjust UI accordingly", iDebugLevel.DebugInfo);
                    //Override the converter selection UI is useModuleAnimationGroup is true
                    chooseField.guiActiveEditor = MAG.isDeployed ? availableInEditor : false;
                    chooseField.guiActive = MAG.isDeployed ? availableInFlight : false;
                }

                //Extract options from the engineList
                GTIDebug.Log("GTI_MultiMode: Set Options & OptionsDisplay", iDebugLevel.DebugInfo);
                Options = new string[modes.Count];
                OptionsDisplay = new string[modes.Count];
                for (int i = 0; (i < modes.Count); i++)
                {
                    //Debug.Log("GTI_MultiModeConverter: i --> " + i);
                    Options[i] = modes[i].ID;
                    OptionsDisplay[i] = modes[i].Name;
                }
                //If there is only one mode available, then hide the selector menu --> It yields null ref errors if used in flight!!!
                //Debug.Log("engineList.Count: " + engineList.Count);
                if (modes.Count < 2)
                {
                    chooseField.guiActive = false;
                    chooseField.guiActiveEditor = false;
                }

                GTIDebug.Log("GTI_MultiMode: Set .options & .display & .onFieldChanged", iDebugLevel.DebugInfo);
                UI_ChooseOption chooseOption = HighLogic.LoadedSceneIsFlight ? chooseField.uiControlFlight as UI_ChooseOption : chooseField.uiControlEditor as UI_ChooseOption;
                chooseOption.options = Options;
                chooseOption.display = OptionsDisplay;
                chooseOption.onFieldChanged = selectMode;
                chooseOption.affectSymCounterparts = affectSymCounterpartsInFlight ? UI_Scene.All : UI_Scene.Editor;

                //Update Actions GUI texts and hide the ones not applicable
                initializeActions();

                //Load the previous selected Option, and sync up with the selectedConverter number
                //ChooseOption = selectedChooseOption;
                GTIDebug.Log("Find selected mode from ChooseOption", iDebugLevel.DebugInfo);
                selModeFromChooseOption();

                //If it's possible to switch mode in flight, then it natural that a Kerbal can do it manually as well.
                externalToEVAOnly = availableInFlight ? true : externalToEVAOnly;

                _GUIsettingsInitialized = true;

                GTIDebug.Log("GTI_MultiMode: initializeGUI() --> end", iDebugLevel.DebugInfo);
            }
        }

        /// <summary>
        /// Updates the selection the user uses the right click UI
        /// </summary>
        /// <param name="field"></param>
        /// <param name="oldValueObj"></param>
        protected virtual void selectMode(BaseField field, object oldValueObj)
        {
            //FindSelectedMode();
            
            /*updateMultiMode();*/
            

            foreach (Delegate d in OnUpdateMultiMode.GetInvocationList())
            {
                GTIDebug.Log(d.Method.ToString());
            }
            OnUpdateMultiMode.Invoke();
        }


        //public virtual List<T> GetCounterPartModules(Part thispart)
        //{
        //    List<Part> CounterParts = thispart.symmetryCounterparts;
        //    List<T> modules = new List<T>(CounterParts.Count);

        //    foreach (Part part in CounterParts)
        //    {
        //        modules.Add(part.FindModuleImplementing<T>());
        //    }

        //    return modules;
        //}

        /// <summary>
        /// Derives the selected converter as integer from the ChooseOption value
        /// </summary>
        protected virtual void FindSelectedMode()
        {
            for (int i = 0; i < modes.Count; i++)
            {
                if (modes[i].ID == ChooseOption)
                {
                    selectedMode = i;
                    //this.Fields["EVAChangeMode"].guiName = "Change mode: " + mode[selectedMode].Name;
                    this.Events[nameof(EVAChangeMode)].guiName = "Change mode: " + modes[selectedMode].Name;

                    ////Sync up all counterparts modules
                    //if (affectSymCounterpartsInFlight)
                    //{
                    //    List<GTI_MultiMode<T>> CounterModules = CounterPartModules(this.part);
                    //    foreach (GTI_MultiMode<T> module in CounterModules)
                    //    {
                    //        module.ChooseOption = ChooseOption;
                    //    }
                    //}

                    return;
                }
            }
            GTIDebug.Log("FindSelectedMode() was unsuccessful in locating current mode: " + ChooseOption, iDebugLevel.DebugInfo);
        }
        private void FindSelectedMode(bool boo = false) { FindSelectedMode(); }

        /// <summary>
        /// selModeFromChooseOption set selectedMode from ChooseOption.
        /// If ChooseOption is empty, then the first mode in moduleList is returned.
        /// Dependent on: ChooseOption, selectedMode, mode.ID
        /// </summary>
        protected virtual void selModeFromChooseOption()
        {
            GTIDebug.Log("selModeFromChooseOption() start", iDebugLevel.DebugInfo);
            if (ChooseOption == string.Empty)
            {
                GTIDebug.Log("selModeFromChooseOption() --> ChooseOption == string.Empty", iDebugLevel.DebugInfo);
                ChooseOption = modes[0].ID;
                selectedMode = 0;
                return;
            }
            else
            {
                for (int i = 0; i < modes.Count; i++)
                {
                    if (ChooseOption == modes[i].ID)
                    {
                        GTIDebug.Log("selModeFromChooseOption() --> ChooseOption == mode[i].ID", iDebugLevel.DebugInfo);
                        selectedMode = i;
                        return;
                    }
                }
                //If mode not found, revert to first setting
                GTIDebug.Log("selModeFromChooseOption() --> Default", iDebugLevel.DebugInfo);
                ChooseOption = modes[0].ID;
                selectedMode = 0;
            }
        }


        /// <summary>
        /// This is where you implement the on screen message function which you can then call in you updateMultiMode method like "if (silentUpdate == false) writeScreenMessage();" 
        /// </summary>
        protected abstract void writeScreenMessage();
        protected virtual void writeScreenMessage(string Message, float duration = 3f, string messagePosition = "UPPER_RIGHT")
        {
            //Default position and switch to user defined position
            ScreenMessageStyle position = ScreenMessageStyle.UPPER_RIGHT;
            switch (messagePosition)
            {
                case "UPPER_CENTER":
                    position = ScreenMessageStyle.UPPER_CENTER;
                    break;
                case "UPPER_RIGHT":
                    position = ScreenMessageStyle.UPPER_RIGHT;
                    break;
                case "UPPER_LEFT":
                    position = ScreenMessageStyle.UPPER_LEFT;
                    break;
                case "LOWER_CENTER":
                    position = ScreenMessageStyle.LOWER_CENTER;
                    break;
            }
            writeScreenMessage(Message, duration, position);
        }
        protected void writeScreenMessage(string Message, float duration = 3f, ScreenMessageStyle messagePosition = ScreenMessageStyle.UPPER_RIGHT)
        {
            ScreenMessages.PostScreenMessage(Message, duration, messagePosition);
        }
        #endregion

        #region Actions
        public virtual void initializeActions()
        {
            for (int i = 1; i <= numberOfSpecificActions; i++)
            {
                //if (MultiModeID.Length != MultiModeNames.Length) GTIDebug.LogError("MultiMode class implementation failed. MultiModeID and MultiModeNames arrays are inconsistent. Please fix the issue.");
                if (modes.Count < i)
                { this.Actions["MultiModeAction_" + i].active = false; }
                else
                { this.Actions["MultiModeAction_" + i].guiName = "Activate: " + modes[i - 1].Name; }
            }
        }

        //Specific actions
        protected const int numberOfSpecificActions = 12;
        [KSPAction("Set #1")]
        public void MultiModeAction_1(KSPActionParam param) { MultiModeAction(0); }

        [KSPAction("Set #2")]
        public void MultiModeAction_2(KSPActionParam param) { MultiModeAction(1); }

        [KSPAction("Set #3")]
        public void MultiModeAction_3(KSPActionParam param) { MultiModeAction(2); }

        [KSPAction("Set #4")]
        public void MultiModeAction_4(KSPActionParam param) { MultiModeAction(3); }

        [KSPAction("Set #5")]
        public void MultiModeAction_5(KSPActionParam param) { MultiModeAction(4); }

        [KSPAction("Set #6")]
        public void MultiModeAction_6(KSPActionParam param) { MultiModeAction(5); }

        [KSPAction("Set #7")]
        public void MultiModeAction_7(KSPActionParam param) { MultiModeAction(6); }

        [KSPAction("Set #8")]
        public void MultiModeAction_8(KSPActionParam param) { MultiModeAction(7); }

        [KSPAction("Set #9")]
        public void MultiModeAction_9(KSPActionParam param) { MultiModeAction(8); }

        [KSPAction("Set #10")]
        public void MultiModeAction_10(KSPActionParam param) { MultiModeAction(9); }

        [KSPAction("Set #11")]
        public void MultiModeAction_11(KSPActionParam param) { MultiModeAction(10); }

        [KSPAction("Set #12")]
        public void MultiModeAction_12(KSPActionParam param) { MultiModeAction(11); }

        protected virtual void MultiModeAction(int inActionSelect)
        {
            GTIDebug.Log("Action ActionPropulsion_" + inActionSelect + " (before): " + ChooseOption, iDebugLevel.Medium);

            //Check if the selected mode is possible
            if (inActionSelect < modes.Count)
            {
                //Check if the selected mode is a change
                if (!(selectedMode == inActionSelect))
                {
                    selectedMode = inActionSelect;

                    ChooseOption = modes[selectedMode].ID;        //converterNames[selectedConverter];
                    GTIDebug.Log("MultiModeAction_" + inActionSelect + " Executed", iDebugLevel.DebugInfo);

                    //updateMultiMode();
                    OnUpdateMultiMode();
                }
            }
            GTIDebug.Log("Action MultiModeAction_" + inActionSelect + " (after): " + ChooseOption, iDebugLevel.Medium);
        }

        [KSPAction("Next mode")]
        public void ActionNextMode(KSPActionParam param) { ActionNextMode(); }
        public virtual void ActionNextMode()
        {
            selectedMode++;
            if (selectedMode > modes.Count - 1) { selectedMode = 0; }
            ChooseOption = modes[selectedMode].ID;
            //updateMultiMode();
            OnUpdateMultiMode();
        }
        [KSPAction("Previous mode")]
        public void ActionPreviousMode(KSPActionParam param) { ActionPreviousMode(); }
        public virtual void ActionPreviousMode()
        {
            selectedMode--;
            //Check if selected proplusion was the first one, and return the last one instead
            if (selectedMode < 0) { selectedMode = modes.Count - 1; }
            ChooseOption = modes[selectedMode].ID;
            //updateMultiMode();
            OnUpdateMultiMode();
        }
        #endregion

        #region ModuleAnimationGroup
        //Comment: Using "OnAnimationGroupStateChanged" event, I could probably further simplify the logics and remove 
        //the overriding of the "ModuleAnimationGroup" functionality
        protected virtual void initializeModuleAnimationGroup()
        {
            //Debug.Log("useModuleAnimationGroup: " + useModuleAnimationGroup);
            if (!_MAGsettingsInitialized && useModuleAnimationGroup == true)
            {
                GTIDebug.Log("useModuleAnimationGroup: Evaluated as true", iDebugLevel.DebugInfo);
                //Animation Anim = new Animation();
                //ModuleAnimationGroup MAG = new ModuleAnimationGroup();

                MAG = part.FindModuleImplementing<ModuleAnimationGroup>();

                //Anim = part.FindModelAnimator(MAG.deployAnimationName);

                //foreach (BaseEvent e in MAG.Events)
                //{
                //    e.active = false;
                //    e.guiActive = false;
                //    e.guiActiveEditor = false;
                //}
                //foreach (BaseAction a in MAG.Actions)
                //{
                //    a.active = false;
                //}

                //GTIDebug.Log("Activate 'ModuleAnimationGroupEvent'", iDebugLevel.DebugInfo);

                //Activate event in this module to trigger animations instead
                //this.Events["ModuleAnimationGroupEvent"].active = true;
                //this.Events["ModuleAnimationGroupEvent"].guiActive = true;
                //this.Events["ModuleAnimationGroupEvent"].guiActiveEditor = true;
                //Patch 2018-03-14 to fix the button showing even when module is not deployed.
                GTIDebug.Log("initializeModuleAnimationGroup: !MAG.isDeployed " + !MAG.isDeployed, iDebugLevel.DebugInfo);
                if (!MAG.isDeployed)
                {
                    ModuleAnimationGroupEvent_DisableModules();
                }
                    
                //Register the event of Animation Groups
                if (GameEvents.OnAnimationGroupStateChanged != null && MAG != null)
                {
                    GTIDebug.Log("Subscribing to OnAnimationGroupStateChanged", iDebugLevel.DebugInfo);
                    GameEvents.OnAnimationGroupStateChanged.Add(OnModuleAnimationGroupStateChanged);
                }

                //?? Add check if the MAG isDeployed? Adjust UI accordingly?

                _MAGsettingsInitialized = true;
            }
        }

        /*[KSPEvent(active = false, guiActive = false, guiActiveEditor = false, guiName = "Deploy")]
        public void ModuleAnimationGroupEvent()
        {
            float AnimLength;

            #region tests
            //Debug.Log("ModuleAnimationGroupEvent fired");

            //Debug.Log("Anim.isPlaying " + Anim.isPlaying.ToString());
            #endregion

            try { AnimLength = Anim.clip.length; } catch { AnimLength = 1f; }

            BaseField chooseField = Fields[nameof(ChooseOption)];
            try  //We only handle the  first module
            {
                if (MAG.isDeployed)
                {
                    MAG.RetractModule();
                    chooseField.guiActive = false;
                    chooseField.guiActiveEditor = false;

                    this.Events["ModuleAnimationGroupEvent"].guiName = txtDeploy;
                    this.Events["ModuleAnimationGroupEvent"].guiActive = false;
                    //StartCoroutine(ModuleAnimationGroupEventCoroutine(AnimLength, 0.001f, Deploying: false));
                }
                else
                {
                    MAG.DeployModule();

                    //Update UI
                    //chooseField.guiActiveEditor = availableInEditor;
                    //chooseField.guiActive = availableInFlight;

                    //Debug.Log("updateConverter in 'ModuleAnimationGroupEvent'");
                    this.Events["ModuleAnimationGroupEvent"].guiName = txtRetract;
                    this.Events["ModuleAnimationGroupEvent"].guiActive = false;
                    //StartCoroutine(ModuleAnimationGroupEventCoroutine(AnimLength, 0.001f, Deploying: true));
                }
            }
            catch
            {
                //... do nothing
                GTIDebug.LogError(this.GetType().Name  + " -- ModuleAnimationGroup --- Error when handling animations");
            }
        }*/

        /*protected virtual IEnumerator ModuleAnimationGroupEventCoroutine(float starttime, float waitingtime, bool Deploying)
        {
            GTIDebug.Log("Start of ModuleAnimationGroupEventCoroutine()", iDebugLevel.High);
            yield return new WaitForSeconds(starttime);
            int _InvokeCounter = 0;
            while (_InvokeCounter++ < 600)
            {
                GTIDebug.Log("Coroutine Looping while animation is playing: " + _InvokeCounter, iDebugLevel.DebugInfo);
                if (Anim.isPlaying == false)
                {
                    GTIDebug.Log("'Animation finished playing' --> update Mode");
                    if (Deploying)
                    {
                        this.Fields["ChooseOption"].guiActiveEditor = availableInEditor;
                        this.Fields["ChooseOption"].guiActive = availableInFlight;
                        updateMultiMode(silentUpdate: true);
                    }
                    else
                    {
                        ModuleAnimationGroupEvent_DisableModules();
                    }

                    //Reactivate the gui button
                    this.Events["ModuleAnimationGroupEvent"].guiActive = true;
                    GTIDebug.Log("Coroutine should stop here", iDebugLevel.DebugInfo);
                    break;
                }
                else
                {
                    GTIDebug.Log("Coroutine will wait for " + waitingtime + " sec and run again", iDebugLevel.DebugInfo);
                    yield return new WaitForSeconds(waitingtime);
                    GTIDebug.Log(this.GetType().Name + " -- Coroutine have waited for " + waitingtime + " sec and continue", iDebugLevel.DebugInfo);
                }
            }
            //Reactivate the gui button if waiting failed
            if (_InvokeCounter >= 600)
            {
                this.Fields["ChooseOption"].guiActive = true;
                this.Events["ModuleAnimationGroupEvent"].guiActive = true;
                GTIDebug.LogError("ModuleAnimationGroupEventCoroutine failed to finish successfully");
            }
        }*/

        protected virtual void OnModuleAnimationGroupStateChanged(ModuleAnimationGroup module, bool Deploying)
        {
            GTIDebug.Log("Animation finished 'OnAnimationGroupStateChanged' --> update Mode", iDebugLevel.High);

            if (module != null && module.part != part) { GTIDebug.Log("triggering part is not this part", iDebugLevel.DebugInfo); return; }
            
            if (Deploying)
            {
                this.Fields["ChooseOption"].guiActiveEditor = availableInEditor;
                this.Fields["ChooseOption"].guiActive = availableInFlight;
                //updateMultiMode(silentUpdate: true);
                OnUpdateMultiMode(silentUpdate: true);
            }
            else
            {
                this.Fields["ChooseOption"].guiActiveEditor = false;
                this.Fields["ChooseOption"].guiActive = false;
                //externalToEVAOnly
                ModuleAnimationGroupEvent_DisableModules();
            }

            //Reactivate the gui button
            //this.Events["ModuleAnimationGroupEvent"].guiActive = true;
            GTIDebug.Log("OnAnimationGroupStateChanged executed", iDebugLevel.DebugInfo);
        }

        protected abstract void ModuleAnimationGroupEvent_DisableModules();
        #endregion

        #region VAB Information
        public override string GetInfo()
        {
            try
            {
                return this.GetType().Name;
            }
            catch (Exception e)
            {
                GTIDebug.LogError(this.GetType().Name + " GetInfo() Error " + e.Message);
                throw;
            }
        }
        #endregion

        protected void onEVA(GameEvents.FromToAction<Part, Part> FromToData)
        {
            //Consistency Check
            GTIDebug.Log("onEVA", iDebugLevel.DebugInfo);
            if (FromToData.to == null || FromToData.from == null) return;
            GTIDebug.Log("Vessel: " + this.vessel.vesselName + "fromVessel: " + FromToData.from.vessel.vesselName + "toVessel: " + FromToData.to.vessel.vesselName, iDebugLevel.DebugInfo);
            GTIDebug.Log("isEVA: " + FromToData.to.vessel.isEVA, iDebugLevel.DebugInfo);

            //if (vessel.isEVA)
            //if (FlightGlobals.ActiveVessel.vesselType == VesselType.EVA)

        }

        protected void onEVABoard(GameEvents.FromToAction<Part, Part> FromToData)
        {
            //Consistency Check
            GTIDebug.Log("onEVABoard", iDebugLevel.DebugInfo);
            if (FromToData.to == null || FromToData.from == null) return;
            GTIDebug.Log("Vessel: " + this.vessel.vesselName + "fromVessel: " + FromToData.from.vessel.vesselName + "toVessel: " + FromToData.to.vessel.vesselName, iDebugLevel.DebugInfo);
            GTIDebug.Log("isEVA: " + FromToData.to.vessel.isEVA, iDebugLevel.DebugInfo);
        }

        private void onVesselSwitching(Vessel fromVessel, Vessel toVessel)
        {
            //Consistency Check
            GTIDebug.Log("onVesselSwitching", iDebugLevel.DebugInfo);
            if (toVessel == null || fromVessel == null) return;
            GTIDebug.Log("isEVA: " + toVessel.isEVA, iDebugLevel.DebugInfo);
        }

        protected virtual void OnDestroy()
        {
            try
            {
                if (this.vessel.vesselName != null)
                    GTIDebug.Log(this.vessel.vesselName + " --> " + this.GetType().Name + " --> OnDestroy()");
            }
            catch { /* DO NOTHING */ }
            

            if (GameEvents.OnAnimationGroupStateChanged != null) GameEvents.OnAnimationGroupStateChanged.Remove(OnModuleAnimationGroupStateChanged);
            if (GameEvents.onCrewOnEva != null) GameEvents.onCrewOnEva.Remove(onEVA);
            if (GameEvents.onCrewBoardVessel != null) GameEvents.onCrewBoardVessel.Remove(onEVABoard);
            if (GameEvents.onVesselSwitching != null) GameEvents.onVesselSwitching.Remove(onVesselSwitching);
        }
    }
}