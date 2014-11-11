using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProbabilityTable : MonoBehaviour {
	[System.Serializable]
	public struct WeightedPoolId {
		public string poolId;
		public float weight;
	}

	[SerializeField] List<WeightedPoolId> poolIds;

	void Awake() {
		if (poolIds == null) poolIds = new List<WeightedPoolId>();
	}

	void OnEnable () {
		float spawnValue = Random.Range (0, 1f);

		string poolId = "";

		float combinedWeights = 0;
		foreach (WeightedPoolId id in poolIds)
			combinedWeights += id.weight;

		for (int i = 0; i < poolIds.Count; i++) {
			spawnValue -= poolIds[i].weight / combinedWeights;
			if (spawnValue < 0 || i+1 == poolIds.Count)  {
				poolId = poolIds[i].poolId;
			}
		}

		PrefabPool.GetPool (poolId).Spawn (transform.position);

		gameObject.SetActive (false);
	}
}
