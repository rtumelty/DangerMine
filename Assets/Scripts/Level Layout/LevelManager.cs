using UnityEngine;	
using System.Collections;

public class LevelManager : MonoBehaviour {
	[SerializeField] Transform cameraTransform;
	[SerializeField] string blockPrefabPoolId;
	[SerializeField] Transform spawnPoint;

	PrefabPool levelBlockPool;
	GameObject lastBlock;
	Camera mainCamera;
	bool gameOver = false;

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
		if (Ally.ActiveAllies == 0 && !gameOver) {
			gameOver = true;
			StartCoroutine (GameOver ());
		}
	}

	void PlaceNextBlock() {
		lastBlock = levelBlockPool.Spawn (spawnPoint.position, spawnPoint.rotation);
		lastBlock.GetComponent<PooledPrefab> ().distanceObject = cameraTransform;
		spawnPoint = lastBlock.transform.FindChild ("NextBlock");
	}

	IEnumerator GameOver() {
		Debug.Log("Last miner died!");
		GlobalManagement.LAST_DISTANCE_COVERED = SpawnObject.cameraDistanceCovered;
		GlobalManagement.SCORE = SpawnObject.cameraDistanceCovered * 10;
		UIMessageReceiver.Instance.SendTrigger("PlayerDied");

		float slowDelay = .05f;
		float slowedTimeScale = .25f;

		for (float i = 0; i < slowDelay; i += Time.unscaledDeltaTime) {
			Time.timeScale = Mathf.Lerp(1, slowedTimeScale, i / slowDelay);
			yield return new WaitForSeconds(Time.unscaledDeltaTime);
		}

		yield return new WaitForSeconds(1f);
		
		for (float i = 0; i < slowDelay; i += Time.unscaledDeltaTime) {
			Time.timeScale = Mathf.Lerp(slowedTimeScale, 1, i / slowDelay);
			yield return new WaitForSeconds(Time.unscaledDeltaTime);
		}
		Time.timeScale = 1;
		
		UIMessageReceiver.Instance.SendTrigger("GameOver");
		yield return null;
	}
}
