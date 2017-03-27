//using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;
//using System.Threading;
using GTI.Config;
using static GTI.Config.GTIConfig;
//using System.Threading.Tasks;     //Needs .Net 4.0+
//using GTI.GenericFunctions;
//using System;

namespace GTI
{
    partial class GTI_MultiModeConverter : PartModule
    {
        #region KSPFields and supporting settings

        /// <summary>
        /// Currently selected converter in integer format for use in the arrays
        /// </summary>
        public int selectedConverter = 0;

        //[KSPField(isPersistant = true)]
        //public string selectedChooseOption = string.Empty;

        //Availability of the functionality
        [KSPField]
        public bool availableInFlight = false;
        [KSPField]
        public bool availableInEditor = true;

        private int _InvokeCounter = 0;
        #endregion

        #region User_Interface

        [KSPField]
        public string messagePosition = string.Empty;

        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "MultiModeConverter")]
        [UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, scene = UI_Scene.All, suppressEditorShipModified = false, options = new[] { "None" }, display = new[] { "None" })]
        public string ChooseOption = string.Empty;

        #endregion

        /// <summary>
        /// Initializes setting for the UI of the multimodeconverter
        /// </summary>
        private void initializeGUI()
        {
            //Debug.Log("GTI_MultiModeConverter: initializeGUI() --> start");

            BaseField chooseField;
            string[] Options;
            string[] OptionsDisplay;

            //Debug.Log("GTI_MultiModeConverter: chooseField");
            chooseField = Fields[nameof(ChooseOption)];
            chooseField.guiName = "Converters";
            chooseField.guiActiveEditor = availableInEditor;
            chooseField.guiActive = availableInFlight;
            if (useModuleAnimationGroup == true)
            {
                //Override the converter selection UI is useModuleAnimationGroup is true
                chooseField.guiActiveEditor = MAG.isDeployed ? availableInEditor : false;
                chooseField.guiActive = MAG.isDeployed ? availableInFlight : false;
            }


            //Extract options from the engineList
            //Debug.Log("GTI_MultiModeConverter: Set Options & OptionsDisplay");
            Options = new string[converterNames.Length];
            OptionsDisplay = new string[converterNames.Length];
            for (int i = 0; (i < converterNames.Length); i++)
            {
                //Debug.Log("GTI_MultiModeConverter: i --> " + i);
                Options[i] = converterNames[i];
                OptionsDisplay[i] = converterNames[i];
            }
            //If there is only one converter available, then hide the selector menu --> It yields null ref errors if used in flight!!!
            //Debug.Log("engineList.Count: " + engineList.Count);
            if (converterNames.Length < 2)
            {
                chooseField.guiActive = false;
                chooseField.guiActiveEditor = false;
            }

            //Debug.Log("GTI_MultiModeConverter: Set .options & .display & .onFieldChanged");
            UI_ChooseOption chooseOption = HighLogic.LoadedSceneIsFlight ? chooseField.uiControlFlight as UI_ChooseOption : chooseField.uiControlEditor as UI_ChooseOption;
            chooseOption.options = Options;
            chooseOption.display = OptionsDisplay;
            chooseOption.onFieldChanged = selectConverter;

            //Update Actions GUI texts and hide the ones not applicable
            initializeActions();

            //GameEvents.OnAnimationGroupStateChanged.Add(SetModuleState);

            //Load the previous selected Option, and sync up with the selectedConverter number
            //ChooseOption = selectedChooseOption;
            selConverterFromChooseOption();

            //Debug.Log("GTI_MultiModeConverter: initializeGUI() --> end");
        }

        /// <summary>
        /// Updates the selection the user uses the right click UI
        /// </summary>
        /// <param name="field"></param>
        /// <param name="oldValueObj"></param>
        private void selectConverter(BaseField field, object oldValueObj)
        {
            //Debug.Log("GTI_MultiModeConverter: User switches converter");
            updateConverter();
        }
        /// <summary>
        /// Derives the selected converter as integer from the ChooseOption value
        /// </summary>
        private void FindSelectedConverter()
        {
            for (int i = 0; i < converterNames.Length; i++)
            {
                if (converterNames[i] == ChooseOption) { selectedConverter = i; return; }
            }
        }
        private void writeScreenMessage()
        {
            //string strOutInfo = string.Empty;
            StringBuilder strOutInfo = new StringBuilder();

            strOutInfo.AppendLine("Converter mode changed to " + converterNames[selectedConverter]);
            strOutInfo.AppendLine("Inputs:");
            foreach (ResourceRatio input in MRC[selectedConverter].Recipe.Inputs)
            {
                strOutInfo.AppendLine(input.ResourceName + " (" + input.Ratio + ")");
            }
            strOutInfo.AppendLine("Outputs:");
            foreach (ResourceRatio output in MRC[selectedConverter].Recipe.Outputs)
            {
                strOutInfo.AppendLine(output.ResourceName + " (" + output.Ratio + ")");
            }

            GTIDebug.Log("\nGTI_MultiModeConverter:\n" + strOutInfo.ToString(), iDebugLevel.DebugInfo);

            //Default position and switch to user defined position
            ScreenMessageStyle position = ScreenMessageStyle.UPPER_CENTER;
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

            writeScreenMessage(
                Message: strOutInfo.ToString(),
                position: position,
                duration: 3f
                );
        }
        private void writeScreenMessage(ScreenMessageStyle position, string Message, float duration = 1.5f)
        {
            ScreenMessages.PostScreenMessage(Message, duration, position);
        }

        #region Actions converter configuration selections
        [KSPAction("Toggle Resource Converter")]
        public void ActionToggleConverter(KSPActionParam param)
        {
            for (int i = 0; i < MRC.Count; i++)
            {
                if (MRC[i].isEnabled)
                {
                    MRC[i].ToggleResourceConverterAction(param);
                }
            }
        }

        [KSPAction("Next Resource Converter")]
        public void ActionNextConverter(KSPActionParam param)
        {
            selectedConverter++;
            if (selectedConverter > converterNames.Length - 1) { selectedConverter = 0; }
            ChooseOption = converterNames[selectedConverter];
            updateConverter();
        }
        [KSPAction("Previous Resource Converter")]
        public void ActionPreviousConverter(KSPActionParam param)
        {
            selectedConverter--;
            //Check if selected proplusion was the first one, and return the last one instead
            if (selectedConverter < 0) { selectedConverter = converterNames.Length - 1; }
            ChooseOption = converterNames[selectedConverter];

            updateConverter();
        }

        //Specific actions
        private const int numberOfSpecificActions = 12;
        private void initializeActions()
        {
            for (int i = 1; i <= numberOfSpecificActions; i++)
            {
                //Debug.Log("GTI_MultiModeConverter: create ActionConverter_" + i);
                if (converterNames.Length < i)
                { this.Actions["ActionConverter_" + i].active = false; }
                else
                { this.Actions["ActionConverter_" + i].guiName = "Activate: " + converterNames[i - 1]; }
            }
        }

        [KSPAction("Set Converter #1")]
        public void ActionConverter_1(KSPActionParam param) { ActionConverter(0); }

        [KSPAction("Set Converter #2")]
        public void ActionConverter_2(KSPActionParam param) { ActionConverter(1); }

        [KSPAction("Set Converter #3")]
        public void ActionConverter_3(KSPActionParam param) { ActionConverter(2); }

        [KSPAction("Set Converter #4")]
        public void ActionConverter_4(KSPActionParam param) { ActionConverter(3); }

        [KSPAction("Set Converter #5")]
        public void ActionConverter_5(KSPActionParam param) { ActionConverter(4); }

        [KSPAction("Set Converter #6")]
        public void ActionConverter_6(KSPActionParam param) { ActionConverter(5); }

        [KSPAction("Set Converter #7")]
        public void ActionConverter_7(KSPActionParam param) { ActionConverter(6); }

        [KSPAction("Set Converter #8")]
        public void ActionConverter_8(KSPActionParam param) { ActionConverter(7); }

        [KSPAction("Set Converter #9")]
        public void ActionConverter_9(KSPActionParam param) { ActionConverter(8); }

        [KSPAction("Set Converter #10")]
        public void ActionConverter_10(KSPActionParam param) { ActionConverter(9); }

        [KSPAction("Set Converter #11")]
        public void ActionConverter_11(KSPActionParam param) { ActionConverter(10); }

        [KSPAction("Set Converter #12")]
        public void ActionConverter_12(KSPActionParam param) { ActionConverter(11); }

        private void ActionConverter(int inActionSelect)
        {
            //Debug.Log("Action ActionPropulsion_" + inActionSelect + " (before): " + ChooseOption);

            //Check if the selected Propulsion is possible
            if (inActionSelect < converterNames.Length)
            {
                if (!(selectedConverter == inActionSelect))
                {
                    selectedConverter = inActionSelect;

                    ChooseOption = converterNames[selectedConverter];
                    GTIDebug.Log("ActionPropulsion_" + inActionSelect + " Executed", iDebugLevel.DebugInfo);
                    updateConverter();
                }
            }
            //Debug.Log("Action ActionPropulsion_" + inActionSelect + " (after): " + ChooseOption);
        }
        /// <summary>
        /// selPropFromChooseOption set selectedPropulsion from ChooseOption.
        /// If ChooseOption is empty, then the first engine in engineList is returned.
        /// Dependent on: engineList, ChooseOption, selectedPropulsion
        /// </summary>
        private void selConverterFromChooseOption()
        {
            if (ChooseOption == string.Empty)
            {
                ChooseOption = converterNames[0];
                selectedConverter = 0;
            }
            else
            {
                for (int i = 0; i < converterNames.Length; i++)
                {
                    if (ChooseOption == converterNames[i])
                    {
                        selectedConverter = i;
                        return;
                    }

                }
            }
        }
        #endregion

        #region ModuleAnimationGroup UI
        //private float AnimLength;

        [KSPEvent(active = false, guiActive = false, guiActiveEditor = false, guiName = "Deploy")]
        public void ModuleAnimationGroupEvent()
        {
            float AnimLength;

            #region tests
            //Debug.Log("ModuleAnimationGroupEvent fired");
            //Animation Anim = part.FindModelAnimator("Deploy");            //Hardcoded animation
            //Debug.Log("Animation exists: " + (Anim != null));
            //try { Debug.Log(".length " + Anim.clip.length); } catch { Debug.LogError(".length failed"); }
            //try { Debug.Log(".name " + Anim.clip.name); } catch { Debug.LogError(".name failed"); }
            //try { Debug.Log(".apparentSpeed " + Anim.clip.apparentSpeed); } catch { Debug.LogError(".apparentSpeed failed"); }
            //try { Debug.Log(".averageDuration " + Anim.clip.averageDuration); } catch { Debug.LogError(".averageDuration failed"); }
            //try { Debug.Log(".isPlaying " + Anim.isPlaying); } catch { Debug.LogError(".isPlaying failed"); }

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

                    for (int i = 0; i < MRC.Count; i++)
                    {
                        //Debug.Log("GTI_MultiModeConverter: Deactivate Converter Module [" + i + "] --> " + MRC[i].ConverterName);
                        //Deactivate the converter
                        MRC[i].DisableModule();
                        //Stop the converter
                        MRC[i].StopResourceConverter();
                    }
                }
                else
                {
                    MAG.DeployModule();

                    //Update UI
                    chooseField.guiActiveEditor = availableInEditor;
                    chooseField.guiActive = availableInFlight;

                    //Debug.Log("updateConverter in 'ModuleAnimationGroupEvent'");
                    //StartCoroutine(ModuleAnimationGroupEventCoroutine(AnimLength, 0.05f));
                    StartCoroutine(ModuleAnimationGroupEventCoroutine(AnimLength, 0.001f));
                    //Thread parallelThread = new Thread(() => threadExecution(AnimLength, 0.001f));
                    //parallelThread.Start();
                }
            }
            catch
            {
                //... do nothing
                GTIDebug.LogError("GTI_MultiModeEngine -- ModuleAnimationGroup --- Error when handling animations");
                //this.Events["ModuleAnimationGroupEvent"].active = false;
                //this.Events["ModuleAnimationGroupEvent"].guiActive = false;
                //this.Events["ModuleAnimationGroupEvent"].guiActiveEditor = false;
            }
        }

        //private void threadExecution()
        //private void threadExecution(float starttime, float waitingtime)
        //{
        //    //StartCoroutine(ModuleAnimationGroupEventCoroutine(AnimLength, 0.005f));
        //    StartCoroutine(ModuleAnimationGroupEventCoroutine(starttime, waitingtime));
        //}

        private IEnumerator ModuleAnimationGroupEventCoroutine(float starttime, float waitingtime)
        {
            //Debug.Log("Start of Coroutine");
            yield return new WaitForSeconds(starttime);
            _InvokeCounter = 0;
            while (_InvokeCounter++ < 600)
            {
                GTIDebug.Log("Coroutine Looping " + _InvokeCounter, iDebugLevel.DebugInfo);

                //Debug.Log("Invoking in 'ModuleAnimationGroupEventInvoke'");
                if (Anim.isPlaying == false)
                {
                    //Debug.Log("'ModuleAnimationGroupEventInvoke': not deployed");
                    updateConverter(silentUpdate: true);

                    //Debug.Log("Coroutine should stop here");
                    break;
                }
                else
                {
                    //Debug.Log("Coroutine will wait for " + waitingtime + " sec and run again");
                    yield return new WaitForSeconds(waitingtime);
                    //Debug.Log("Coroutine have waited for " + waitingtime + " sec and continue");
                }
            }
        }
        #endregion
    }
}