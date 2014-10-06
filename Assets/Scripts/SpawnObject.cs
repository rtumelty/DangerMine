using UnityEngine;
using System.Collections;

public class SpawnObject : MonoBehaviour 
{

	[System.Serializable] struct rule
	{

		public PrefabPool prefabPool;
		public int minSpawnDistance;
		public int maxSpawnDistance;
		public int maxObjectsOnScreen;
		public int distanceSinceLastSpawn;
	}

	[SerializeField] rule[] ruleSets;
	[SerializeField] int cameraDistanceCovered = 0;
	[SerializeField] int cameraStartingXPosition = 0;
	private GameObject spawnPoint0;
	private int currentObjectsOnScreen = 0;

	void Awake()
	{
		cameraStartingXPosition = ( int )Camera.main.transform.position.x;
		spawnPoint0 = GameObject.FindWithTag ("SP0");
	}


	void Update()
	{
		cameraDistanceCovered += ( int ) Camera.main.transform.position.x - cameraStartingXPosition;


		foreach(rule myRule in ruleSets)
		{
			int myRandomSpawnDistance = Random.Range( myRule.minSpawnDistance, myRule.maxSpawnDistance + 1 );
			if( myRule.distanceSinceLastSpawn >= myRandomSpawnDistance )
			{
				myRule.prefabPool.Spawn( spawnPoint0.transform.position, spawnPoint0.transform.rotation );
			}
		}
	}
}
