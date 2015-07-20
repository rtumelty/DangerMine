#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FormationProfile : ScriptableObject {

	public string name;
	public GameObject[] prefabs;
	


	public void Initialize() {
		name = "";
		prefabs = new GameObject[0];
	}

	public void Initialize(string profileName, GameObject[] profilePrefabs) {
		name = profileName;
		prefabs = profilePrefabs;
	}

	#if UNITY_EDITOR
	public bool expandPrefabs;
	public Vector2 scrollPrefabs = default(Vector2);

	public void ShowGUI() {
		name = EditorGUILayout.TextField("Name:", name);
		
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

		EditorUtility.SetDirty(this);
	}
#endif
}
