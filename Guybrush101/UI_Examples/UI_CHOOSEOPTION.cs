/*
using System;
using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    class UI_TEST : PartModule
    {
        //[KSPField(guiActiveEditor = true, isPersistant = true, guiName = "Subtype")]
        //[UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, options = new[] { "None" }, scene = UI_Scene.Editor, suppressEditorShipModified = true)]
        //public int subtypeIndexControl = 0;

        //[UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, options = new string[] { "1", "2", "3" }, display = new string[] { "one", "two", "three" }, scene = UI_Scene.Editor, suppressEditorShipModified = true)]

        [KSPField(guiName = "Choose me", guiActiveEditor = true, isPersistant = true, guiActive = true)]
        [UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, scene = UI_Scene.All, display = new string[] { "one", "two", "three" })]  //, options = new string[] { "1", "2", "3" }
        public string ChooseOption = "1";

        public override void OnStart(PartModule.StartState state)
        {
            bool OptionInArray = false;

            var chooseField = Fields["ChooseOption"];          //Fields[nameof(ChooseOption)];
            chooseField.guiName = "Choose Something";

            var array = new string[3] { "1", "2", "3" };

            var chooseOptionEditor = chooseField.uiControlEditor as UI_ChooseOption;
            chooseOptionEditor.options = array;
            chooseOptionEditor.onFieldChanged = UpdateFromGUI;

            var chooseOptionFlight = chooseField.uiControlFlight as UI_ChooseOption;
            chooseOptionFlight.options = array;
            chooseOptionFlight.onFieldChanged = UpdateFromGUI;

            foreach( string item in array)
            {
                OptionInArray = (ChooseOption == item) ? true : false;
                if (OptionInArray) { break; }
            }
            if (!OptionInArray) { ChooseOption = array[0]; }
            
            //var chooseOption = chooseField.uiControlEditor as UI_ChooseOption;
            //chooseOption.options = subtypes.Select(s => s.title).ToArray();
        }

        private void UpdateFromGUI(BaseField field, object oldValueObj)
        {
            //int oldValue = (int)oldValueObj; // If you actually care about it
            //UpdateSubtype();
            //GameEvents.onEditorShipModified.Fire(EditorLogic.fetch.ship);
            Debug.Log("old ChooseOptions selected: " + oldValueObj.ToString());
            Debug.Log("New ChooseOptions selected: " + ChooseOption);
            Debug.Log(field.ToString() + " | " + field.name);
            Fields[nameof(ChooseOption)].guiName = "Choose Something else";
            
        }




    }
}
*/
/*




//[KSPField(guiActiveEditor = true, isPersistant = true, guiName = "Subtype")]
//[UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, options = new[] { "None" }, scene = UI_Scene.Editor, suppressEditorShipModified = true)]
//public int subtypeIndexControl = 0;



*/