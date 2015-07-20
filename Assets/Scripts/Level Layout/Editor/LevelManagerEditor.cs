using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor {

	public override void OnInspectorGUI() {
		LevelManager levelManager = target as LevelManager;
		levelManager.ShowGUI(this);
	}
}
