using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {
	static bool destroyed = false;
	static GridManager instance;
	public static GridManager Instance {
		get {
			if (destroyed) return null;
			if (instance == null) {
				GameObject go = new GameObject("_GridManager");
				instance = go.AddComponent<GridManager>();
			}
			return instance;
		}
	}

	Dictionary<GameEntity, GridCoordinate> occupiedPositions;

	void Awake() {
		if (instance != null) Destroy(this);

		instance = this;
		DontDestroyOnLoad(this);
		occupiedPositions = new Dictionary<GameEntity, GridCoordinate>();
	}
	
	void OnEnable() { destroyed = false; }
	void OnDisable() { destroyed = true; }

	void OnLevelWasLoaded(int level) {
		occupiedPositions.Clear();
	}

	void Update() {
		List<GameEntity> keys = new List<GameEntity>(occupiedPositions.Keys);
		foreach (GameEntity key in keys) {
			GridCoordinate position = new GridCoordinate(key.transform.position);
			occupiedPositions[key] = position;
		}
	}
	
	public bool IsOccupied(Vector2 position) {
		return IsOccupied (position as GridCoordinate);
	}

	public bool IsOccupied(GridCoordinate position) {
		foreach (KeyValuePair<GameEntity, GridCoordinate> pair in occupiedPositions) {
			if (pair.Value == position) return true;
		}
		return false;
	}
	
	public GameEntity EntityAt(Vector2 position) {
		return EntityAt (position as GridCoordinate);
	}

	public GameEntity EntityAt(GridCoordinate position) {
		foreach (KeyValuePair<GameEntity, GridCoordinate> pair in occupiedPositions) {
			if (pair.Value == position) return pair.Key;
		}
		return null;
	}

	public void RegisterEntity(GameEntity entity) {
		GridCoordinate position = new GridCoordinate(entity.transform.position);
		occupiedPositions.Add(entity, position);
	}

	public void UnregisterEntity(GameEntity entity) {
		occupiedPositions.Remove(entity);
	}
}
