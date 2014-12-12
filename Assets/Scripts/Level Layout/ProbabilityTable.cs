using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProbabilityTable : MonoBehaviour {
	[System.Serializable]
	public struct WeightedPrefab {
		public GameObject prefab;
		public float weight;
	}

	[SerializeField] List<WeightedPrefab> prefabs;

	void Awake() {
		if (prefabs == null) prefabs = new List<WeightedPrefab>();
	}

	void OnEnable () {
		float spawnValue = Random.Range (0, 1f);

		GameObject prefab = null;

		float combinedWeights = 0;
		foreach (WeightedPrefab id in prefabs)
			combinedWeights += id.weight;

		for (int i = 0; i < prefabs.Count; i++) {
			spawnValue -= prefabs[i].weight / combinedWeights;
			if (spawnValue < 0 || i+1 == prefabs.Count)  {
				prefab = prefabs[i].prefab;
			}
		}

		PrefabPool.GetPool (prefab).Spawn (transform.position);

		gameObject.SetActive (false);
	}
}
