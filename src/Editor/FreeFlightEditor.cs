using UnityEditor;

namespace UnityFreeFlight {

	[CustomEditor (typeof (FreeFlight) )]
	public class FreeFlightEditor : UnityEditor.Editor {
		
		public override void OnInspectorGUI () {

			serializedObject.Update ();
			EditorGUILayout.PropertyField( serializedObject.FindProperty ("modeManager"));
			EditorGUILayout.LabelField( "Misc Options" );
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField( serializedObject.FindProperty ("disableOnError"));
			EditorGUI.indentLevel--;
			serializedObject.ApplyModifiedProperties ();
		}
	}

}
