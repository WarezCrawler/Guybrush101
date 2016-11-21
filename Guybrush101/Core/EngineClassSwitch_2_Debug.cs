using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    partial class GTI_EngineClassSwitch_2 : PartModule
    {

        #region --------------------------------Debugging---------------------------------------
        [KSPEvent(active = true, guiActive = true, guiActiveEditor = true, guiName = "DEBUG")]
        public void DEBUG_ENGINESSWITCH()
        {
            initializeSettings();
            PhysicsUtilities Calc = new PhysicsUtilities();
            Utilities MiscFx = new Utilities();
            //System.Text.StringBuilder BuildString = new System.Text.StringBuilder();
            //float Density = propList[selectedPropellant].propDensity;
            //int i = 0;

            foreach (var moduleEngine in ModuleEngines)
            {

                //Debug.Log("CGF velCurveKeys: " + velCurveKeys);
                //Debug.Log("CGF atmCurveKeys: " + atmCurveKeys);
                Debug.Log(
                    "\nSome key information on moduleengine: " + moduleEngine.GUIName +
                    "\nrequestedThrottle: " + moduleEngine.requestedThrottle * 100 + "%" +
                    "\nmaxThrust: " + moduleEngine.maxThrust +
                    "\nresultingThrust: " + moduleEngine.resultingThrust +
                    "\nmaxFuelFlow: " + moduleEngine.maxFuelFlow +
                    "\nrequestedMassFlow: " + moduleEngine.requestedMassFlow
                    //"\nmaxFuelRate (calc): " + Calc.calcFuelRateFromfuelFlow(moduleEngine.maxFuelFlow, Density) +
                    //"\nFuelRate (calc): " + Calc.calcFuelRateFromfuelFlow(moduleEngine.requestedMassFlow, Density)
                    //"\nWeighted Density of " + propList[selectedPropellant].Propellants + " is " + Density + "Kg/L"
                    );

                foreach (var propellant in moduleEngine.propellants)
                {
                       Debug.Log(
                        "\nforeach(var propellant in moduleEngine.propellants)" +
                        "\nPropellant: " + propellant.name +
                        "\nratio: " + propellant.ratio +
                        "\ndrawStackGauge: " + propellant.drawStackGauge +
                        "\nignoreForISP: " + propellant.ignoreForIsp);
                }


                //for (int j = 0; j < moduleEngine.atmosphereCurve.Curve.length; j++)
                //{
                //    Debug.Log(moduleEngine.atmosphereCurve.Curve[j].time + ", " + moduleEngine.atmosphereCurve.Curve[j].value);
                //}

                //DEBUG
                /*i = 0;
                float CurveTimeValue = 0;
                foreach (Keyframe key in moduleEngine.atmosphereCurve.Curve.keys)
                {
                    Debug.Log("atmosphereKey[" + i + "]: " + key.time + " " + key.value + " " + key.inTangent + " " + key.outTangent);
                    if (CurveTimeValue < key.value) { CurveTimeValue = key.value;  }
                    i++;
                }*/
                Debug.Log(MiscFx.KeyFrameGetToCFG(moduleEngine.atmosphereCurve.Curve.keys, "atmosphereKeys --> "));
                /*Debug.Log(
                    "ISP: " + CurveTimeValue +
                    "\nmaxFuelRate should be: " + Calc.calcFuelFlow(moduleEngine.maxThrust, Density, CurveTimeValue)
                );*/
                Debug.Log(MiscFx.KeyFrameGetToCFG(moduleEngine.atmCurve.Curve.keys, "atmCurveKeys --> "));
                Debug.Log(MiscFx.KeyFrameGetToCFG(moduleEngine.velCurve.Curve.keys, "velCurveKeys --> "));

                Debug.Log(
                    "Events for the following engine" +
                    "\nmoduleEngine.GUIName" + moduleEngine.GUIName
                    );
                foreach (var engineEvent in moduleEngine.Events)
                {
                    Debug.Log(
                        "\nmoduleEngine.Events" +
                        "\nGUIName: " + engineEvent.GUIName +
                        "\nid: " + engineEvent.id +
                        "\nname: " + engineEvent.name +
                        "\nactive: " + engineEvent.active +
                        "\nassigned: " + engineEvent.assigned +
                        "\ncategory: " + engineEvent.category +
                        "\nexternalToEVAOnly: " + engineEvent.externalToEVAOnly +
                        "\nguiActive: " + engineEvent.guiActive +
                        "\nguiActiveEditor: " + engineEvent.guiActiveEditor +
                        "\nguiActiveUncommand: " + engineEvent.guiActiveUncommand +
                        "\nguiActiveUnfocused: " + engineEvent.guiActiveUnfocused +
                        "\nguiIcon: " + engineEvent.guiIcon +
                        "\nunfocusedRange: " + engineEvent.unfocusedRange +
                        "\n");
                }
                
                for (int i = 0; i < moduleEngine.Fields.Count; i++)
                {
                    //moduleEngine.Fields[i].guiName;

                    if (moduleEngine.Fields[i].guiActive)
                    {
                        Debug.Log(
                            "\nmoduleEngine.Fields[" + i + "]" +
                            "\nguiName: " + moduleEngine.Fields[i].guiName +
                            "\nname: " + moduleEngine.Fields[i].name +
                            "\noriginalValue: " + moduleEngine.Fields[i].originalValue +
                            "\nisPersistant: " + moduleEngine.Fields[i].isPersistant +
                            "\nguiActive: " + moduleEngine.Fields[i].guiActive +
                            "\nguiActiveEditor: " + moduleEngine.Fields[i].guiActiveEditor +
                            "\nguiFormat: " + moduleEngine.Fields[i].guiFormat +
                            "\nguiUnits: " + moduleEngine.Fields[i].guiUnits +
                            "\nHasInterface: " + moduleEngine.Fields[i].HasInterface +
                            "\nhost: " + moduleEngine.Fields[i].host +
                            "\nuiControlEditor: " + moduleEngine.Fields[i].uiControlEditor +
                            "\nuiControlFlight: " + moduleEngine.Fields[i].uiControlFlight +
                            "\nuiControlOnly: " + moduleEngine.Fields[i].uiControlOnly +
                            "\n");
                    }
                }
                for (int i = 0; i < moduleEngine.Actions.Count; i++)
                {
                    //moduleEngine.Fields[i].guiName;

                    //if (moduleEngine.Actions[i].active)
                    //{
                        Debug.Log(
                            "\nmoduleEngine.Actions[" + i + "]" +
                            "\nguiName: " + moduleEngine.Actions[i].guiName +
                            "\nname: " + moduleEngine.Actions[i].name +
                            "\noriginalValue: " + moduleEngine.Actions[i].actionGroup +
                            "\nisPersistant: " + moduleEngine.Actions[i].active +
                            "\nguiActive: " + moduleEngine.Actions[i].defaultActionGroup +
                            "\nguiActiveEditor: " + moduleEngine.Actions[i].listParent +
                            "\n");
                    //}
                }

                //foreach (var engineField in moduleEngine.Fields)
                //{
                //    Debug.Log("moduleEngine.Fields" +
                //        "\nengineField: " + engineField
                //        );
                //}
            }

            /*
            foreach (var bodyItem in FlightGlobals.Bodies)
            {
                Debug.Log("Planet stats -->" +
                    "\nBodyName: " + bodyItem.bodyName +
                    "\nRadius: " + bodyItem.Radius +
                    "\nsphereOfInfluence: " + bodyItem.sphereOfInfluence +
                    "\nGetInstanceID: " + bodyItem.GetInstanceID() +
                    "\natmospherePressureCurve:" + MiscFx.KeyFrameGetToCFG(bodyItem.atmospherePressureCurve.Curve.keys, "bodyItem.atmospherePressureCurve.Curve.keys --> ")
                    );

                //Clear string Builder
                //BuildString.Length = 0;
                //BuildString.Capacity = 16;
            }
            */
        }
        #endregion
    }
}
