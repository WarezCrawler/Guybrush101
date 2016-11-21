/*
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    class GTI_ISRUSwitch : PartModule
    {
        [KSPField]
        string ConverterName = string.Empty;

        private List<ModuleResourceConverter> ResourceConverters;

        public override void OnLoad(ConfigNode node)
        {

            InitializeSettings();



        }

        public void InitializeSettings()
        {

            ResourceConverters = part.FindModulesImplementing<ModuleResourceConverter>();

            foreach (var converter in ResourceConverters)
            {
                DEBUG_EVENTS(converter);
                DEBUG_FIELDS(converter, false);
                DEBUG_ACTIONS(converter);
            }

        }


        public void DEBUG_EVENTS(ModuleResourceConverter ResConv)
        {
            foreach (BaseEvent ResConvEvent in ResConv.Events)
            {
                Debug.Log(
                    "\nModuleResourceConverter.Events" +
                    "\nGUIName: " + ResConvEvent.GUIName +
                    "\nid: " + ResConvEvent.id +
                    "\nname: " + ResConvEvent.name +
                    "\nactive: " + ResConvEvent.active +
                    "\nassigned: " + ResConvEvent.assigned +
                    "\ncategory: " + ResConvEvent.category +
                    "\nexternalToEVAOnly: " + ResConvEvent.externalToEVAOnly +
                    "\nguiActive: " + ResConvEvent.guiActive +
                    "\nguiActiveEditor: " + ResConvEvent.guiActiveEditor +
                    "\nguiActiveUncommand: " + ResConvEvent.guiActiveUncommand +
                    "\nguiActiveUnfocused: " + ResConvEvent.guiActiveUnfocused +
                    "\nguiIcon: " + ResConvEvent.guiIcon +
                    "\nunfocusedRange: " + ResConvEvent.unfocusedRange +
                    "\n");
            }
        }
        public void DEBUG_FIELDS(ModuleResourceConverter ResConv, bool OnlyActive = true)
        {
            for (int i = 0; i < ResConv.Fields.Count; i++)
            {
                if (ResConv.Fields[i].guiActive == OnlyActive)
                {
                    Debug.Log(
                        "\nModuleResourceConverter.Fields[" + i + "]" +
                        "\nguiName: " + ResConv.Fields[i].guiName +
                        "\nname: " + ResConv.Fields[i].name +
                        "\noriginalValue: " + ResConv.Fields[i].originalValue +
                        "\nisPersistant: " + ResConv.Fields[i].isPersistant +
                        "\nguiActive: " + ResConv.Fields[i].guiActive +
                        "\nguiActiveEditor: " + ResConv.Fields[i].guiActiveEditor +
                        "\nguiFormat: " + ResConv.Fields[i].guiFormat +
                        "\nguiUnits: " + ResConv.Fields[i].guiUnits +
                        "\nHasInterface: " + ResConv.Fields[i].HasInterface +
                        "\nhost: " + ResConv.Fields[i].host +
                        "\nuiControlEditor: " + ResConv.Fields[i].uiControlEditor +
                        "\nuiControlFlight: " + ResConv.Fields[i].uiControlFlight +
                        "\nuiControlOnly: " + ResConv.Fields[i].uiControlOnly +
                        "\n");
                }
            }
        }
        public void DEBUG_ACTIONS(ModuleResourceConverter ResConv)
        {
            for (int i = 0; i < ResConv.Actions.Count; i++)
            {
                Debug.Log(
                    "\nModuleResourceConverter.Actions[" + i + "]" +
                    "\nguiName: " + ResConv.Actions[i].guiName +
                    "\nname: " + ResConv.Actions[i].name +
                    "\noriginalValue: " + ResConv.Actions[i].actionGroup +
                    "\nisPersistant: " + ResConv.Actions[i].active +
                    "\nguiActive: " + ResConv.Actions[i].defaultActionGroup +
                    "\nguiActiveEditor: " + ResConv.Actions[i].listParent +
                    "\n");
            }
        }
    }
}
*/