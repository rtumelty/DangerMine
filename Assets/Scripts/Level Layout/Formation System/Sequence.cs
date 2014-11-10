using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Sequence : SpawnGroup {
	public List<Formation> formations;

	public void DisplaySequence() {
		if (formations == null) formations = new List<Formation>();

		name = EditorGUILayout.TextField("Name", name);

		EditorGUILayout.Space();
		
		minimumDistance = EditorGUILayout.IntField("Min Distance", minimumDistance);
		maximumDistance = EditorGUILayout.IntField("Max Distance", maximumDistance);

		EditorGUILayout.Space();

		probabilityWeight = EditorGUILayout.FloatField("Probability Weight", probabilityWeight);

		EditorGUILayout.Space();

		int newWidth = 0;
		EditorGUILayout.LabelField("Formations:");
		
		for (int i = 0; i < formations.Count; i++) {
			EditorGUILayout.BeginHorizontal();

				formations[i] = EditorGUILayout.ObjectField(formations[i], typeof(Formation), false) as Formation;
				if (GUILayout.Button("-", GUILayout.MaxWidth(15))) {
					formations.RemoveAt(i);
				}

			EditorGUILayout.EndHorizontal();

			if (formations[i] != null)
				newWidth += formations[i].width + formations[i].interval;
		}

		if (GUILayout.Button("Add formation")) {
			formations.Add(null);
		}

		width = newWidth;
	}
}
