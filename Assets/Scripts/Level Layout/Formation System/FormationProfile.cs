using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FormationProfile : ScriptableObject {
	public Formation formation;

	public string name;

	public int minimumDistance;
	public int maximumDistance;

	public float probabilityWeight;

	public GameObject[] prefabs;
	
	
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

	public void UpdatePrefabArraySize() {
		GameObject[] newPrefabs = new GameObject[formation.spawnPoints.Count];

		for (int i = 0; i < newPrefabs.Length && i < prefabs.Length; i++)
			newPrefabs[i] = prefabs[i];

		prefabs = newPrefabs;
	}

	public void DisplayProfile() {
		name = EditorGUILayout.TextField("Name", name);
		EditorGUILayout.ObjectField("Formation", formation, typeof(Formation), false);

		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		minimumDistance = Mathf.RoundToInt( Mathf.Clamp(EditorGUILayout.IntField("Min Distance", minimumDistance), 
		                                                formation.minimumDistance, formation.maximumDistance));

		maximumDistance = Mathf.RoundToInt( Mathf.Clamp(EditorGUILayout.IntField("Max Distance", maximumDistance),
		                                                formation.minimumDistance, formation.maximumDistance));

		EditorGUILayout.EndHorizontal();
		
		probabilityWeight = EditorGUILayout.FloatField("Probability weight", probabilityWeight);
		
		EditorGUILayout.Space();

		expandPrefabs = EditorGUILayout.Foldout(expandPrefabs, "Prefabs");
		if (expandPrefabs) {
			scrollPrefabs = EditorGUILayout.BeginScrollView(scrollPrefabs, GUILayout.MinHeight(50));

			for (int i = 0; i < prefabs.Length; i++)
				prefabs[i] = EditorGUILayout.ObjectField("Entity " + i.ToString(), prefabs[i], typeof(GameObject), false) as GameObject;

			EditorGUILayout.EndScrollView();
		}
		
	}
}
