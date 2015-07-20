using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Formation))]
public class FormationEditor : Editor {

	public override void OnInspectorGUI() {
		Formation formation = target as Formation;
		formation.ShowGUI();

		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
	}
}
