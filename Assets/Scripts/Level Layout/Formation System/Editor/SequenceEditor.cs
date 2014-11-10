using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Sequence))]
public class SequenceEditor : Editor {
	
	public override void OnInspectorGUI() {
		Sequence sequence = target as Sequence;
		sequence.DisplaySequence();

		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
	}
}
