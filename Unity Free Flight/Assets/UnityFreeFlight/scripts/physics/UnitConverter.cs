using UnityEngine;
using System.Collections;
using System;


public class UnitConverter {

	public enum Units{ Metric, Imperial };
	public enum Types { Length, Area, Weight, Force };

	public static string[,] names = {
		{"Meters", "Square Meters", "Kilograms", "Newtons"},
		{"Feet", "Square Feet", "Pounds", "Force Pounds"}
	};

	public static string[,] abreviations = {
		{"M", "M^2", "KG", "N"},
		{"FT", "FT^2", "LB", "LBf"}
	};

//	public static string[] metric = {"M", "M^2", "KG", "N"};
//	public static string[] imperial = {"FT", "FT^2", "LB", "LBf"};
	static float[,] formulas = new float[,] {
		//We store everything in metric, so any metric conversions are always 1.
		{1.0f, 1.0f, 1.0f, 1.0f},
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

	public string getTypeName(Units theUnit, Types theType, bool abrev=true){
		if (abrev)
			return abreviations [(int) theUnit, (int) theType];
		return names[(int) theUnit, (int) theType];
	}

	public string getTypeName(Types thetype, bool abrev=true) {
		return getTypeName (_unit, thetype, abrev);
	}

	public string getLengthType(bool abrev=true) {
		return getTypeName (Types.Length, abrev);
	}

	public string getAreaType(bool abrev=true) {
		return getTypeName (Types.Area, abrev);
	}

	public string getWeightType(bool abrev=true) {
		return getTypeName (Types.Weight, abrev);
	}

	public string getForceType(bool abrev=true) {
		return getTypeName (Types.Force, abrev);
	}

}
