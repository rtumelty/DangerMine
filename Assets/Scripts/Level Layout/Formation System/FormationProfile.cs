using UnityEngine;
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
	
	
	public bool expand;
	public bool expandPrefabs;
	public Vector2 scrollPrefabs = default(Vector2);

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

}
