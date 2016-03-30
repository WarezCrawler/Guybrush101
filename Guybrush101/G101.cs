using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using UnityEngine;

namespace Guybrush101
{
    /// <summary>
    /// My first part!
    /// </summary>
    public class PropulsionTech : PartModule
    {
        //private List<ModuleEngines> ModuleEngines;
        //[KSPField(isPersistant = true)]
        //float originalThrust = 0;
        [KSPField]
        public string engineID;


        [KSPField(isPersistant = true)]
        bool booToggle = false;
        private string info;

        float Thrustpct = 50;
        float MaxThrust = 100;
        float maxFuelFlow = 0;

        private ModuleEngines engineModule;
        private List<ModuleEngines> ModuleEngines;

        [KSPEvent(active = true, guiActive = true, guiActiveEditor = true, guiName = "THRUST")]
        public void toggleupdate()
        {
            //Manipulate this parent part of the module | this.part.####
            //Get the engines module of the part
            ModuleEngines engineModule = PropulsionTech.getEnginesModule(this.part);

            //MaxThrust = engineModule.maxThrust;
            
            MaxThrust = ToggleMaxThrust(engineModule, Thrustpct, ref booToggle);
            //engineModule.propellants.

            //Inverse the boolean indicator
            //booToggle = !booToggle;

            //#### DEBUG ####
            info = "Burst toggleupdate\n";
            info += "MaxThrust " + MaxThrust + "\n";
            info += "maxFuelFlow " + maxFuelFlow + "\n";
            info += "booToggle " + booToggle;
            Debug.Log(info);
            //#### DEBUG ####

            //Update the event text
            Events["toggleupdate"].guiName = (booToggle ? "MaxThrust (burst) " + MaxThrust : "MaxThrust (base) " + MaxThrust);

        }

        //This function sets/unsets burst, and returns the new thrust value
        private float ToggleMaxThrust(ModuleEngines module, float Thrustpct, ref bool currentThrustState)
        {
            if (currentThrustState == true)
            {
                //i.e. if burst, then return the engine to normal state
                module.maxThrust = module.maxThrust / (1 + Thrustpct / 100);
                module.maxFuelFlow = module.maxFuelFlow / (1 + Thrustpct / 100);
                maxFuelFlow = module.maxFuelFlow;
                currentThrustState = false;                   //true = burst | false = normal
            }
            else
            {
                //i.e. if not burst, then burst the engine
                module.maxThrust = module.maxThrust * (1 + Thrustpct / 100);
                module.maxFuelFlow = module.maxFuelFlow * (1 + Thrustpct / 100);
                maxFuelFlow = module.maxFuelFlow;
                currentThrustState = true;
            }
            return module.maxThrust;
        }
        private void StartBurst(ModuleEngines module, float Thrustpct, ref bool currentThrustState)
        {

        }
        private void StopBurst(ModuleEngines module, float Thrustpct, ref bool currentThrustState)
        {

        }

        public static ModuleEngines getEnginesModule(Part part)
        {
            foreach (PartModule partModule in part.Modules)
            {
                if (partModule.moduleName == "ModuleEngines" || partModule.moduleName == "ModuleEnginesFX") return (ModuleEngines)partModule;
            }
            return null;
        }

        //Initialize the event text based on the input
        public override void OnStart(PartModule.StartState state)
        {
            ModuleEngines engineModule = PropulsionTech.getEnginesModule(this.part);
            MaxThrust = engineModule.maxThrust;

            //#### DEBUG ####
            //info = "Burst OnStart\n";
            //info += "MaxThrust " + MaxThrust + "\n";
            //info += "maxFuelFlow " + maxFuelFlow + "\n";
            //info += "booToggle " + booToggle;
            //Debug.LogError(info);
            //#### DEBUG ####

            //                                                  true                                false
            Events["toggleupdate"].guiName = (booToggle ? "MaxThrust (burst) " + MaxThrust : "MaxThrust (base) " + MaxThrust);
        }

        ////Infomation
        public override string GetInfo()
        {
            if (info == null)
            {
                info = "Burst\n";
                info += "MaxThrust " + MaxThrust + "\n";
                info += "maxFuelFlow " + maxFuelFlow + "\n";
                info += "booToggle " + booToggle;
            }
            return info;
        }



        /// <summary>
        /// Called when the part is started by Unity.
        /// </summary>
        //public override void OnStart(StartState state)
        //{
        //    // Add stuff to the log
        //    //print("Hello, Kerbin!");
        //    //ModuleEngines = part.FindModulesImplementing<ModuleEngines>();
        //    //ModuleEngines = part.FindModulesImplementing<ModuleEngines>();

        //    //foreach (var moduleEngine in ModuleEngines)
        //    //{
        //    //    //if (moduleEngine.engineID == engineID)  // || engineID.IsNullOrWhiteSpace())
        //    //    if (moduleEngine.engineID == "")
        //    //    {

        //    //    }
        //    //}
        //}
    }
}
