using UnityEngine;	
using System.Collections;

public class LevelGenerator : MonoBehaviour {
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
	
	}
	
	// Update is called once per frame
	void Update () {
		if (spawnPoint.position.x - (cameraTransform.position.x + (mainCamera.orthographicSize * mainCamera.aspect)) < 5)
			PlaceNextBlock ();	
	}

	void PlaceNextBlock() {
		lastBlock = levelBlockPool.Spawn (spawnPoint.position, spawnPoint.rotation);
		lastBlock.GetComponent<PooledPrefab> ().distanceObject = cameraTransform;
		spawnPoint = lastBlock.transform.FindChild ("NextBlock");
	}
}
