using UnityEditor;

namespace UnityFreeFlight {

	[CustomEditor (typeof (FreeFlight) )]
	public class FreeFlightEditor : UnityEditor.Editor {
		
		public override void OnInspectorGUI () {

			serializedObject.Update ();
			EditorGUILayout.PropertyField( serializedObject.FindProperty ("modeManager"));
			serializedObject.ApplyModifiedProperties ();
		}
	}

}
