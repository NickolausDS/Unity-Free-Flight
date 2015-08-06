using System;
using System.Reflection;
using System.Collections.Generic;

public class UnityTypeResolver {

	/// <summary>
	/// Gets all the derived classes of a given base class. This is useful
	/// when you have editor code that needs access to types in engine code.
	/// </summary>
	/// <returns>The all sub types.</returns>
	/// <param name="aBaseClass">A base class.</param>
	public static List<Type> GetAllSubTypes(Type aBaseClass) {
		var result = new List<Type>();
		Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
		foreach (var A in AS) {
			Type[] types = A.GetTypes();
			foreach (var T in types) {
				if (T.IsSubclassOf(aBaseClass))
					result.Add(T);
			}
		}
		return result;
	}

}