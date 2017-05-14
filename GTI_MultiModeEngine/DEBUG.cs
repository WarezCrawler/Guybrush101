using UnityEngine;

namespace GTI
{
    //partial class GTI_MultiModeEngineFX : PartModule
    //{

    //    #region --------------------------------Debugging---------------------------------------
    //    [KSPEvent(active = true, guiActive = true, guiActiveEditor = true, guiName = "[GTI] DEBUG")]
    //    public void DEBUG_ENGINESSWITCH()
    //    {
    //        initializeSettings();
    //        //PhysicsUtilities Calc = new PhysicsUtilities();
    //        //Utilities MiscFx = new Utilities();

    //        //System.Text.StringBuilder BuildString = new System.Text.StringBuilder();
    //        //float Density = propList[selectedPropellant].propDensity;
    //        //int i = 0;





    //        foreach (var moduleEngine in ModuleEngines)
    //        {
    //            Debug.Log("moduleEngine.g: " + moduleEngine.g);

    //            //Debug.Log("CGF velCurveKeys: " + velCurveKeys);
    //            //Debug.Log("CGF atmCurveKeys: " + atmCurveKeys);
    //            Debug.Log(
    //                "\nSome key information on moduleengine: " + moduleEngine.GUIName +
    //                "\nrequestedThrottle: " + moduleEngine.requestedThrottle * 100 + "%" +
    //                "\nmaxThrust: " + moduleEngine.maxThrust +
    //                "\nresultingThrust: " + moduleEngine.resultingThrust +
    //                "\nmaxFuelFlow: " + moduleEngine.maxFuelFlow +
    //                "\nrequestedMassFlow: " + moduleEngine.requestedMassFlow
    //                //"\nmaxFuelRate (calc): " + Calc.calcFuelRateFromfuelFlow(moduleEngine.maxFuelFlow, Density) +
    //                //"\nFuelRate (calc): " + Calc.calcFuelRateFromfuelFlow(moduleEngine.requestedMassFlow, Density)
    //                //"\nWeighted Density of " + propList[selectedPropellant].Propellants + " is " + Density + "Kg/L"
    //                );

    //            foreach (var propellant in moduleEngine.propellants)
    //            {
    //                Debug.Log(
    //                 "\nforeach(var propellant in moduleEngine.propellants)" +
    //                 "\nPropellant: " + propellant.name +
    //                 "\nratio: " + propellant.ratio +
    //                 "\ndrawStackGauge: " + propellant.drawStackGauge +
    //                 "\nignoreForISP: " + propellant.ignoreForIsp);
    //            }


    //            //for (int j = 0; j < moduleEngine.atmosphereCurve.Curve.length; j++)
    //            //{
    //            //    Debug.Log(moduleEngine.atmosphereCurve.Curve[j].time + ", " + moduleEngine.atmosphereCurve.Curve[j].value);
    //            //}

    //            //DEBUG
    //            /*i = 0;
    //            float CurveTimeValue = 0;
    //            foreach (Keyframe key in moduleEngine.atmosphereCurve.Curve.keys)
    //            {
    //                Debug.Log("atmosphereKey[" + i + "]: " + key.time + " " + key.value + " " + key.inTangent + " " + key.outTangent);
    //                if (CurveTimeValue < key.value) { CurveTimeValue = key.value;  }
    //                i++;
    //            }*/
    //            Debug.Log(Utilities.KeyFrameGetToCFG(moduleEngine.atmosphereCurve.Curve.keys, "atmosphereKeys --> "));
    //            /*Debug.Log(
    //                "ISP: " + CurveTimeValue +
    //                "\nmaxFuelRate should be: " + Calc.calcFuelFlow(moduleEngine.maxThrust, Density, CurveTimeValue)
    //            );*/
    //            Debug.Log(Utilities.KeyFrameGetToCFG(moduleEngine.atmCurve.Curve.keys, "atmCurveKeys --> "));
    //            Debug.Log(Utilities.KeyFrameGetToCFG(moduleEngine.velCurve.Curve.keys, "velCurveKeys --> "));

    //            Debug.Log(
    //                "Events for the following engine" +
    //                "\nmoduleEngine.GUIName" + moduleEngine.GUIName
    //                );
    //            foreach (var engineEvent in moduleEngine.Events)
    //            {
    //                Debug.Log(
    //                    "\nmoduleEngine.Events" +
    //                    "\nGUIName: " + engineEvent.GUIName +
    //                    "\nid: " + engineEvent.id +
    //                    "\nname: " + engineEvent.name +
    //                    "\nactive: " + engineEvent.active +
    //                    "\nassigned: " + engineEvent.assigned +
    //                    "\ncategory: " + engineEvent.category +
    //                    "\nexternalToEVAOnly: " + engineEvent.externalToEVAOnly +
    //                    "\nguiActive: " + engineEvent.guiActive +
    //                    "\nguiActiveEditor: " + engineEvent.guiActiveEditor +
    //                    "\nguiActiveUncommand: " + engineEvent.guiActiveUncommand +
    //                    "\nguiActiveUnfocused: " + engineEvent.guiActiveUnfocused +
    //                    "\nguiIcon: " + engineEvent.guiIcon +
    //                    "\nunfocusedRange: " + engineEvent.unfocusedRange +
    //                    "\n");
    //            }

