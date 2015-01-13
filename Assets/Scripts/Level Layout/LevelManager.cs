using UnityEngine;	
using System.Collections;

public class LevelManager : MonoBehaviour {
	static LevelManager instance;

	public static LevelManager Instance {
		get {
			return instance;
		}
	}

	[SerializeField] Transform cameraTransform;
	[SerializeField] GameObject blockPrefab;
	[SerializeField] Transform spawnPoint;

	PrefabPool levelBlockPool;
	GameObject lastBlock;
	Camera mainCamera;
	bool gameStarted = false;
	bool gameOver = false;

	public bool GameStarted {
		get {
			return gameStarted;
		}
		set {
			gameStarted = value;
		}
	}

	// Use this for initialization
	void Awake () {
		if (instance == null) instance = this;
		else Destroy(this);

		if (cameraTransform == null)
			cameraTransform = Camera.main.transform;

		mainCamera = Camera.main;

		levelBlockPool = PrefabPool.GetPool (blockPrefab);

		PlaceNextBlock ();
		PlaceNextBlock ();
	
		
		GlobalManagement.LAST_DISTANCE_COVERED = 0;
		GlobalManagement.SCORE = 0;

		gameStarted = false;
		gameOver = false;

		Ally.ActiveAllies = 0;
	}

	void OnDisable() {
		StopAllCoroutines();
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

	public void CheckEndCondition() {
		if (!gameStarted) { 
			if (Ally.ActiveAllies > 0) 
				gameStarted = true;

			return;
		}	

		// End conditions
		if (Ally.ActiveAllies == 0 && !gameOver) {
			gameOver = true;
			StartCoroutine (GameOver ());
		}
	}

	IEnumerator GameOver() {
		Debug.Log("Last miner died!");
		GlobalManagement.LAST_DISTANCE_COVERED = FormationManager.CameraDistanceCovered;
		GlobalManagement.SCORE = FormationManager.CameraDistanceCovered * 10;
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
