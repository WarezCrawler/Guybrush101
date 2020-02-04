using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GTI
{
    public class GTI_ModuleAsteroidDrill : BaseDrill
    {
        [KSPField(isPersistant = true)]
        public bool DirectAttach;

        [KSPField]
        public float PowerConsumption = 1f;

        [KSPField]
        public bool RockOnly;

        protected bool _isValidSituation = true;

        protected Transform impactTransformCache;

        private ConversionRecipe recipe = new ConversionRecipe();

        private double _drilledMass;

        private string _status;

        private Part _potato;

        private ModuleAsteroidInfo _info;

        private static string cacheAutoLOC_258405;
        private static string cacheAutoLOC_258412;
        private static string cacheAutoLOC_258419;
        private static string cacheAutoLOC_258428;
        private static string cacheAutoLOC_258436;
        private static string cacheAutoLOC_258443;
        private static string cacheAutoLOC_258451;
        private static string cacheAutoLOC_258501;

        //# autoLOC_258405 = No asteroid detected
        //# autoLOC_258412 = Not directly attached to asteroid
        //# autoLOC_258419 = No resource data
        //# autoLOC_258428 = No surface impact
        //# autoLOC_258436 = No info
        //# autoLOC_258443 = Resources Depleted
        //# autoLOC_258451 = Insufficient Power

        public GTI_ModuleAsteroidDrill()
        {
        }

        internal static void CacheLocalStrings()
        {
            //# autoLOC_258405 = No asteroid detected
            //# autoLOC_258412 = Not directly attached to asteroid
            //# autoLOC_258419 = No resource data
            //# autoLOC_258428 = No surface impact
            //# autoLOC_258436 = No info
            //# autoLOC_258443 = Resources Depleted
            //# autoLOC_258451 = Insufficient Power
            //#autoLOC_258501 = No Storage Space
            cacheAutoLOC_258405 = "No asteroid detected";
            cacheAutoLOC_258412 = "Not directly attached to asteroid";
            cacheAutoLOC_258419 = "No resource data";
            cacheAutoLOC_258428 = "No surface impact";
            cacheAutoLOC_258436 = "No info";
            cacheAutoLOC_258443 = "Resources Depleted";
            cacheAutoLOC_258451 = "Resources Depleted";
            cacheAutoLOC_258501 = "No Storage Space";
        }

        protected virtual bool CheckForImpact()
        {
            RaycastHit raycastHit;
            if (this.ImpactTransform == string.Empty || this.impactTransformCache == null)
            {
                return true;
            }
            Vector3 vector3 = this.impactTransformCache.position;
            Ray ray = new Ray(vector3, this.impactTransformCache.forward);
            ModuleAsteroid componentUpwards = null;
            if (Physics.Raycast(ray, out raycastHit, this.ImpactRange))
            {
                componentUpwards = raycastHit.collider.gameObject.GetComponentUpwards<ModuleAsteroid>();
            }
            return componentUpwards != null;
        }

        protected virtual Part GetAttachedPotato()
        {
            ModuleAsteroid moduleAsteroid = null;
            int count = base.vessel.parts.Count;
            do
            {
                int num = count;
                count = num - 1;
                if (num <= 0)
                {
                    return null;
                }
                moduleAsteroid = base.vessel.parts[count].FindModuleImplementing<ModuleAsteroid>();
            }
            while (moduleAsteroid == null);
            return moduleAsteroid.part;
        }

        public override string GetInfo()
        {
            StringBuilder stringBuilder = StringBuilderCache.Acquire(256);
            stringBuilder.Append(this.ConverterName);
            stringBuilder.Append("\n");
            //#autoLOC_6001045 = <color=#BADA55>(Asteroid use - <<1>>% base efficiency)</color>
            //stringBuilder.Append(Localizer.Format("#autoLOC_6001045", new object[] { (int)(this.Efficiency * 100f) }));
            stringBuilder.Append(String.Format("<color=#BADA55>(Asteroid use - <<1>>% base efficiency)</color>", new object[] { (int)(this.Efficiency * 100f) }));
            //#autoLOC_258587 = \n\n<color=#99FF00>Power Consumption:</color>
            //stringBuilder.Append(Localizer.Format("#autoLOC_258587"));
            stringBuilder.Append(String.Format("\n\n<color=#99FF00>Power Consumption:</color>"));
            stringBuilder.Append("\n - ");
            //# autoLOC_501004 = Electric Charge
            //stringBuilder.Append(Localizer.Format("#autoLOC_501004"));
            stringBuilder.Append(String.Format("Electric Charge"));
            stringBuilder.Append(": ");
            if ((double)(this.PowerConsumption * this.Efficiency) < 0.0001)
            {
                string[] str = new string[1];
                float powerConsumption = this.PowerConsumption * (float)KSPUtil.dateTimeFormatter.Day * this.Efficiency;
                str[0] = powerConsumption.ToString("0.00");
                //#autoLOC_6001046 = <<1>>/day
                //stringBuilder.Append(Localizer.Format("#autoLOC_6001046", str));
                stringBuilder.Append(String.Format("<<1>>/day", str));
            }
            else if ((double)(this.PowerConsumption * this.Efficiency) >= 0.01)
            {
                string[] strArrays = new string[1];
                float single = this.PowerConsumption * this.Efficiency;
                strArrays[0] = single.ToString("0.00");
                //#autoLOC_6001048 = <<1>>/sec
                //stringBuilder.Append(Localizer.Format("#autoLOC_6001048", strArrays));
                stringBuilder.Append(String.Format("<<1>>/sec", strArrays));
            }
            else
            {
                string[] str1 = new string[1];
                float powerConsumption1 = this.PowerConsumption * (float)KSPUtil.dateTimeFormatter.Hour * this.Efficiency;
                str1[0] = powerConsumption1.ToString("0.00");
                //#autoLOC_6001047 = <<1>>/hr
                //stringBuilder.Append(Localizer.Format("#autoLOC_6001047", str1));
                stringBuilder.Append(String.Format("<<1>>/hr", str1));
            }
            return stringBuilder.ToStringAndRelease();
        }

        public override string GetModuleDisplayName()
        {
            //#autoLoc_6003029 = Asteroid Drill
            //return Localizer.Format("#autoLoc_6003029");
            return "Asteroid Drill";
        }

        public override bool IsSituationValid()
        {
            int count = base.vessel.parts.Count;
            Label0:
            int num = count;
            count = num - 1;
            if (num <= 0)
            {
                return false;
            }
            Part item = base.vessel.parts[count];
            int count1 = item.Modules.Count;
            do
            {
                int num1 = count1;
                count1 = num1 - 1;
                if (num1 <= 0)
                {
                    goto Label0;
                }
            }
            while (!(item.Modules[count1] is ModuleAsteroid));
            return true;
        }

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            if (!HighLogic.LoadedSceneIsFlight)
            {
                return;
            }
            this._isValidSituation = this.IsSituationValid();
            this.isEnabled = this._isValidSituation;
            this._preCalculateEfficiency = true;
            this.impactTransformCache = base.part.FindModelTransform(this.ImpactTransform);
        }

        public override void OnUpdate()
        {
            bool flag = this.IsSituationValid();
            bool flag1 = flag;
            if (flag != this._isValidSituation)
            {
                this._isValidSituation = flag1;
                this.isEnabled = this._isValidSituation;
                MonoUtilities.RefreshContextWindows(base.part);
            }
            if (!this._isValidSituation && this.isEnabled)
            {
                this.isEnabled = false;
                MonoUtilities.RefreshContextWindows(base.part);
            }
            base.OnUpdate();
        }

        protected override void PostProcess(ConverterResults result, double deltaTime)
        {
            base.PostProcess(result, deltaTime);
            if (this._drilledMass < 1E-09)
            {
                return;
            }
            if (this._potato == null || this._info == null)
            {
                return;
            }
            double num = this._info.currentMassVal;          // - this._drilledMass;            //Take no mass of the asteroid
            this._info.currentMassVal = num;
            this.status = this._status;
        }

        protected override ConversionRecipe PrepareRecipe(double deltaTime)
        {
            this._status = "Connected";
            this._potato = this.GetAttachedPotato();
            this._drilledMass = 0;
            if (this._potato == null)
            {
                this._status = cacheAutoLOC_258405;
                this.IsActivated = false;
                return null;
            }
            if (this.DirectAttach && !base.part.children.Contains(this._potato) && !(base.part.parent == this._potato))
            {
                this._status = cacheAutoLOC_258412;
                this.IsActivated = false;
                return null;
            }
            if (!this.RockOnly && !this._potato.Modules.Contains("ModuleAsteroidResource"))
            {
                this._status = cacheAutoLOC_258419;
                this.IsActivated = false;
                return null;
            }
            List<ModuleAsteroidResource> moduleAsteroidResources = this._potato.FindModulesImplementing<ModuleAsteroidResource>();
            if (!this.CheckForImpact())
            {
                this._status = cacheAutoLOC_258428;
                this.IsActivated = false;
                return null;
            }
            this._info = this._potato.FindModuleImplementing<ModuleAsteroidInfo>();
            if (this._info == null)
            {
                this._status = cacheAutoLOC_258436;
                this.IsActivated = false;
                return null;
            }
            if (this._info.massThresholdVal >= this._info.currentMassVal)
            {
                this._status = cacheAutoLOC_258443;
                this.IsActivated = false;
                return null;
            }
            if (this.ResBroker.AmountAvailable(base.part, "ElectricCharge", deltaTime, ResourceFlowMode.NULL) <= deltaTime * (double)this.PowerConsumption)
            {
                this._status = cacheAutoLOC_258451;
                this.IsActivated = false;
                return null;
            }
            this.UpdateConverterStatus();
            if (!this.IsActivated)
            {
                return null;
            }
            double efficiencyMultiplier = this.GetEfficiencyMultiplier();
            bool flag = false;
            int count = moduleAsteroidResources.Count;
            int num = 0;
            while (true)
            {
                if (num < count)
                {
                    ModuleAsteroidResource item = moduleAsteroidResources[num];
                    if (this.ResBroker.StorageAvailable(base.part, item.resourceName, deltaTime, item._flowMode, (double)this.FillAmount) > deltaTime * (double)item.abundance * (double)this.Efficiency * efficiencyMultiplier)
                    {
                        flag = true;
                        break;
                    }
                    else
                    {
                        num++;
                    }
                }
                else
                {
                    break;
                }
            }
            this.recipe.Clear();
            if (!flag)
            {
                this._status = cacheAutoLOC_258501;
            }
            else
            {
                List<ResourceRatio> inputs = this.recipe.Inputs;
                ResourceRatio resourceRatio = new ResourceRatio()
                {
                    ResourceName = "ElectricCharge",
                    Ratio = (double)this.PowerConsumption,
                    FlowMode = ResourceFlowMode.NULL
                };
                inputs.Add(resourceRatio);
                for (int i = 0; i < count; i++)
                {
                    ModuleAsteroidResource moduleAsteroidResource = moduleAsteroidResources[i];
                    if ((double)moduleAsteroidResource.abundance > 1E-09)
                    {
                        PartResourceDefinition definition = PartResourceLibrary.Instance.GetDefinition(moduleAsteroidResource.resourceName);
                        double num1 = deltaTime * (double)moduleAsteroidResource.abundance * (double)this.Efficiency * efficiencyMultiplier;
                        double num2 = this._info.currentMassVal - this._info.massThresholdVal;
                        double num3 = num2 / (double)definition.density;
                        double num4 = Math.Min(num1, num3);
                        GTI_ModuleAsteroidDrill moduleAsteroidDrill = this;
                        moduleAsteroidDrill._drilledMass = moduleAsteroidDrill._drilledMass + (double)definition.density * num4;
                        ResourceRatio resourceRatio1 = new ResourceRatio()
                        {
                            ResourceName = moduleAsteroidResource.resourceName,
                            Ratio = num4 / deltaTime,
                            DumpExcess = true,
                            FlowMode = ResourceFlowMode.NULL
                        };
                        this.recipe.Outputs.Add(resourceRatio1);
                    }
                }
            }
            return this.recipe;
        }
    }
}
