using UnityEngine;
using System.Collections;

public class SpawnObject : MonoBehaviour 
{
	
	[System.Serializable] struct rule
	{
		public string spawnableObjectName;
		public PrefabPool prefabPool;
		public int minSpawnDistance;
		public int maxSpawnDistance;
		public int maxObjectsOnScreen;
		public int myNextSpawnPoint;
		public int myLastSpawnPosition;
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
		cameraDistanceCovered = ( int ) Camera.main.transform.position.x - cameraStartingXPosition;

		for(int i = 0; i < ruleSets.Length; i++)
		{ 

			if( ruleSets[i].myNextSpawnPoint <= spawnPoint0.transform.position.x )
			{
				int myRandomSpawnLane = Random.Range(-2, 3);
				GameObject spawnedObject = ruleSets[i].prefabPool.Spawn( new Vector3(spawnPoint0.transform.position.x, myRandomSpawnLane, spawnPoint0.transform.position.z), spawnPoint0.transform.rotation ); 
				ruleSets[i].myNextSpawnPoint = (int)spawnPoint0.transform.position.x + Random.Range( ruleSets[i].minSpawnDistance, ruleSets[i].maxSpawnDistance + 1 );
				ruleSets[i].myLastSpawnPosition = (int)spawnedObject.transform.position.x;

				foreach (Renderer rend in (spawnedObject.GetComponentsInChildren<Renderer>()))
	         	{
					rend.sortingLayerName = "Lane_" + myRandomSpawnLane.ToString(); 
				}
			}
		}
	}
}


	