    //            for (int i = 0; i < moduleEngine.Fields.Count; i++)
    //            {
    //                //moduleEngine.Fields[i].guiName;

    //                if (moduleEngine.Fields[i].guiActive)
    //                {
    //                    Debug.Log(
    //                        "\nmoduleEngine.Fields[" + i + "]" +
    //                        "\nguiName: " + moduleEngine.Fields[i].guiName +
    //                        "\nname: " + moduleEngine.Fields[i].name +
    //                        "\noriginalValue: " + moduleEngine.Fields[i].originalValue +
    //                        "\nisPersistant: " + moduleEngine.Fields[i].isPersistant +
    //                        "\nguiActive: " + moduleEngine.Fields[i].guiActive +
    //                        "\nguiActiveEditor: " + moduleEngine.Fields[i].guiActiveEditor +
    //                        "\nguiFormat: " + moduleEngine.Fields[i].guiFormat +
    //                        "\nguiUnits: " + moduleEngine.Fields[i].guiUnits +
    //                        "\nHasInterface: " + moduleEngine.Fields[i].HasInterface +
    //                        "\nhost: " + moduleEngine.Fields[i].host +
    //                        "\nuiControlEditor: " + moduleEngine.Fields[i].uiControlEditor +
    //                        "\nuiControlFlight: " + moduleEngine.Fields[i].uiControlFlight +
    //                        "\nuiControlOnly: " + moduleEngine.Fields[i].uiControlOnly +
    //                        "\n");
    //                }
    //            }
    //            for (int i = 0; i < moduleEngine.Actions.Count; i++)
    //            {
    //                //moduleEngine.Fields[i].guiName;

    //                //if (moduleEngine.Actions[i].active)
    //                //{
    //                Debug.Log(
    //                    "\nmoduleEngine.Actions[" + i + "]" +
    //                    "\nguiName: " + moduleEngine.Actions[i].guiName +
    //                    "\nname: " + moduleEngine.Actions[i].name +
    //                    "\noriginalValue: " + moduleEngine.Actions[i].actionGroup +
    //                    "\nisPersistant: " + moduleEngine.Actions[i].active +
    //                    "\nguiActive: " + moduleEngine.Actions[i].defaultActionGroup +
    //                    "\nguiActiveEditor: " + moduleEngine.Actions[i].listParent +
    //                    "\n");
    //                //}
    //            }

    //            //foreach (var engineField in moduleEngine.Fields)
    //            //{
    //            //    Debug.Log("moduleEngine.Fields" +
    //            //        "\nengineField: " + engineField
    //            //        );
    //            //}




    //            ////Curve experiment
    //            //try
    //            //{
    //            //    Debug.Log("ThrottleISPCurve[0]: " + ThrottleISPCurve[0]);
    //            //}
    //            //catch
    //            //{
    //            //    Debug.LogError("ThrottleISPCurve[0] resulted in a error");
    //            //}
    //            //try
    //            //{
    //            //    Debug.Log("ThrottleISPCurve: " + ThrottleISPCurve);
    //            //}
    //            //catch
    //            //{
    //            //    Debug.LogError("ThrottleISPCurve resulted in a error");
    //            //}


    //        }

    //        /*
    //        foreach (var bodyItem in FlightGlobals.Bodies)
    //        {
    //            Debug.Log("Planet stats -->" +
    //                "\nBodyName: " + bodyItem.bodyName +
    //                "\nRadius: " + bodyItem.Radius +
    //                "\nsphereOfInfluence: " + bodyItem.sphereOfInfluence +
    //                "\nGetInstanceID: " + bodyItem.GetInstanceID() +
    //                "\natmospherePressureCurve:" + MiscFx.KeyFrameGetToCFG(bodyItem.atmospherePressureCurve.Curve.keys, "bodyItem.atmospherePressureCurve.Curve.keys --> ")
    //                );

    //            //Clear string Builder
    //            //BuildString.Length = 0;
    //            //BuildString.Capacity = 16;
    //        }
    //        */
    //    }


    //    #endregion
    //}

