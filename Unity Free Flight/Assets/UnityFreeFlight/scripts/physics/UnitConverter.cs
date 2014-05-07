using UnityEngine;
using System.Collections;
using System;

/*
 * Never worry about which units to use ever again! Unit Converter has you covered!
 * 
 * This class was created to reduce the need for users to adhere to a specific unit type.
 * If you're comfortable using feet, use feet, ect. 
 * 
 * By default, the converter uses metric as a 'base' and converts everything else to and
 * from metric. It's intended that all physics (or computationally complex) math is done
 * using metric, and any time an End User wants a given number in something else, this 
 * converts it for them. 
 * 
 */
public class UnitConverter {

	/*
	 * On adding new Units:
	 * 	1. Add your units to the unit enum
	 *  2. add all the names and abreviations to the list below
	 * 		Your new list must match the length of the others!
	 * 	3. Add formula conversions, from metric to your specific type
	 * 	4. Add another list of conversion methods. If it's a linear conversion, 
	 * 		you may use the simple linear conversion methods, otherwise you need
	 * 		to write your own method for getting and setting the type to and from metric
	 * 
	 * On adding new Types
	 * 
	 * Pretty much the same as the above. Make sure you fill in:
	 * 	1. The name of the unit in "Types"
	 * 	2. The name and abreviation in string form
	 * 	3. The formula for converting to and from metric
	 * 	4. the get and set conversion methods
	 * Additionally, you may want to add another convienience method at
	 * the bottom for your type (such as getLengthType).
	 */ 

	public enum Units{ Metric, Imperial };
	public enum Types { Length, Area, Weight, Force };

	public static string[,] names = {
		{"Meters", "Square Meters", "Kilograms", "Newtons"},
		{"Feet", "Square Feet", "Pounds", "Force Pounds"}
	};

	public static string[,] abbreviations = {
		{"M", "M^2", "KG", "N"},
		{"FT", "FT^2", "LB", "LBf"}
	};
	
	static float[,] formulas = new float[,] {
		//We store everything in metric, so any metric conversions are always 1.
		{1.0f, 1.0f, 1.0f, 1.0f},
		//meters to feet, sqm to sqft, kg to lb, Newtons to lbForce 
		{ 3.28084f, 10.7640f, 2.20462f, 0.22481f }
	};

	//Ignore this if you are adding a new type or new unit
	private delegate float linearConv (float value, float formula);

	//Get and set conversions
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
			return abbreviations [(int) theUnit, (int) theType];
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
