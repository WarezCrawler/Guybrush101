using System;
using System.Collections.Generic;
using UnityEngine;
using GTI.GenericFunctions;

namespace GTI
{
    partial class GTI_EngineClassSwitch : PartModule
    {
        [KSPField]
        public string runningEffectName;
        [KSPField]
        public string powerEffectName;
        [KSPField]
        public string spoolEffectName;

        [KSPField]
        public string engageEffectName;
        [KSPField]
        public string disengageEffectName;

        [KSPField]
        public string engineSpoolIdle;
        [KSPField]
        public string engineSpoolTime;

        [KSPField]
        public string useEngineResponseTime;
        [KSPField]
        public string engineAccelerationSpeed;
        [KSPField]
        public string engineDecelerationSpeed;

        [KSPField]
        public string fx_exhaustFlame_blue;
        [KSPField]
        public string fx_exhaustLight_blue;
        [KSPField]
        public string fx_smokeTrail_light;
        [KSPField]
        public string fx_exhaustSparks_flameout;

        [KSPField]
        public string sound_vent_medium;
        [KSPField]
        public string sound_rocket_hard;
        [KSPField]
        public string sound_vent_soft;
        [KSPField]
        public string sound_explosion_low;

        bool runningEffectNameEmpty, powerEffectNameEmpty, spoolEffectNameEmpty, engineSpoolIdleEmpty, engineSpoolTimeEmpty;
        bool useEngineResponseTimeEmpty, engineAccelerationSpeedEmpty, engineDecelerationSpeedEmpty;
        bool engageEffectNameEmpty, disengageEffectNameEmpty;
        bool fx_exhaustFlame_blueEmpty, fx_exhaustLight_blueEmpty, fx_smokeTrail_lightEmpty, fx_exhaustSparks_flameoutEmpty;
        bool sound_vent_mediumEmpty, sound_rocket_hardEmpty, sound_vent_softEmpty, sound_explosion_lowEmpty;


