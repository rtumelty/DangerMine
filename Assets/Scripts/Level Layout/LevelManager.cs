using UnityEngine;	
using System.Collections;

public class LevelManager : MonoBehaviour {
	[SerializeField] Transform cameraTransform;
	[SerializeField] string blockPrefabPoolId;
	[SerializeField] Transform spawnPoint;

	PrefabPool levelBlockPool;
	GameObject lastBlock;
	Camera mainCamera;

	// Use this for initialization
	void Awake () {
		if (cameraTransform == null)
			cameraTransform = Camera.main.transform;

		mainCamera = Camera.main;

		levelBlockPool = PrefabPool.GetPool (blockPrefabPoolId);

		PlaceNextBlock ();
		PlaceNextBlock ();
	
		
		GlobalManagement.LAST_DISTANCE_COVERED = 0;
		GlobalManagement.SCORE = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (spawnPoint.position.x - (cameraTransform.position.x + (mainCamera.orthographicSize * mainCamera.aspect)) < 5)
			PlaceNextBlock ();	

		// End conditions
		if (Ally.ActiveAllies == 0)
			StartCoroutine (GameOver ());
	}

	void PlaceNextBlock() {
		lastBlock = levelBlockPool.Spawn (spawnPoint.position, spawnPoint.rotation);
		lastBlock.GetComponent<PooledPrefab> ().distanceObject = cameraTransform;
		spawnPoint = lastBlock.transform.FindChild ("NextBlock");
	}

	IEnumerator GameOver() {
		GlobalManagement.LAST_DISTANCE_COVERED = SpawnObject.cameraDistanceCovered;
		GlobalManagement.SCORE = SpawnObject.cameraDistanceCovered * 10;
		
		Application.LoadLevel ("ResultsScreen");

		yield return null;
	}
}
