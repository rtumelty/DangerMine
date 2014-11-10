using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FormationProfile {
	public Formation formation;

	public string name;

	public int minimumDistance;
	public int maximumDistance;

	public float probabilityWeight;

	public string[] prefabPoolIds;
	
	
	bool expand;

	public bool Expand {
		get {
			return expand;
		}
		set {
			expand = value;
		}
	}

	bool expandPrefabs;
	Vector2 scrollPrefabs = default(Vector2);

	public FormationProfile(Formation parent) {
		formation = parent;
		name = parent.name + "_" + parent.profiles.Count;
		minimumDistance = parent.minimumDistance;
		maximumDistance = parent.maximumDistance;
		probabilityWeight = 1;
		prefabPoolIds = new string[parent.spawnPoints.Count];
	}

	public void UpdatePrefabArraySize() {
		string[] newPrefabs = new string[formation.spawnPoints.Count];

		if (prefabPoolIds != null) {
			for (int i = 0; i < newPrefabs.Length && i < prefabPoolIds.Length; i++)
				newPrefabs[i] = prefabPoolIds[i];
		}

		prefabPoolIds = newPrefabs;
	}

	public void DisplayProfile() {
		name = EditorGUILayout.TextField("Name", name);
		EditorGUILayout.ObjectField("Formation", formation, typeof(Formation), false);

		EditorGUILayout.Space();

		minimumDistance = Mathf.RoundToInt( Mathf.Clamp(EditorGUILayout.IntField("Min Distance", minimumDistance), 
		                                                formation.minimumDistance, formation.maximumDistance));

		maximumDistance = Mathf.RoundToInt( Mathf.Clamp(EditorGUILayout.IntField("Max Distance", maximumDistance),
	                                                formation.minimumDistance, formation.maximumDistance));

		probabilityWeight = EditorGUILayout.FloatField("Probability weight", probabilityWeight);
		
		EditorGUILayout.Space();

		expandPrefabs = EditorGUILayout.Foldout(expandPrefabs, "Prefabs");
		if (expandPrefabs) {
			if (prefabPoolIds == null) UpdatePrefabArraySize();

			scrollPrefabs = EditorGUILayout.BeginScrollView(scrollPrefabs, GUILayout.MinHeight(50));

				for (int i = 0; i < prefabPoolIds.Length; i++)
					prefabPoolIds[i] = EditorGUILayout.TextField("Entity " + i.ToString(), prefabPoolIds[i]
				                                             );

			EditorGUILayout.EndScrollView();
		}
		
	}
}
