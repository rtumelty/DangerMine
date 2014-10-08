using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

	static GridManager instance;
	public static GridManager Instance {
		get {
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
		foreach (KeyValuePair<GameEntity, GridCoordinate> pair in occupiedPositions) {
			if (pair.Value == position as GridCoordinate) return true;
		}
		return false;
	}

	public void RegisterEntity(GameEntity entity) {
		GridCoordinate position = new GridCoordinate(entity.transform.position);
		occupiedPositions.Add(entity, position);
	}

	public void UnregisterEntity(GameEntity entity) {
		occupiedPositions.Remove(entity);
	}
}
