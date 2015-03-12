#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxLayer : MonoBehaviour {
	public enum LayerType {
		Tiled,
		Spawned
	}

	[SerializeField] LayerType layerType;
	[SerializeField] GameObject parallaxReference;
	[SerializeField] float parallaxFactor;
	[SerializeField] int sortingLayerOrder;

#region Tiled parameters
	[SerializeField] GameObject tiledTexture;
#endregion

#region Spawning parameters
	[SerializeField] GameObject[] prefabs;
	[SerializeField] float minInterval = 3;
	[SerializeField ] float maxInterval = 6;
	
	float nextSpawn;
	float lastX;
#endregion

	void OnEnable() {

		switch (layerType) {
		case LayerType.Tiled:
			tiledTexture.SetActive(true);
			foreach (Renderer spriteRenderer in tiledTexture.GetComponentsInChildren<Renderer>())
				spriteRenderer.sortingOrder = sortingLayerOrder;
			ParallaxObject parallaxObject = tiledTexture.GetComponent<ParallaxObject>();
			parallaxObject.parallaxScale = parallaxFactor;
			parallaxObject.parallaxReference = parallaxReference;
			parallaxObject.tiled = true;

			parallaxObject.Init();
			break;
		case LayerType.Spawned:
			SpawnObject(transform.position + new Vector3(Random.Range(-maxInterval, maxInterval), 0, 0));
			break;
		}

		lastX = parallaxReference.transform.position.x;
	}

	void Update() {
		if (layerType == LayerType.Tiled)
			return;

		float deltaX = parallaxReference.transform.position.x - lastX;

		nextSpawn -= deltaX;
		if (nextSpawn <= 0) 
			SpawnObject(transform.position);

		lastX = parallaxReference.transform.position.x;
	}

	void SpawnObject(Vector3 position = default(Vector3)) {
		GameObject newObject;
		
		switch (layerType) {
		case LayerType.Tiled:
			Debug.Log("Invalid for tiled layers");
			break;
		case LayerType.Spawned:
			newObject = PrefabPool.GetPool(prefabs[Random.Range(0, prefabs.Length)]).Spawn(position);
			
			newObject.renderer.sortingOrder = sortingLayerOrder;
			ParallaxObject parallaxObject = newObject.GetComponent<ParallaxObject>();
			parallaxObject.parallaxScale = parallaxFactor;
			parallaxObject.parallaxReference = parallaxReference;
			parallaxObject.tiled = false;

			parallaxObject.Init();
			break;
		}

		nextSpawn = Random.Range(minInterval, maxInterval);
	}

#if UNITY_EDITOR
	bool expandPrefabs = false;
	public void ShowGUI(Editor editor) {
		layerType = (LayerType) EditorGUILayout.EnumPopup("Layer type:", layerType);
		parallaxReference = EditorGUILayout.ObjectField("Parallax reference:", parallaxReference, typeof(GameObject), true) as GameObject;
		parallaxFactor = EditorGUILayout.FloatField("Parallax factor:", parallaxFactor);
		sortingLayerOrder = EditorGUILayout.IntField("Sorting order:", sortingLayerOrder);

		if (layerType == LayerType.Tiled) {
			tiledTexture = EditorGUILayout.ObjectField("Tiled texture:", tiledTexture, typeof(GameObject), true) as GameObject;
		}
		else {
			if (prefabs == null) prefabs = new GameObject[0];

			expandPrefabs = EditorGUILayout.Foldout(expandPrefabs, "Prefabs:");
			if (expandPrefabs) {
				int newSize = EditorGUILayout.IntField("Size", prefabs.Length);

				if (newSize != prefabs.Length) {
					GameObject[] newPrefabs = new GameObject[newSize];
					for (int i = 0; i < newPrefabs.Length && i < prefabs.Length; i++)
						newPrefabs[i] = prefabs[i];

					prefabs = newPrefabs;
				}

				for (int i = 0; i < prefabs.Length; i++) {
					prefabs[i] = EditorGUILayout.ObjectField("Element " + i + ":", prefabs[i], typeof(GameObject), false) as GameObject;
				}
			}

			minInterval = EditorGUILayout.FloatField("Min interval:", minInterval);
			maxInterval = EditorGUILayout.FloatField("Max interval:", maxInterval);
		}

		EditorUtility.SetDirty(this);
	}
#endif
}
