//using System;
//using System.Collections.Generic;
//using System.Linq;
using UnityEngine;
//using GTI.GenericFunctions;
using static GTI.Config.GTIConfig;

namespace GTI
{
    partial class OLD_GTI_MultiModeConverter : PartModule
    {
        [KSPField]
        public string debugMode = "false";

        private void debugActivator()
        {
            if (DebugActive && DebugLevel == iDebugLevel.DebugInfo) { Events["DEBUG_EVENT"].guiActive = true; Events["DEBUG_EVENT"].guiActiveEditor = true; Events["DEBUG_EVENT"].active = true; Debug.Log("GTI_MultiModeConverter debugMode activated"); }
            else { Events["DEBUG_EVENT"].guiActive = false; Events["DEBUG_EVENT"].guiActiveEditor = false; Events["DEBUG_EVENT"].active = false; }
        }

        #region --------------------------------Debugging---------------------------------------
        [KSPEvent(active = true, guiActive = true, guiActiveEditor = true, guiName = "DEBUG GTI_MultiModeConverter")]
        public void DEBUG_EVENT()
        {
            initializeSettings();
            //PhysicsUtilities Calc = new PhysicsUtilities();
            //Utilities MiscFx = new Utilities();

            foreach (var converter in MRC)
            {
                Debug.Log("converter.Events");
                //for (int i = 0; i < converter.Events.Count; i++)
                //{
                //    Debug.Log(
                //         "\ni: [" + i + "]" +
                //         "\nGUIName: " + converter.Events[i].GUIName +
                //         "\nid: " + converter.Events[i].id +
                //         "\nname: " + converter.Events[i].name +
                //         "\nactive: " + converter.Events[i].active +
                //         "\nassigned: " + converter.Events[i].assigned +
                //         "\ncategory: " + converter.Events[i].category +
                //         "\nexternalToEVAOnly: " + converter.Events[i].externalToEVAOnly +
                //         "\nguiActive: " + converter.Events[i].guiActive +
                //         "\nguiActiveEditor: " + converter.Events[i].guiActiveEditor +
                //         "\nguiActiveUncommand: " + converter.Events[i].guiActiveUncommand +
                //         "\nguiActiveUnfocused: " + converter.Events[i].guiActiveUnfocused +
                //         "\nguiIcon: " + converter.Events[i].guiIcon +
                //         "\nunfocusedRange: " + converter.Events[i].unfocusedRange +
                //         "\n");
                //}
                foreach (BaseEvent e in converter.Events)
                {
                    Debug.Log(
                         //"\ni: [" + i + "]" +
                         "\nGUIName: " + e.GUIName +
                         "\nid: " + e.id +
                         "\nname: " + e.name +
                         "\nactive: " + e.active +
                         "\nassigned: " + e.assigned +
                         "\ncategory: " + e.category +
                         "\nexternalToEVAOnly: " + e.externalToEVAOnly +
                         "\nguiActive: " + e.guiActive +
                         "\nguiActiveEditor: " + e.guiActiveEditor +
                         "\nguiActiveUncommand: " + e.guiActiveUncommand +
                         "\nguiActiveUnfocused: " + e.guiActiveUnfocused +
                         "\nguiIcon: " + e.guiIcon +
                         "\nunfocusedRange: " + e.unfocusedRange +
                         "\n");
                }

                Debug.Log("converter.Fields");
                for (int i = 0; i < converter.Fields.Count; i++)
                {
                    if (converter.Fields[i].guiActive)
                    {
                        Debug.Log(
                            "\ni: [" + i + "]" +
                            "\nguiName: " + converter.Fields[i].guiName +
                            "\nname: " + converter.Fields[i].name +
                            "\noriginalValue: " + converter.Fields[i].originalValue +
                            "\nisPersistant: " + converter.Fields[i].isPersistant +
                            "\nguiActive: " + converter.Fields[i].guiActive +
                            "\nguiActiveEditor: " + converter.Fields[i].guiActiveEditor +
                            "\nguiFormat: " + converter.Fields[i].guiFormat +
                            "\nguiUnits: " + converter.Fields[i].guiUnits +
                            "\nHasInterface: " + converter.Fields[i].HasInterface +
                            "\nhost: " + converter.Fields[i].host +
                            "\nuiControlEditor: " + converter.Fields[i].uiControlEditor +
                            "\nuiControlFlight: " + converter.Fields[i].uiControlFlight +
                            "\nuiControlOnly: " + converter.Fields[i].uiControlOnly +
                            "\n");
                    }
                }
                Debug.Log("converter.Actions");
                for (int i = 0; i < converter.Actions.Count; i++)
                {
                    Debug.Log(
                        "\nconverter.Actions[" + i + "]" +
                        "\nguiName: " + converter.Actions[i].guiName +
                        "\nname: " + converter.Actions[i].name +
                        "\noriginalValue: " + converter.Actions[i].actionGroup +
                        "\nisPersistant: " + converter.Actions[i].active +
                        "\nguiActive: " + converter.Actions[i].defaultActionGroup +
                        "\nguiActiveEditor: " + converter.Actions[i].listParent +
                        "\n");
                }
            }
        }
        #endregion
    }
}
