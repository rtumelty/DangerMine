using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabPool : MonoBehaviour {
	List<GameObject> pool;
	int currentPoolIndex = 0;
	[SerializeField] GameObject prefabToPool;
	[SerializeField] int preinstantiatedCount = 20;
	[SerializeField] int maxObjectCount = 100;

	private static Dictionary<GameObject, PrefabPool> pools;

	void Awake () 
	{
		if (pools == null) 
		{
			pools = new Dictionary<GameObject, PrefabPool>();
		}

		PrefabPool temp;
		if (pools.TryGetValue (prefabToPool, out temp)) 
						pools.Remove (prefabToPool);

		pools.Add (prefabToPool, this);

		pool = new List<GameObject>();

		for (int i = 0; i < preinstantiatedCount; i++) {
			GameObject newInstance = Instantiate(prefabToPool) as GameObject;
			pool.Add(newInstance);
			newInstance.transform.parent = transform;

			newInstance.SetActive(false);
		}
	}

	public static PrefabPool GetPool(GameObject prefab) {
		PrefabPool fetchedPool;
		pools.TryGetValue (prefab, out fetchedPool);
		return fetchedPool;
	}

	public GameObject Spawn(Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion)) {
		GameObject prefab = GetNextInactiveObject();
		prefab.transform.position = position;
		prefab.transform.rotation = rotation;
		prefab.SetActive(true);
		return prefab;
	}

	private GameObject GetNextInactiveObject() {
		bool active = true;
		GameObject go = null;

		for (int i = 0; i < pool.Count && active == true; i++) {
			if (currentPoolIndex >= pool.Count) currentPoolIndex = 0;

			go = pool[currentPoolIndex++];
			if (currentPoolIndex >= pool.Count) currentPoolIndex = 0;

			active = go.activeSelf;
		}

		if (active) {
			if (pool.Count < maxObjectCount) {
				go = Instantiate(prefabToPool) as GameObject;
				go.transform.parent = transform;
				pool.Add(go);
			}
			else {
				go = pool[currentPoolIndex++];
			}
		}

		return go;
	}
}
