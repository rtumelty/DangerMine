using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(GameEntity), true)]
public class GameEntityInspector : Editor {
	public override void OnInspectorGUI() {
		GameEntity entity = target as GameEntity;

		entity.DrawInspectorGUI(this);
	}
}
