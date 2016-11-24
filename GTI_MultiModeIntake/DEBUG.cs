using System;
using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    partial class GTI_MultiModeIntake : PartModule
    {
        #region --------------------------------Debugging---------------------------------------
        [KSPEvent(active = false, guiActive = false, guiActiveEditor = true, guiName = "DEBUG")]
        public void DEBUG_INTAKESWITCH()
        {
            /* Debugging Area */

            //Debug.Log("Update GUI");
            //GUIResourceName = ModuleIntakes[0].resourceName;
            //Debug.Log("GUI Updated");

            Debug.Log(
                "\nIntakeAir id: " + PartResourceLibrary.Instance.GetDefinition("IntakeAir").id +
                "\nIntakeAir density: " + PartResourceLibrary.Instance.GetDefinition("IntakeAir").density +
                "\nIntakeAtm id: " + PartResourceLibrary.Instance.GetDefinition("IntakeAtm").id +
                "\nIntakeAtm density: " + PartResourceLibrary.Instance.GetDefinition("IntakeAtm").density
            );

            Debug.Log(
                "\nModuleIntakes[0].resourceName: " + ModuleIntakes[0].resourceName +
                "\nresourceId: " + ModuleIntakes[0].resourceId +
                "\nresourceDef: " + ModuleIntakes[0].resourceDef +
                "\nres: " + ModuleIntakes[0].res +
                "\nresourceUnits: " + ModuleIntakes[0].resourceUnits +
                "\ncheckForOxygen: " + ModuleIntakes[0].checkForOxygen +
                "\narea: " + ModuleIntakes[0].area +
                "\nairFlow: " + ModuleIntakes[0].airFlow +
                "\nModuleIntakes.Count: " + ModuleIntakes.Count
                );

            for (int i = 0; i < part.Resources.Count; i++)
            {
                Debug.Log(
                    "\npart.Resources[0].resourceName: " + part.Resources[i].resourceName +
                    "\npart.Resources[0].amount: " + part.Resources[i].amount +
                    "\npart.Resources[0].maxAmount: " + part.Resources[i].maxAmount +
                    "\nresMaxAmountEmpty: " + resMaxAmountEmpty
                    );
            }

            for (int i = 0; i < ModuleIntakes[0].Fields.Count; i++)
            {
                //moduleEngine.Fields[i].guiName;

                if (ModuleIntakes[0].Fields[i].guiActive)
                {
                    Debug.Log(
                        "\nmoduleEngine.Fields[" + i + "]" +
                        "\nguiName: " + ModuleIntakes[0].Fields[i].guiName +
                        "\nname: " + ModuleIntakes[0].Fields[i].name +
                        "\noriginalValue: " + ModuleIntakes[0].Fields[i].originalValue +
                        "\nisPersistant: " + ModuleIntakes[0].Fields[i].isPersistant +
                        "\nguiActive: " + ModuleIntakes[0].Fields[i].guiActive +
                        "\nguiActiveEditor: " + ModuleIntakes[0].Fields[i].guiActiveEditor +
                        "\nguiFormat: " + ModuleIntakes[0].Fields[i].guiFormat +
                        "\nguiUnits: " + ModuleIntakes[0].Fields[i].guiUnits +
                        "\nHasInterface: " + ModuleIntakes[0].Fields[i].HasInterface +
                        "\nhost: " + ModuleIntakes[0].Fields[i].host +
                        "\nuiControlEditor: " + ModuleIntakes[0].Fields[i].uiControlEditor +
                        "\nuiControlFlight: " + ModuleIntakes[0].Fields[i].uiControlFlight +
                        "\nuiControlOnly: " + ModuleIntakes[0].Fields[i].uiControlOnly +
                        "\n");
                }
            }

        }
        #endregion

    }
}
