using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Sequence))]
public class SequenceEditor : Editor {
	
	public override void OnInspectorGUI() {
		Sequence sequence = target as Sequence;
		DisplaySequence(sequence);

		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
	}

	public void DisplaySequence(Sequence sequence) {
		if (sequence.formations == null) sequence.formations = new List<Formation>();
		
		sequence.name = EditorGUILayout.TextField("Name", sequence.name);
		
		EditorGUILayout.Space();
		
		sequence.minimumDistance = EditorGUILayout.IntField("Min Distance", sequence.minimumDistance);
		sequence.maximumDistance = EditorGUILayout.IntField("Max Distance", sequence.maximumDistance);
		
		EditorGUILayout.Space();
		
		sequence.probabilityWeight = EditorGUILayout.FloatField("Probability Weight", sequence.probabilityWeight);
		
		EditorGUILayout.Space();
		
		int newWidth = 0;
		EditorGUILayout.LabelField("Formations:");
		
		for (int i = 0; i < sequence.formations.Count; i++) {
			EditorGUILayout.BeginHorizontal();
			
			sequence.formations[i] = EditorGUILayout.ObjectField(sequence.formations[i], typeof(Formation), false) as Formation;
			if (GUILayout.Button("-", GUILayout.MaxWidth(15))) {
				sequence.formations.RemoveAt(i);
			}
			
			EditorGUILayout.EndHorizontal();
			
			if (sequence.formations[i] != null)
				newWidth += sequence.formations[i].width + sequence.formations[i].interval;
		}
		
		if (GUILayout.Button("Add formation")) {
			sequence.formations.Add(null);
		}
		
		sequence.width = newWidth;
	}

}
