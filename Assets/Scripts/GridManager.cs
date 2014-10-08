using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

	static GridManager instance;
	public static GridManager Instance {
		get {
			if (instance == null) {
				GameObject go = new GameObject("_GridManager");
				instance = new GridManager();
			}
			return instance;
		}
	}

	Dictionary<GameEntity, Vector2> occupiedPositions;

	void Awake() {
		if (instance != null) Destroy(this);

		DontDestroyOnLoad(this);
		occupiedPositions = new Dictionary<GameEntity, Vector2>();
	}

	void OnLevelWasLoaded(int level) {
		occupiedPositions.Clear();
	}

	void Update() {
		List<GameEntity> keys = new List<GameEntity>(occupiedPositions.Keys);
		foreach (GameEntity key in keys) {
			Vector2 position = new Vector2(Mathf.Round(key.transform.position.x), 
			                               Mathf.Round(key.transform.position.y));
			occupiedPositions[key] = position;
		}
	}

	public bool IsOccupied(Vector2 position) {
		foreach (KeyValuePair<GameEntity, Vector2> pair in occupiedPositions) {
			if (Vector2.Equals(pair.Value, position)) return true;
		}
		return false;
	}

	public void RegisterEntity(GameEntity entity) {
		Vector2 position = new Vector2(Mathf.Round(entity.transform.position.x), 
		                               Mathf.Round(entity.transform.position.y));
		occupiedPositions.Add(entity, position);
	}

	public void UnregisterEntity(GameEntity entity) {
		occupiedPositions.Remove(entity);
	}
}
