/*
using System;
using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    class UI_TEST : PartModule
    {

        //[UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, options = new string[] { "1", "2", "3" }, display = new string[] { "one", "two", "three" }, scene = UI_Scene.Editor, suppressEditorShipModified = true)]

        [KSPField(guiActiveEditor = true, isPersistant = true, guiName = "Subtype", guiActive = true)]
        [UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, scene = UI_Scene.All)]
        public int ChooseOption = 0;

        public override void OnStart(PartModule.StartState state)
        {
            var chooseField = Fields[nameof(ChooseOption)];
            chooseField.guiName = "Choose Something";

            var chooseOption = chooseField.uiControlEditor as UI_ChooseOption;
            chooseOption.options = new string[] { "one", "two", "three" };

            chooseOption.onFieldChanged = UpdateFromGUI;

            //var chooseOption = chooseField.uiControlEditor as UI_ChooseOption;
            //chooseOption.options = subtypes.Select(s => s.title).ToArray();
        }

        private void UpdateFromGUI(BaseField field, object oldValueObj)
        {
            //int oldValue = (int)oldValueObj; // If you actually care about it
                                             //UpdateSubtype();
                                             //GameEvents.onEditorShipModified.Fire(EditorLogic.fetch.ship);

            Debug.Log("New ChooseOptions selected: " + ChooseOption);
        }




    }
}
*/