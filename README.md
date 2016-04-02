# KSP plugin Engine Switcher

This is a plugin for Kerbal Space Program version 1.1.
The purpose is to introduce an alternative way to switch engine configuration (propellants and other properties).

Features:
- Multiengine switch


CFG:
	
	MODULE
	{
		name = GTI_EngineClassSwitch
		
		//Generel Scoping
		//** Only single values and optional
		
		//engineID = AirBreathing				//Scope of implementation
		availableInFlight = true
		availableInEditor = true
		
		
		//Propellant Level declarations (mandatory)
		//** Values split on setup level ";", and on propellant split ","
		
		propellantNames = LiquidFuel,IntakeAir;LiquidFuel,IntakeAir;LiquidFuel
		propellantRatios = 1,1;1,1;1
		
		propellantIgnoreForIsp = false,false;false,true;false
		propellantDrawGauge = true,false;true,false;true
		
		
		//Engine level declarations
		//**Values split on setup level ";"
		
		iniGUIpropellantNames = Cruise LF;Normal LF;Boost LF
		//engineNames = "NOT IMPLEMENTED"
		EngineTypes =  Turbine;Turbine;Turbine
		maxThrust = 105;105;105
		heatProduction =  60;60;60
		
		
		//Curves (mandatory)
		//** These are split in setup level "|", key pair level ";", and on keys " "
		//** splitters are different because base functionaly is different, plus
		//** these are normally notated in a table format with white spaces in the keys
		
		atmosphereCurveKeys = 0 2000;1 4000|0 2000;1 4000|0 2000;1 4000
		velCurveKeys = 0 1|0 1|0 1
		atmCurveKeys = 0 1;1 1|0 1;1 1|0 1;1 1
	}
	
