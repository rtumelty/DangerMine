using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FormationProfile {
	public Formation formation;

	public string name;

	public float minimumDistance;
	public float maximumDistance;

	public float probabilityWeight;

	public GameObject[] prefabs;
	
	
	public bool expand;
	public bool expandPrefabs;
	public Vector2 scrollPrefabs = default(Vector2);

	public FormationProfile(Formation parent) {
		formation = parent;
		name = parent.name + "_" + parent.profiles.Count;
		minimumDistance = parent.minimumDistance;
		maximumDistance = parent.maximumDistance;
		probabilityWeight = 1;
		prefabs = new GameObject[parent.spawnPoints.Count];
	}

	public void UpdatePrefabArraySize() {
		GameObject[] newPrefabs = new GameObject[formation.spawnPoints.Count];

		if (prefabs != null) {
			for (int i = 0; i < newPrefabs.Length && i < prefabs.Length; i++)
				newPrefabs[i] = prefabs[i];
		}

		prefabs = newPrefabs;
	}

}