        public void InitializeEffects()
        {
            Utilities Util = new Utilities();

            string[] arrRunningEffectName, arrPowerEffectName, arrSpoolEffectName, arrEngineSpoolIdle, arrEngineSpoolTime;
            string[] arrUseEngineResponseTime, arrEngineAccelerationSpeed, arrEngineDecelerationSpeed;
            string[] arrEngageEffectName, arrDisengageEffectName;
            string[] arrFx_exhaustFlame_blue, arrFx_exhaustLight_blue, arrFx_smokeTrail_light, arrFx_exhaustSparks_flameout;
            string[] arrSound_vent_medium, arrSound_rocket_hard, arrSound_vent_soft, arrSound_explosion_low;

            #region Parse Arrays
            runningEffectNameEmpty          = Util.ArraySplitEvaluate(runningEffectName         , out arrRunningEffectName, ';');
            powerEffectNameEmpty            = Util.ArraySplitEvaluate(powerEffectName           , out arrPowerEffectName, ';');
            spoolEffectNameEmpty            = Util.ArraySplitEvaluate(spoolEffectName           , out arrSpoolEffectName, ';');

            engageEffectNameEmpty = Util.ArraySplitEvaluate(engageEffectName, out arrEngageEffectName, ';');
            disengageEffectNameEmpty = Util.ArraySplitEvaluate(disengageEffectName, out arrDisengageEffectName, ';');

            engineSpoolIdleEmpty            = Util.ArraySplitEvaluate(engineSpoolIdle           , out arrEngineSpoolIdle, ';');
            engineSpoolTimeEmpty            = Util.ArraySplitEvaluate(engineSpoolTime           , out arrEngineSpoolTime, ';');

            useEngineResponseTimeEmpty      = Util.ArraySplitEvaluate(useEngineResponseTime     , out arrUseEngineResponseTime, ';');
            engineAccelerationSpeedEmpty    = Util.ArraySplitEvaluate(engineAccelerationSpeed   , out arrEngineAccelerationSpeed, ';');
            engineDecelerationSpeedEmpty    = Util.ArraySplitEvaluate(engineDecelerationSpeed   , out arrEngineDecelerationSpeed, ';');

            fx_exhaustFlame_blueEmpty       = Util.ArraySplitEvaluate(fx_exhaustFlame_blue      , out arrFx_exhaustFlame_blue, ';');
            fx_exhaustLight_blueEmpty       = Util.ArraySplitEvaluate(fx_exhaustLight_blue      , out arrFx_exhaustLight_blue, ';');
            fx_smokeTrail_lightEmpty        = Util.ArraySplitEvaluate(fx_smokeTrail_light       , out arrFx_smokeTrail_light, ';');
            fx_exhaustSparks_flameoutEmpty  = Util.ArraySplitEvaluate(fx_exhaustSparks_flameout , out arrFx_exhaustSparks_flameout, ';');

            sound_vent_mediumEmpty          = Util.ArraySplitEvaluate(sound_vent_medium         , out arrSound_vent_medium, ';');
            sound_rocket_hardEmpty          = Util.ArraySplitEvaluate(sound_rocket_hard         , out arrSound_rocket_hard, ';');
            sound_vent_softEmpty            = Util.ArraySplitEvaluate(sound_vent_soft           , out arrSound_vent_soft, ';');
            sound_explosion_lowEmpty        = Util.ArraySplitEvaluate(sound_explosion_low       , out arrSound_explosion_low, ';');
            #endregion

            #region Populate Propellant List
            //Populate the propList
            Debug.Log("EngineSwitch Load Effects to propList");
            for (int i = 0; i < propList.Count; i++)
            {
                propList[i].runningEffectName = runningEffectNameEmpty ? string.Empty : arrRunningEffectName[i];
                propList[i].powerEffectName = powerEffectNameEmpty ? string.Empty : arrPowerEffectName[i];
                propList[i].spoolEffectName = spoolEffectNameEmpty ? string.Empty : arrSpoolEffectName[i];

                propList[i].engageEffectName = engageEffectNameEmpty ? string.Empty : arrEngageEffectName[i];
                propList[i].disengageEffectName = disengageEffectNameEmpty ? string.Empty : arrDisengageEffectName[i];

                propList[i].engineSpoolIdle = engineSpoolIdleEmpty ? string.Empty : arrEngineSpoolIdle[i];
                propList[i].engineSpoolTime = engineSpoolTimeEmpty ? string.Empty : arrEngineSpoolTime[i];

                propList[i].useEngineResponseTime = useEngineResponseTimeEmpty ? string.Empty : arrUseEngineResponseTime[i];
                propList[i].engineAccelerationSpeed = engineAccelerationSpeedEmpty ? string.Empty : arrEngineAccelerationSpeed[i];
                propList[i].engineDecelerationSpeed = engineDecelerationSpeedEmpty ? string.Empty : arrEngineDecelerationSpeed[i];

                propList[i].fx_exhaustFlame_blue = fx_exhaustFlame_blueEmpty ? string.Empty : arrFx_exhaustFlame_blue[i];
                propList[i].fx_exhaustLight_blue = fx_exhaustLight_blueEmpty ? string.Empty : arrFx_exhaustLight_blue[i];
                propList[i].fx_smokeTrail_light = fx_smokeTrail_lightEmpty ? string.Empty : arrFx_smokeTrail_light[i];
                propList[i].fx_exhaustSparks_flameout = fx_exhaustSparks_flameoutEmpty ? string.Empty : arrFx_exhaustSparks_flameout[i];

                propList[i].sound_vent_medium = sound_vent_mediumEmpty ? string.Empty : arrSound_vent_medium[i];
                propList[i].sound_rocket_hard = sound_rocket_hardEmpty ? string.Empty : arrSound_rocket_hard[i];
                propList[i].sound_vent_soft = sound_vent_softEmpty ? string.Empty : arrSound_vent_soft[i];
                propList[i].sound_explosion_low = sound_explosion_lowEmpty ? string.Empty : arrSound_explosion_low[i];
            }
            Debug.Log("EngineSwitch Load Effects to propList finished");
            #endregion
        }

