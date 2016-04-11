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


        public void InitializeEffects()
        {





        }

        public void updateEngineModuleEffects(bool calledByPlayer, string callingFunction = "player")
        {

        }
    }
}
