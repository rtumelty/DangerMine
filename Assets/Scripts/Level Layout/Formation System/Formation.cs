#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Formation : ScriptableObject {
	public string name;
	public int interval;
	public int width;
	public GridCoordinate[] slots;

	public void Initialize() {
		
		if (slots == null)
			slots = new GridCoordinate[0];
	}

#if UNITY_EDITOR
	public bool expandSlots = false;

	public void ShowGUI() {
		interval = EditorGUILayout.IntField("Interval:", interval);
		width = EditorGUILayout.IntField("Width:", width);

		if (slots == null) slots = new GridCoordinate[0];
		
		expandSlots = EditorGUILayout.Foldout(expandSlots, "Slot coordinates:");
		if (expandSlots) {
			int newSize = EditorGUILayout.IntField("Size", slots.Length);
			
			if (newSize != slots.Length) {
				GridCoordinate[] newCoords = new GridCoordinate[newSize];
				for (int i = 0; i < newCoords.Length && i < slots.Length; i++)
					newCoords[i] = slots[i];
				
				slots = newCoords;
			}
			
			for (int i = 0; i < slots.Length; i++) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();

				
				EditorGUILayout.BeginVertical(EditorStyles.textArea);
				if (slots[i] == null) slots[i] = new GridCoordinate(0,0);
				EditorGUILayout.LabelField("Slot " + i);
				slots[i].x = EditorGUILayout.IntField("X", slots[i].x);
				slots[i].y = EditorGUILayout.IntField("Y", slots[i].y);
				
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}
		}

		EditorUtility.SetDirty(this);
	}
#endif
}