    partial class GTI_MultiModeEngineOLD : PartModule
    {
        #region --------------------------------Debugging---------------------------------------
        [KSPEvent(active = false, guiActive = false, guiActiveEditor = false, guiName = "[GTI] DEBUG")]
        public void DEBUG_ENGINESSWITCH()
        {
            initializeSettings();

            Debug.Log(
                "\nSome key information on ModuleEngines: " + ModuleEngines.GUIName +
                "\nrequestedThrottle: " + ModuleEngines.requestedThrottle * 100 + "%" +
                "\nmaxThrust: " + ModuleEngines.maxThrust +
                "\nresultingThrust: " + ModuleEngines.resultingThrust +
                "\nmaxFuelFlow: " + ModuleEngines.maxFuelFlow +
                "\nrequestedMassFlow: " + ModuleEngines.requestedMassFlow +
                "ModuleEngines.g: " + ModuleEngines.g
                );

            foreach (var propellant in ModuleEngines.propellants)
            {
                Debug.Log(
                 "\nforeach(var propellant in ModuleEngines.propellants)" +
                 "\nPropellant: " + propellant.name +
                 "\nratio: " + propellant.ratio +
                 "\ndrawStackGauge: " + propellant.drawStackGauge +
                 "\nignoreForISP: " + propellant.ignoreForIsp);
            }

            Debug.Log(
                "Events for the following engine" +
                "\nModuleEngines.GUIName" + ModuleEngines.GUIName
                );

            GTIDebug.LogEvents(ModuleEngines);

            //foreach (var engineEvent in ModuleEngines.Events)
            //{
            //    Debug.Log(
            //        "\nModuleEngines.Events" +
            //        "\nGUIName: " + engineEvent.GUIName +
            //        "\nid: " + engineEvent.id +
            //        "\nname: " + engineEvent.name +
            //        "\nactive: " + engineEvent.active +
            //        "\nassigned: " + engineEvent.assigned +
            //        "\ncategory: " + engineEvent.category +
            //        "\nexternalToEVAOnly: " + engineEvent.externalToEVAOnly +
            //        "\nguiActive: " + engineEvent.guiActive +
            //        "\nguiActiveEditor: " + engineEvent.guiActiveEditor +
            //        "\nguiActiveUncommand: " + engineEvent.guiActiveUncommand +
            //        "\nguiActiveUnfocused: " + engineEvent.guiActiveUnfocused +
            //        "\nguiIcon: " + engineEvent.guiIcon +
            //        "\nunfocusedRange: " + engineEvent.unfocusedRange +
            //        "\n");
            //}

            for (int i = 0; i < ModuleEngines.Fields.Count; i++)
            {
                if (ModuleEngines.Fields[i].guiActive)
                {
                    Debug.Log(
                        "\nModuleEngines.Fields[" + i + "]" +
                        "\nguiName: " + ModuleEngines.Fields[i].guiName +
                        "\nname: " + ModuleEngines.Fields[i].name +
                        "\noriginalValue: " + ModuleEngines.Fields[i].originalValue +
                        "\nisPersistant: " + ModuleEngines.Fields[i].isPersistant +
                        "\nguiActive: " + ModuleEngines.Fields[i].guiActive +
                        "\nguiActiveEditor: " + ModuleEngines.Fields[i].guiActiveEditor +
                        "\nguiFormat: " + ModuleEngines.Fields[i].guiFormat +
                        "\nguiUnits: " + ModuleEngines.Fields[i].guiUnits +
                        "\nHasInterface: " + ModuleEngines.Fields[i].HasInterface +
                        "\nhost: " + ModuleEngines.Fields[i].host +
                        "\nuiControlEditor: " + ModuleEngines.Fields[i].uiControlEditor +
                        "\nuiControlFlight: " + ModuleEngines.Fields[i].uiControlFlight +
                        "\nuiControlOnly: " + ModuleEngines.Fields[i].uiControlOnly +
                        "\n");
                }
            }
            for (int i = 0; i < ModuleEngines.Actions.Count; i++)
            {
                Debug.Log(
                    "\nModuleEngines.Actions[" + i + "]" +
                    "\nguiName: " + ModuleEngines.Actions[i].guiName +
                    "\nname: " + ModuleEngines.Actions[i].name +
                    "\noriginalValue: " + ModuleEngines.Actions[i].actionGroup +
                    "\nisPersistant: " + ModuleEngines.Actions[i].active +
                    "\nguiActive: " + ModuleEngines.Actions[i].defaultActionGroup +
                    "\nguiActiveEditor: " + ModuleEngines.Actions[i].listParent +
                    "\n");
            }

            Debug.Log("[GTI] Part ConfigNode (prefab)\n" + Utilities.GetPartConfig(this.part).ToString());
        }
        #endregion
    }
}
