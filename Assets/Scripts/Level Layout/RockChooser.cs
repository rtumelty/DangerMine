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
		float seed = Random.Range (0, 1);

		string poolId;

		if (seed < crackedRockProbability)
			poolId = crackedRockPoolId;
		else if (seed < crackedRockProbability + hardRockProbability)
			poolId = hardRockPoolId;
		else
			poolId = goldRockPoolId;
		
		Debug.Log (poolId);
		Debug.Log (PrefabPool.GetPool (poolId));

		GameObject newRock = PrefabPool.GetPool (poolId).Spawn (transform.position);
		foreach (Renderer rend in newRock.GetComponentsInChildren<Renderer>())
			rend.sortingLayerID = renderer.sortingLayerID;

		gameObject.SetActive (false);
	}
}
