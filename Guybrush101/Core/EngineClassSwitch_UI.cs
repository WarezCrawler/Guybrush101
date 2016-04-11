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
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Propellants")]
        private string GUIpropellantNames = String.Empty;
        [KSPField]
        public string iniGUIpropellantNames = string.Empty;

        [KSPField(isPersistant = true)]
        public int selectedPropellant = 0;                                             //holds the selected propellant setup.

        //Availability of the functionality
        [KSPField]
        public bool availableInFlight = false;
        [KSPField]
        public bool availableInEditor = true;

        //private bool RightClickUI_onoff = false;
        #endregion

        #region User_Interface

        
        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "EngineSwitcher")]
        [UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, scene = UI_Scene.All, suppressEditorShipModified = false, options = new[] { "None" })]
        public string ChooseOption = "0";
        //private string[] Options;
        //private string[] OptionsDisplay;
        //BaseField chooseField;
        //UI_ChooseOption chooseOption;

        private void initializeGUI()
        {
            BaseField chooseField;
            string[] Options;
            string[] OptionsDisplay;

            //Debug.Log("initializeGUI(): START");
            chooseField                     = Fields[nameof(ChooseOption)];
            chooseField.guiName             = "Propellants";
            chooseField.guiActiveEditor     = availableInEditor;
            chooseField.guiActive           = availableInFlight;

            //Debug.Log("initializeGUI() | arrPropellantNames.Length: " + arrPropellantNames.Length);

            //Create array Options that are simple ref's to the propellant list
            Debug.Log(
                "propList.Count: " + propList.Count
                );
            Options = new string[propList.Count];        //Options = new string[arrPropellantNames.Length];
            OptionsDisplay = new string[propList.Count];
            for (int i = 0; (i < propList.Count) ; i++)
            {
                Debug.Log(
                    "\ni: " + i +
                    "\npropList[i].Propellants: " + propList[i].Propellants
                    );

                //Debug.Log("Tech basicRocketry: " + ResearchAndDevelopment.GetTechnologyState("basicRocketry"));
                //if (ResearchAndDevelopment.GetTechnologyState(propList[i].requiredTech) == RDTech.State.Available)
                //if (propList[i].EngineConfigAvailable)
                //{
                    Debug.Log("Add " + propList[i].Propellants + " (" + i + ") to UI");
                    Options[i] = i.ToString();
                    OptionsDisplay[i] = propList[i].Propellants;
                //}
            }
            
            
            //Set which function run's when changing selection, which options, and the text to display
            //var chooseOptionEditor = chooseField.uiControlEditor as UI_ChooseOption;
            UI_ChooseOption chooseOption = HighLogic.LoadedSceneIsFlight ? chooseField.uiControlFlight as UI_ChooseOption : chooseField.uiControlEditor as UI_ChooseOption;
            chooseOption.options = Options;
            chooseOption.display = OptionsDisplay;              //arrPropellantNames;        //Should be GUInames array
            chooseOption.onFieldChanged = selectPropellant;
        }

        //onFieldChanged action
        private void selectPropellant(BaseField field, object oldValueObj)
        {
            selectedPropellant = int.Parse(ChooseOption);
            //chooseField.guiName = propList[selectedPropellant].Propellants;
            updateEngineModule(true);
        }
        #endregion
        
        
        [KSPAction("Next propellant")]
        public void nextPropellantAction(KSPActionParam param)
        {
            //nextPropellantEvent();

            selectedPropellant++;
            if (selectedPropellant > (propList.Count - 1))                           //arrPropellantNames.GetUpperBound(0))
            {
                //if we move from last propellant, then the next one is the first one - aka 0
                selectedPropellant = 0;
            }
            ChooseOption = selectedPropellant.ToString();
            updateEngineModule(true);

        }
        [KSPAction("Previous propellant")]
        public void previousPropellantAction(KSPActionParam param)
        {
            //previousPropellantEvent();
            selectedPropellant--;
            if (selectedPropellant < 0)
            {
                //if we move from the first propellant, then the previous is the last - aka the upperbound
                selectedPropellant = (propList.Count-1);                           //arrPropellantNames.GetUpperBound(0))
            }
            ChooseOption = selectedPropellant.ToString();
            updateEngineModule(true);
        }
        
    }
}
