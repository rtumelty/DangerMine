#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

/// <summary>
/// Game entity. Base class of all objects in game. Registers positions on world and screen grids.
/// </summary>
public class GameEntity : MonoBehaviour {
	public enum EntityState {
		Inactive,
		Active,
		Dying
	}
	
	protected EntityState entityState = EntityState.Inactive;
	
	public EntityState State {
		get {
			return entityState;
		}
		set {
			LogMessage("Changing state: " + value.ToString());
			entityState = value;
		}
	}

	[SerializeField] protected DebugLevel debugLevel = DebugLevel.None;

	protected Renderer[] renderers;

	/// <summary>
	/// Gets the world coords.
	/// </summary>
	/// <value>The world coords.</value>
	public GridCoordinate WorldCoords {
		get {
			return new GridCoordinate(transform.position);
		}
	}

	/// <summary>
	/// Gets the screen coords.
	/// </summary>
	/// <value>The screen coords.</value>
	public GridCoordinate ScreenCoords {
		get {
			return GridManager.WorldToScreenGridCoords(transform.position);
		}
	}

	protected virtual void Awake() {
	}

	protected virtual void OnEnable() {
		LogMessage("Enabled");

		if (renderers == null)
			renderers = GetComponentsInChildren<Renderer>();

		UpdateSortingLayer();
		
		GridManager.Instance.RegisterEntity(GridManager.Grid.WorldGrid, this);
		GridManager.Instance.RegisterEntity(GridManager.Grid.ScreenGrid, this);
	}
	protected virtual void OnDisable() {
		LogMessage("Disabled");
		entityState = EntityState.Inactive;

		if (GridManager.Instance != null) {
			GridManager.Instance.UnregisterEntity(GridManager.Grid.WorldGrid, this);
			GridManager.Instance.UnregisterEntity(GridManager.Grid.ScreenGrid, this);
		}
	}

	/// <summary>
	/// Updates the sorting layer of all renderers on GameObject and its children. 
	/// </summary>
	protected void UpdateSortingLayer () {
		string newLayer = "Lane_" + WorldCoords.y;

		LogMessage("Updating sorting layer - " + newLayer);

		foreach ( Renderer rend in renderers) {
			rend.sortingLayerName = newLayer;
		}
	}

#if UNITY_EDITOR
	protected bool showDefaultInspector = false;

	/// <summary>
	/// Draws the inspector GUI. Overridden in subclasses.
	/// </summary>
	/// <param name="editor">Editor.</param>
	public virtual void DrawInspectorGUI(Editor editor) {
		showDefaultInspector = EditorGUILayout.Toggle("Use default inspector?", showDefaultInspector);

		if (showDefaultInspector) {
			editor.DrawDefaultInspector();
			return;
		}

		EditorGUILayout.LabelField("Debugging / Logging", EditorStyles.boldLabel);
		debugLevel = (DebugLevel) EditorGUILayout.EnumPopup("Debug level:", debugLevel);
		entityState = (EntityState) EditorGUILayout.EnumPopup("Entity state:", entityState);

		EditorGUILayout.Vector2Field("Screen coords:", new Vector2(ScreenCoords.x, ScreenCoords.y));
		EditorGUILayout.Vector2Field("World coords:", new Vector2(WorldCoords.x, WorldCoords.y));
	
		EditorGUILayout.Space();

		EditorUtility.SetDirty(this);
	}
#endif

	protected void LogMessage(string message, DebugLevel level = DebugLevel.Info) {
		if ((level & debugLevel) > 0) {
			switch (level) {
			case DebugLevel.Info:
				Debug.Log (/*"Time: " + Time.time + ",*/ "GameEntity: " + name + ": \n" + message);
				break;
			case DebugLevel.Warning:
				Debug.LogWarning (/*"Time: " + Time.time + ",*/ "GameEntity: " + name + ": \n" + message);
				break;
			case DebugLevel.Error:
				Debug.LogError (/*"Time: " + Time.time + ",*/ "GameEntity: " + name + ": \n" + message);
				break;
			default:
				break;
			}
   		}
	}
}
