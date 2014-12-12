#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

public class Rock : DestructibleEntity {
	[SerializeField] protected bool dropsPickup;
	[SerializeField] protected GameObject droppedObject;

	PrefabPool droppedObjectPool;

	protected override void Awake() {
		droppedObjectPool = PrefabPool.GetPool(droppedObject);

		base.Awake();
	}

	protected override void OnDisable() {
		if (destroyedBy is Ally && dropsPickup) {
			if (droppedObjectPool != null)
				droppedObjectPool.Spawn(transform.position);
		}

		base.OnDisable();
	}

#if UNITY_EDITOR
	public override void DrawInspectorGUI(Editor editor) {
		base.DrawInspectorGUI(editor);

		dropsPickup = EditorGUILayout.Toggle("Drops item?", dropsPickup);
		if (dropsPickup) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(10);

			droppedObject = EditorGUILayout.ObjectField("Item prefab", droppedObject, typeof(GameObject), false) as GameObject;
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
		}
			
	}
#endif
}
