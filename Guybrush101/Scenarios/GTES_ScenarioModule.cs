//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;

//namespace GTI
//{
//    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.TRACKSTATION, GameScenes.SPACECENTER)]
//    public class GTES_ScenarioModule : ScenarioModule
//    {
//        public override void OnSave(ConfigNode node)
//        {
//            try
//            {
//                GTI_ES2_ConfigNodes ES2nodes = new GTI_ES2_ConfigNodes();
                
//                //ES2_upgrades is for upgrades for EngineClassSwitch_2 partModule
//                ConfigNode upgradeNodes = new ConfigNode("ES2_upgrades");
//                upgradeNodes.AddNode(ES2nodes.createConfigNode("newNode"));


//                //Save data in the persistent file
//                //node.AddNode(upgradeNodes);

//                Debug.Log("[GTI] OnSave()");


//                //double time = DateTime.Now.Ticks;
//                //ConfigNode upgradeNodes = new ConfigNode("upgrades");
//                //foreach (string upgradeName in KRnD.upgrades.Keys)
//                //{
//                //    KRnDUpgrade upgrade;
//                //    if (!KRnD.upgrades.TryGetValue(upgradeName, out upgrade)) continue;
//                //    upgradeNodes.AddNode(upgrade.createConfigNode(upgradeName));
//                //    Debug.Log("[KRnD] saved: " + upgradeName + " " + upgrade.ToString());
//                //}
//                //node.AddNode(upgradeNodes);

//                //time = (DateTime.Now.Ticks - time) / TimeSpan.TicksPerSecond;
//                //Debug.Log("[KRnD] saved " + upgradeNodes.CountNodes.ToString() + " upgrades in " + time.ToString("0.000s"));

//                //ConfigNode guiSettings = new ConfigNode("gui");
//                //guiSettings.AddValue("left", KRnDGUI.windowPosition.xMin);
//                //guiSettings.AddValue("top", KRnDGUI.windowPosition.yMin);
//                //node.AddNode(guiSettings);
//            }
//            catch (Exception e)
//            {
//                Debug.LogError("[GTI] OnSave(): " + e.ToString());
//            }
//        }

//        public override void OnLoad(ConfigNode node)
//        {





//        }
//    }

//    public class GTI_ES2_ConfigNodes
//    {
//        public ConfigNode createConfigNode(string name)
//        {
//            ConfigNode node = new ConfigNode(name);
//            node.AddValue("newValueKey", "newValue");



//            //ConfigNode node = new ConfigNode(name);
//            //if (this.ispVac > 0) node.AddValue(ISP_VAC, this.ispVac.ToString());
//            //if (this.ispAtm > 0) node.AddValue(ISP_ATM, this.ispAtm.ToString());
//            //if (this.dryMass > 0) node.AddValue(DRY_MASS, this.dryMass.ToString());
//            //if (this.fuelFlow > 0) node.AddValue(FUEL_FLOW, this.fuelFlow.ToString());
//            //if (this.torque > 0) node.AddValue(TORQUE, this.torque.ToString());
//            //if (this.chargeRate > 0) node.AddValue(CHARGE_RATE, this.chargeRate.ToString());
//            //if (this.crashTolerance > 0) node.AddValue(CRASH_TOLERANCE, this.crashTolerance.ToString());
//            //if (this.batteryCharge > 0) node.AddValue(BATTERY_CHARGE, this.batteryCharge.ToString());
//            //if (this.generatorEfficiency > 0) node.AddValue(GENERATOR_EFFICIENCY, this.generatorEfficiency.ToString());
//            //if (this.converterEfficiency > 0) node.AddValue(CONVERTER_EFFICIENCY, this.converterEfficiency.ToString());
//            //if (this.parachuteStrength > 0) node.AddValue(PARACHUTE_STRENGTH, this.parachuteStrength.ToString());
//            //if (this.maxTemperature > 0) node.AddValue(MAX_TEMPERATURE, this.maxTemperature.ToString());
//            //if (this.fuelCapacity > 0) node.AddValue(FUEL_CAPACITY, this.fuelCapacity.ToString());
//            return node;
//        }

//        /* NOTICE IT USES THE CUSTOM TYPE IT WANT'S TO RETURN VALUES IN
//        public static KRnDUpgrade createFromConfigNode(ConfigNode node)
//        {




//            //KRnDUpgrade upgrade = new KRnDUpgrade();
//            //if (node.HasValue(ISP_VAC)) upgrade.ispVac = Int32.Parse(node.GetValue(ISP_VAC));
//            //if (node.HasValue(ISP_ATM)) upgrade.ispAtm = Int32.Parse(node.GetValue(ISP_ATM));
//            //if (node.HasValue(DRY_MASS)) upgrade.dryMass = Int32.Parse(node.GetValue(DRY_MASS));
//            //if (node.HasValue(FUEL_FLOW)) upgrade.fuelFlow = Int32.Parse(node.GetValue(FUEL_FLOW));
//            //if (node.HasValue(TORQUE)) upgrade.torque = Int32.Parse(node.GetValue(TORQUE));
//            //if (node.HasValue(CHARGE_RATE)) upgrade.chargeRate = Int32.Parse(node.GetValue(CHARGE_RATE));
//            //if (node.HasValue(CRASH_TOLERANCE)) upgrade.crashTolerance = Int32.Parse(node.GetValue(CRASH_TOLERANCE));
//            //if (node.HasValue(BATTERY_CHARGE)) upgrade.batteryCharge = Int32.Parse(node.GetValue(BATTERY_CHARGE));
//            //if (node.HasValue(GENERATOR_EFFICIENCY)) upgrade.generatorEfficiency = Int32.Parse(node.GetValue(GENERATOR_EFFICIENCY));
//            //if (node.HasValue(CONVERTER_EFFICIENCY)) upgrade.converterEfficiency = Int32.Parse(node.GetValue(CONVERTER_EFFICIENCY));
//            //if (node.HasValue(PARACHUTE_STRENGTH)) upgrade.parachuteStrength = Int32.Parse(node.GetValue(PARACHUTE_STRENGTH));
//            //if (node.HasValue(MAX_TEMPERATURE)) upgrade.maxTemperature = Int32.Parse(node.GetValue(MAX_TEMPERATURE));
//            //if (node.HasValue(FUEL_CAPACITY)) upgrade.fuelCapacity = Int32.Parse(node.GetValue(FUEL_CAPACITY));
//            return upgrade;
//        }
//        */
//    }
//}
