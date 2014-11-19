using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FormationManager : MonoBehaviour 
{
	
	[SerializeField] SpawnGroup[] spawnGroups;
	[SerializeField] bool printDebugInfo = false;

	private int cameraStartingXPosition = 0;
	private Transform spawnOrigin;
	private static int cameraDistanceCovered = 0;
	private static int nextSpawnDistance = 0;
	private bool placingSequence = false;

	public static int CameraDistanceCovered {
		get {
			return cameraDistanceCovered;
		}
	}
	
	void Awake()
	{
		cameraStartingXPosition = Mathf.RoundToInt(Camera.main.transform.position.x);
		cameraDistanceCovered = 0;
		nextSpawnDistance = 0;
		spawnOrigin = GameObject.FindWithTag ("SP0").transform;
	}
	
	
	void Update()
	{
		cameraDistanceCovered = ( int ) Camera.main.transform.position.x - cameraStartingXPosition;

		if (cameraDistanceCovered >= nextSpawnDistance && ! placingSequence)
			SpawnNextGroup();
	}

	List<SpawnGroup> GetActiveSpawnGroups() {
		List<SpawnGroup> activeSpawnGroups = new List<SpawnGroup>();
		for(int i = 0; i < spawnGroups.Length; i++)
		{ 
			if (spawnGroups[i].maximumDistance == 0) spawnGroups[i].maximumDistance = Mathf.Infinity; 
			if (spawnGroups[i].minimumDistance <= cameraDistanceCovered && spawnGroups[i].maximumDistance >= cameraDistanceCovered) {
				activeSpawnGroups.Add(spawnGroups[i]);
			}
		}

		if (activeSpawnGroups.Count == 0) {
			Debug.LogError("No active spawn groups at distance " + cameraDistanceCovered);
			return null;
		}

		return activeSpawnGroups;
	}

	SpawnGroup ChooseNextGroup() {
		List<SpawnGroup> activeGroups = GetActiveSpawnGroups();
		if (activeGroups == null) return null;
		
		float combinedWeights = 0;
		foreach (SpawnGroup group in activeGroups)
			combinedWeights += group.probabilityWeight;
		
		float spawnValue = Random.value;

		for (int i = 0; i < activeGroups.Count; i++) {
			spawnValue -= activeGroups[i].probabilityWeight / combinedWeights;
			if (spawnValue < 0 || i+1 == activeGroups.Count)  {
				return activeGroups[i];
			}
		}

		return null;
	}

	void SpawnNextGroup() {
		SpawnGroup nextGroup = ChooseNextGroup();

		if (nextGroup is Sequence) {
			placingSequence = true;
			StartCoroutine(PlaceSequence(nextGroup as Sequence));

		}
		else if (nextGroup is Formation) {
			PlaceFormation(nextGroup as Formation);
		}
	}

	void PlaceFormation(Formation formation) {
		FormationProfile profile = formation.ChooseProfile(nextSpawnDistance);

		if (printDebugInfo){
			Debug.Log("Placing formation " + formation + ", profile " + profile + " at distance " + cameraDistanceCovered);
		}
		nextSpawnDistance += formation.interval;
		int heightOffset = Random.Range(0, 6 - formation.height);
		Vector3 formationOffset = spawnOrigin.position + new Vector3(nextSpawnDistance, heightOffset * GridCoordinate.YScale, 0);

		for (int i = 0; i < formation.spawnPoints.Count; i++) {
			PrefabPool.GetPool(profile.prefabPoolIds[i]).Spawn(formationOffset + formation.spawnPoints[i].ToVector3());
		}
		nextSpawnDistance += formation.width;
	}

	IEnumerator PlaceSequence(Sequence sequence) {
		if (printDebugInfo)
			Debug.Log("Starting sequence " + sequence.name + " at distance " + cameraDistanceCovered);

		int i = 0;

		while (i < sequence.formations.Count) {
			if (cameraDistanceCovered >= nextSpawnDistance) {
				PlaceFormation(sequence.formations[i++]);
			}

			yield return new WaitForSeconds(Time.deltaTime);
		}

		placingSequence = false;
	}
}


	