        public void updateEngineModuleEffects(ModuleEngines moduleEngine, bool calledByPlayer, string callingFunction = "player")
        {
            ConfigNode EngineEffectNode = new ConfigNode();

            Debug.Log("Switch exhaust effects on: " + moduleEngine.name);
            if (!runningEffectNameEmpty)
            {
                EngineEffectNode.SetValue("runningEffectName", propList[selectedPropellant].runningEffectName, true);
                Debug.Log("EngineEffectNode.SetValue('runningEffectName', " + propList[selectedPropellant].runningEffectName + ", true);");
            }
            if (!powerEffectNameEmpty)
            {
                EngineEffectNode.SetValue("powerEffectName", propList[selectedPropellant].powerEffectName, true);
                Debug.Log("EngineEffectNode.SetValue('powerEffectName', " + propList[selectedPropellant].powerEffectName + ", true);");
            }
            if (!spoolEffectNameEmpty)
            {
                EngineEffectNode.SetValue("spoolEffectName", propList[selectedPropellant].spoolEffectName, true);
                Debug.Log("EngineEffectNode.SetValue('spoolEffectName', " + propList[selectedPropellant].spoolEffectName + ", true);");
            }

            if (!engageEffectNameEmpty)
            {
                EngineEffectNode.SetValue("engageEffectName", propList[selectedPropellant].engageEffectName, true);
                Debug.Log("EngineEffectNode.SetValue('engageEffectName', " + propList[selectedPropellant].engageEffectName + ", true);");
            }
            if (!disengageEffectNameEmpty)
            {
                EngineEffectNode.SetValue("disengageEffectName", propList[selectedPropellant].disengageEffectName, true);
                Debug.Log("EngineEffectNode.SetValue('disengageEffectName', " + propList[selectedPropellant].disengageEffectName + ", true);");
            }

            if (!engineSpoolIdleEmpty)
            {
                EngineEffectNode.SetValue("engineSpoolIdle", propList[selectedPropellant].engineSpoolIdle, true);
                Debug.Log("EngineEffectNode.SetValue('engineSpoolIdle', " + propList[selectedPropellant].engineSpoolIdle + ", true);");
            }
            if (!engineSpoolTimeEmpty)
            {
                EngineEffectNode.SetValue("engineSpoolTime", propList[selectedPropellant].engineSpoolTime, true);
                Debug.Log("EngineEffectNode.SetValue('engineSpoolTime', " + propList[selectedPropellant].engineSpoolTime + ", true);");
            }

            if (!useEngineResponseTimeEmpty)
            {
                EngineEffectNode.SetValue("useEngineResponseTime", propList[selectedPropellant].useEngineResponseTime, true);
                Debug.Log("EngineEffectNode.SetValue('useEngineResponseTime', " + propList[selectedPropellant].useEngineResponseTime + ", true);");
            }
            if (!engineAccelerationSpeedEmpty)
            {
                EngineEffectNode.SetValue("engineAccelerationSpeed", propList[selectedPropellant].engineAccelerationSpeed, true);
                Debug.Log("EngineEffectNode.SetValue('engineAccelerationSpeed', " + propList[selectedPropellant].engineAccelerationSpeed + ", true);");
            }
            if (!engineDecelerationSpeedEmpty)
            {
                EngineEffectNode.SetValue("engineDecelerationSpeed", propList[selectedPropellant].engineDecelerationSpeed, true);
                Debug.Log("EngineEffectNode.SetValue('engineDecelerationSpeed', " + propList[selectedPropellant].engineDecelerationSpeed + ", true);");
            }

            if (!fx_exhaustFlame_blueEmpty)
            {
                EngineEffectNode.SetValue("fx_exhaustFlame_blue", propList[selectedPropellant].fx_exhaustFlame_blue, true);
                Debug.Log("EngineEffectNode.SetValue('fx_exhaustFlame_blue', " + propList[selectedPropellant].fx_exhaustFlame_blue + ", true);");
            }
            if (!fx_exhaustLight_blueEmpty)
            {
                EngineEffectNode.SetValue("fx_exhaustLight_blue", propList[selectedPropellant].fx_exhaustLight_blue, true);
                Debug.Log("EngineEffectNode.SetValue('fx_exhaustLight_blue', " + propList[selectedPropellant].fx_exhaustLight_blue + ", true);");
            }
            if (!fx_smokeTrail_lightEmpty)
            {
                EngineEffectNode.SetValue("fx_smokeTrail_light", propList[selectedPropellant].fx_smokeTrail_light, true);
                Debug.Log("EngineEffectNode.SetValue('fx_smokeTrail_light', " + propList[selectedPropellant].fx_smokeTrail_light + ", true);");
            }
            if (!fx_exhaustSparks_flameoutEmpty)
            {
                EngineEffectNode.SetValue("fx_exhaustSparks_flameout", propList[selectedPropellant].fx_exhaustSparks_flameout, true);
                Debug.Log("EngineEffectNode.SetValue('fx_exhaustSparks_flameout', " + propList[selectedPropellant].fx_exhaustSparks_flameout + ", true);");
            }

            if (!sound_vent_mediumEmpty)
            {
                EngineEffectNode.SetValue("sound_vent_medium", propList[selectedPropellant].sound_vent_medium, true);
                Debug.Log("EngineEffectNode.SetValue('sound_vent_medium', " + propList[selectedPropellant].sound_vent_medium + ", true);");
            }
            if (!sound_rocket_hardEmpty)
            {
                EngineEffectNode.SetValue("sound_rocket_hard", propList[selectedPropellant].sound_rocket_hard, true);
                Debug.Log("EngineEffectNode.SetValue('sound_rocket_hard', " + propList[selectedPropellant].sound_rocket_hard + ", true);");
            }
            if (!sound_vent_softEmpty)
            {
                EngineEffectNode.SetValue("sound_vent_soft", propList[selectedPropellant].sound_vent_soft, true);
                Debug.Log("EngineEffectNode.SetValue('sound_vent_soft', " + propList[selectedPropellant].sound_vent_soft + ", true);");
            }
            if (!sound_explosion_lowEmpty)
            {
                EngineEffectNode.SetValue("sound_explosion_low", propList[selectedPropellant].sound_explosion_low, true);
                Debug.Log("EngineEffectNode.SetValue('sound_explosion_low', " + propList[selectedPropellant].sound_explosion_low + ", true);");
            }

            moduleEngine.Load(EngineEffectNode);
            
            
            //moduleEngine.SetupFXGroups();
            //moduleEngine.DeactivateRunningFX();
            //moduleEngine.ActivateRunningFX();
            //moduleEngine.InitializeFX();
            //part.InitializeModules();
            //part.InitializeEffects();


            //ConfigNode EngineNode = part.partInfo.partConfig.GetNode("MODULE");
            //ConfigNode copiedEngineNode = new ConfigNode("MODULE");
            //EngineNode.CopyTo(copiedEngineNode);

            //Debug.Log("foreach (ConfigNode innervalue in copiedEngineNode.values)");
            //foreach (var innervalue in copiedEngineNode.values)
            //{
            //    //Debug.Log("foreach (ConfigNode innervalue in copiedEngineNode.values)");
            //    Debug.Log(
            //        "\nid: " + innervalue.ToString() + 
            //        "\name: " + innervalue.name
            //        );

            //    //if (part.partInfo.partConfig.nodes.Contains("ModuleEnginesFX"))
            //    //{
            //    //    Debug.Log("If ModuleEnginesFX");
            //    //    innervalue.Save("Node.txt");
            //    //    Debug.Log(
            //    //        "\ninnerNode1.GetValue('name'): " + innervalue.GetValue("name") +
            //    //        "\ninnerNode1.GetValue('maxThrust'): " + innervalue.GetValue("maxThrust")
            //    //        );
            //    //    //innerNode1.CopyTo(copiedEngineNode);
            //    //    break;
            //    //}
            //}
            //foreach (ConfigNode innerNode1 in copiedEngineNode.nodes)
            //{
            //    Debug.Log("foreach (ConfigNode innerNode1 in copiedEngineNode.nodes)");
            //    Debug.Log("part.partInfo.partConfig.nodes.Contains('ModuleEnginesFX'): " + part.partInfo.partConfig.nodes.Contains("ModuleEnginesFX"));
            //    Debug.Log("innerNode1.GetNodeID('ModuleEnginesFX'): " + innerNode1.GetNodeID("ModuleEnginesFX"));
            //    Debug.Log("innerNode1.GetValue('name'): " + innerNode1.GetValue("name"));

            //    if (part.partInfo.partConfig.nodes.Contains("ModuleEnginesFX"))
            //    {
            //        Debug.Log("If ModuleEnginesFX");
            //        innerNode1.Save("Node.txt");
            //        Debug.Log(
            //            "\ninnerNode1.GetValue('name'): " + innerNode1.GetValue("name") +
            //            "\ninnerNode1.GetValue('maxThrust'): " + innerNode1.GetValue("maxThrust")
            //            );
            //        //innerNode1.CopyTo(copiedEngineNode);
            //        break;
            //    }
            //}

            //ConfigNode effectsNode = part.partInfo.partConfig.GetNode("EFFECTS");
            //ConfigNode copiedEffectsNode = new ConfigNode("EFFECTS");
            //effectsNode.CopyTo(copiedEffectsNode);
            //part.Effects.OnLoad(copiedEffectsNode);




            //EngineNode.CopyTo(copiedEngineNode);
            //part.Effects.OnLoad(copiedEngineNode);
            //part.LoadModule(copiedEngineNode);
            //part.partInfo.partConfig.GetNodes("MODULE");
            //part.partInfo.partConfig.GetNode("ModuleEnginesFX");



            //moduleEngine.OnLoad(EngineEffectNode);
            //part.partInfo.partConfig.
            //part.Effects.Initialize();
            //moduleEngine.InitializeFX();

            //moduleEngine.FXReset();
            //moduleEngine.FXUpdate();
            //moduleEngine.
            //}
        }
    }
}
