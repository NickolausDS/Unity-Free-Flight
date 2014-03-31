using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FlightBody)) ]
public class FlightBodyInspector : Editor {
	
	FlightBody fBody;
	PropertyField[] m_fields;

//	void OnEnable() {
//		fBody = (FlightBody)target;
//	}
	
	public void OnEnable()
	{
		fBody = target as FlightBody;
		m_fields = ExposeProperties.GetProperties( fBody );
	}
	
	public override void OnInspectorGUI () {
		
		if ( fBody == null )
			return;
		
		this.DrawDefaultInspector();
		
		ExposeProperties.Expose( m_fields );
		
	}
}
