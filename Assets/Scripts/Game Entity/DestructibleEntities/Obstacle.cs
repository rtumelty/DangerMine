#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

public class Obstacle : DestructibleEntity {
	[SerializeField] protected bool dropsPickup;
	[SerializeField] protected GameObject droppedObject;
	[SerializeField] protected int dropCount = 1;
	[SerializeField] protected int highDropCount = 1;
	[SerializeField] protected float highDropChance = .1f;

	PrefabPool droppedObjectPool;

	protected override void Awake() {
		droppedObjectPool = PrefabPool.GetPool(droppedObject);

		base.Awake();
	}

	protected override void OnEnable() {
		base.OnEnable();
		
		State = EntityState.Active;
	}

	protected override void OnDisable() {
		Debug.Log(destroyedBy);

		if (destroyedBy is Ally && dropsPickup) {
			LogMessage("Dropping object " + droppedObject + " from pool " + droppedObjectPool);

			if (droppedObjectPool != null) {
				int drop = dropCount;
				if (Random.value < highDropChance)
					drop = highDropCount;

				for (int i = 0; i < drop; i++)
					droppedObjectPool.Spawn(transform.position);
			}
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

			EditorGUILayout.BeginVertical();
			droppedObject = EditorGUILayout.ObjectField("Item prefab", droppedObject, typeof(GameObject), false) as GameObject;
			dropCount = EditorGUILayout.IntField("Number spawned", dropCount);
			highDropChance = EditorGUILayout.FloatField("High drop chance", highDropChance);
			highDropCount = EditorGUILayout.IntField("Number spawned (high)", highDropCount);

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
		}
			
	}
#endif
}
