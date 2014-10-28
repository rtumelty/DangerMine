using UnityEngine;
using System.Collections;

public class RockChooser : MonoBehaviour {
	[SerializeField] string crackedRockPoolId;
	[SerializeField] float crackedRockProbability = .55f;
	
	[SerializeField] string hardRockPoolId;
	[SerializeField] float hardRockProbability = .3f;
	
	[SerializeField] string goldRockPoolId;
	[SerializeField] float goldRockProbability = .15f;

	void OnEnable () {
		float seed = Random.Range (0, 1f);

		string poolId = "";

		if (seed < crackedRockProbability)
			poolId = crackedRockPoolId;
		else if (seed < crackedRockProbability + hardRockProbability)
			poolId = hardRockPoolId;
		else if (seed < crackedRockProbability + hardRockProbability + goldRockProbability)
			poolId = goldRockPoolId;

		PrefabPool.GetPool (poolId).Spawn (transform.position);

		gameObject.SetActive (false);
	}
}
