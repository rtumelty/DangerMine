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

		GameObject prefab = null;

		float combinedWeights = 0;
		foreach (WeightedPrefab id in prefabs)
			combinedWeights += id.weight;

		float spawnValue = Random.Range (0, combinedWeights);

		for (int i = 0; i < prefabs.Count; i++) {
			spawnValue -= prefabs[i].weight;
			if (spawnValue < 0 || i+1 == prefabs.Count)  {
				prefab = prefabs[i].prefab;
				break;
			}
		}

		PrefabPool.GetPool (prefab).Spawn (transform.position);

		gameObject.SetActive (false);
	}
}
