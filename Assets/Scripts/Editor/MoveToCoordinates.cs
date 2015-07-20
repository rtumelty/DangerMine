using UnityEditor;
using UnityEngine;
using System.Collections;

public class MoveToCoordinates : EditorWindow {
	int gridX = 0;
	int gridY = 0;

	GameObject selection;

	void Reposition(Transform transform) {
		transform.position = GridManager.ScreenCoordsToWorldPosition(new GridCoordinate(gridX, gridY));
	}

	[MenuItem("Window/Grid position")]
	static void Init() {
		MoveToCoordinates window = (MoveToCoordinates)EditorWindow.GetWindow<MoveToCoordinates>();
	}

	void OnGUI() {
		gridX = EditorGUILayout.IntField("X coord:", gridX);
		gridY = EditorGUILayout.IntField("Y coord:", gridY);

		selection = Selection.activeGameObject;

		if (GUILayout.Button("Reposition") && selection != null) 
			Reposition(selection.transform	);
	}
}
