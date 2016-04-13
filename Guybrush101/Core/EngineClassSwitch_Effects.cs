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
        public string engageEffectName;
        [KSPField]
        public string disengageEffectName;

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

            engineSpoolIdleEmpty            = Util.ArraySplitEvaluate(engineSpoolIdle           , out arrEngineSpoolIdle, ';');
            engineSpoolTimeEmpty            = Util.ArraySplitEvaluate(engineSpoolTime           , out arrEngineSpoolTime, ';');

            useEngineResponseTimeEmpty      = Util.ArraySplitEvaluate(useEngineResponseTime     , out arrUseEngineResponseTime, ';');
            engineAccelerationSpeedEmpty    = Util.ArraySplitEvaluate(engineAccelerationSpeed   , out arrEngineAccelerationSpeed, ';');
            engineDecelerationSpeedEmpty    = Util.ArraySplitEvaluate(engineDecelerationSpeed   , out arrEngineDecelerationSpeed, ';');

            engageEffectNameEmpty           = Util.ArraySplitEvaluate(engageEffectName          , out arrEngageEffectName, ';');
            disengageEffectNameEmpty        = Util.ArraySplitEvaluate(disengageEffectName       , out arrDisengageEffectName, ';');

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

                propList[i].engineSpoolIdle = engineSpoolIdleEmpty ? string.Empty : arrEngineSpoolIdle[i];
                propList[i].engineSpoolTime = engineSpoolTimeEmpty ? string.Empty : arrEngineSpoolTime[i];

                propList[i].useEngineResponseTime = useEngineResponseTimeEmpty ? string.Empty : arrUseEngineResponseTime[i];
                propList[i].engineAccelerationSpeed = engineAccelerationSpeedEmpty ? string.Empty : arrEngineAccelerationSpeed[i];
                propList[i].engineDecelerationSpeed = engineDecelerationSpeedEmpty ? string.Empty : arrEngineDecelerationSpeed[i];

                propList[i].engageEffectName = engageEffectNameEmpty ? string.Empty : arrEngageEffectName[i];
                propList[i].disengageEffectName = disengageEffectNameEmpty ? string.Empty : arrDisengageEffectName[i];

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
            ConfigNode newEngineEffectNode = new ConfigNode();
            ConfigNode EngineEffectNode = newEngineEffectNode;
            

            if (!runningEffectNameEmpty)
            {

                if (string.IsNullOrEmpty(propList[selectedPropellant].runningEffectName))
                {
                    EngineEffectNode.RemoveValue("runningEffectName");
                    moduleEngine.DeactivateRunningFX();
                }
                else
                {
                    EngineEffectNode.AddValue("runningEffectName", propList[selectedPropellant].runningEffectName);
                    moduleEngine.ActivateRunningFX();
                }

                
                Debug.Log(
                        "\npropList[selectedPropellant].runningEffectName " + propList[selectedPropellant].runningEffectName +
                        "\nstring.IsNullOrEmpt " + string.IsNullOrEmpty(propList[selectedPropellant].runningEffectName) +
                        "" + "");
            }
            if (!powerEffectNameEmpty)
            {
                if (string.IsNullOrEmpty(propList[selectedPropellant].runningEffectName))
                {
                    EngineEffectNode.RemoveValue("powerEffectName");
                    moduleEngine.DeactivatePowerFX();
                }
                else
                {
                    EngineEffectNode.AddValue("powerEffectName", propList[selectedPropellant].powerEffectName);
                    moduleEngine.ActivatePowerFX();
                }



                
                Debug.Log("propList[selectedPropellant].powerEffectName: " + propList[selectedPropellant].powerEffectName);
            }
            if (!spoolEffectNameEmpty)
            {
                EngineEffectNode.AddValue("spoolEffectName", propList[selectedPropellant].spoolEffectName);
                Debug.Log("propList[selectedPropellant].spoolEffectName: " + propList[selectedPropellant].spoolEffectName);
            }



            moduleEngine.Load(EngineEffectNode);
            moduleEngine.InitializeFX();
            
            //moduleEngine.FXReset();
            //moduleEngine.FXUpdate();
            //moduleEngine.
            //}
        }
    }
}
