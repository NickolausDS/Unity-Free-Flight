using UnityEngine;
using System.Collections;
using System;


public class UnitConverter {

	public enum Units{ Metric, Imperial };
	public enum Types { Length, Area, Weight, Force };

	public static string[] metric = {"M", "M^2", "KG", "N"};
	public static string[] imperial = {"FT", "FT^2", "LB", "LBf"};
	static float[,] formulas = new float[,] {
		//We store everything in metric, so any metric conversions are always 1.
		{1.0f, 1.0f, 1.0f},
		//meters to feet, sqm to sqft, kg to lb, Newtons to lbForce 
		{ 3.28084f, 10.7640f, 2.20462f, 0.22481f }
	};

	private delegate float linearConv (float value, float formula);
	static linearConv[,] getConversions = new linearConv[,] { 
		{ reflexive, reflexive, reflexive, reflexive },		
		{ linearGetConversion, linearGetConversion, linearGetConversion, linearGetConversion }
	};

	static linearConv[,] setConversions = new linearConv[,] { 
		{ reflexive, reflexive, reflexive, reflexive },		
		{ linearSetConversion, linearSetConversion, linearSetConversion, linearSetConversion }
	};

	public static float getConv(Units theUnit, Types theType, float theValue) {
		float theFormula = formulas [(int) theUnit, (int) theType];
		return getConversions 		[(int) theUnit, (int) theType] (theValue, theFormula);
	}

	public static float setConv(Units theUnit, Types theType, float theValue) {
		float theFormula = formulas [(int) theUnit, (int) theType];
		return setConversions 		[(int) theUnit, (int) theType] (theValue, theFormula);
	}

	public static float convert(Units fromUnit, Units toUnit, Types theType, float theValue) {
		//Set Conversion to metric, so we can convert it to anything else
		float val = setConv(fromUnit, theType, theValue);
		//Return metric to whatever conversion
		return getConv (toUnit, theType, val);
	}

	static float linearGetConversion (float value, float formula) {
		return value * formula;
	}
	
	static float linearSetConversion (float value, float formula) {
		return value / formula;
	}
	
	private static float reflexive (float v, float f) {
		return v;
	}

	public Units _unit = Units.Metric;

	public UnitConverter(Units theunit) {
		_unit = theunit;
	}

	public UnitConverter() {_unit = Units.Metric;}

	public string getLengthType() {
		return getUnitType (Types.Length);
	}
	public string getAreaType() {
		return getUnitType (Types.Area);
	}
	public string getWeightType() {
		return getUnitType (Types.Weight);
	}

	public string getUnitType(Types thetype) {
			if (_unit == Units.Metric) {
						return metric [(int)thetype];
				} else if (_unit == Units.Imperial) {
						return imperial [(int)thetype];
				} else {
						throw new Exception ("Tried to get unit type that doesn't exist");
				}
	}

}
