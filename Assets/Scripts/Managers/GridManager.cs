using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {
	[SerializeField] Transform screenGridOrigin;

	public Vector3 ScreenOrigin {
		set {
			screenGridOrigin.transform.position = value;
		}
		get {
			return screenGridOrigin.position;
		}
	}

	public const int maxY = 4;
	public const int minY = 0;
	
	public static float minWorldY {
		get {
			return instance.screenGridOrigin.position.y + ((float)minY * GridCoordinate.XScale);
		}
	}
	
	public static float maxWorldY {
		get {
			return instance.screenGridOrigin.position.y + ((float)maxY * GridCoordinate.XScale);
		}
	}

	public const int minScreenX = -5;
	public const int maxScreenX = 5;
	
	public static float minWorldX {
		get {
			return instance.screenGridOrigin.position.x + ((float)minScreenX * GridCoordinate.XScale);
		}
	}
	
	public static float maxWorldX {
		get {
			return instance.screenGridOrigin.position.x + ((float)maxScreenX * GridCoordinate.XScale);
		}
	}

	public static int PlayableAreaWidth {
		get {
			return 3 + Mathf.Abs(minScreenX) + maxScreenX;
		}
	}

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

	public enum Grid {
		WorldGrid,
		ScreenGrid
	}
	
	Dictionary<GameEntity, GridCoordinate> occupiedWorldPositions;
	Dictionary<GameEntity, GridCoordinate> occupiedScreenPositions;

	void Awake() {
		if (instance != null) Destroy(this);

		instance = this;
		
		occupiedWorldPositions = new Dictionary<GameEntity, GridCoordinate>();
		occupiedScreenPositions = new Dictionary<GameEntity, GridCoordinate>();
	}
	
	void OnEnable() { destroyed = false; }
	void OnDisable() { destroyed = true; }

	void OnLevelWasLoaded(int level) {
		occupiedScreenPositions.Clear();
		occupiedWorldPositions.Clear();
		foreach (GameEntity entity in FindObjectsOfType<GameEntity>()) RegisterEntity(Grid.WorldGrid, entity);
	}

	void Update() {
		List<GameEntity> keys = new List<GameEntity>(occupiedWorldPositions.Keys);
		foreach (GameEntity key in keys) {
			GridCoordinate position = new GridCoordinate(key.transform.position);
			occupiedWorldPositions[key] = position;
		}
		
		keys = new List<GameEntity>(occupiedScreenPositions.Keys);
		foreach (GameEntity key in keys) {
			GridCoordinate position = WorldToScreenGridCoords(key.transform.position);
			occupiedScreenPositions[key] = position;
		}
	}
	
	public bool IsOccupied(Grid grid, Vector2 position) {
		return IsOccupied (grid, new GridCoordinate(position ));
	}

	public bool IsOccupied(Grid grid, GridCoordinate position) {
		Dictionary<GameEntity, GridCoordinate> gridToCheck = null;
		
		switch (grid) {
		case Grid.WorldGrid:
			gridToCheck = occupiedWorldPositions;
			break;
		case Grid.ScreenGrid:
			gridToCheck = occupiedScreenPositions;
			break;
		}
		
		if (position.y < minY || position.y > maxY)
			return true;

		foreach (KeyValuePair<GameEntity, GridCoordinate> pair in gridToCheck) {
			if (pair.Value == position) return true;
		}
		return false;
	}
	
	public List<GameEntity> EntitiesAt(Grid grid, Vector2 position) {
        return EntitiesAt(grid, new GridCoordinate(position));
	}
	
	public List<GameEntity> EntitiesAt(Grid grid, GridCoordinate position) {
		Dictionary<GameEntity, GridCoordinate> gridToCheck = null;
		List<GameEntity> entities = new List<GameEntity>();
		
		switch (grid) {
		case Grid.WorldGrid:
			gridToCheck = occupiedWorldPositions;
			break;
		case Grid.ScreenGrid:
			gridToCheck = occupiedScreenPositions;
			break;
		}
		
		if (position.y < minY || position.y > maxY)
			return null;
		
		foreach (KeyValuePair<GameEntity, GridCoordinate> pair in gridToCheck) {
			if (pair.Value == position) entities.Add(pair.Key);
		}
		return entities;
	}
	
	public GameEntity EntityAt(Grid grid, GridCoordinate position) {
		Dictionary<GameEntity, GridCoordinate> gridToCheck = null;
		List<GameEntity> entities = new List<GameEntity>();
		
		switch (grid) {
		case Grid.WorldGrid:
			gridToCheck = occupiedWorldPositions;
			break;
		case Grid.ScreenGrid:
			gridToCheck = occupiedScreenPositions;
			break;
		}
		
		if (position.y < minY || position.y > maxY)
			return null;
		
		foreach (KeyValuePair<GameEntity, GridCoordinate> pair in gridToCheck) {
			if (pair.Value == position) return (pair.Key);
		}

		return null;
	}

	public void RegisterEntity(Grid grid, GameEntity entity) {
		Dictionary<GameEntity, GridCoordinate> gridToUse = null;

		switch (grid) {
		case Grid.WorldGrid:
			gridToUse = occupiedWorldPositions;
			break;
		case Grid.ScreenGrid:
			gridToUse = occupiedScreenPositions;
			break;
		}
		
		if (gridToUse.ContainsKey(entity)) {
			Debug.Log(entity + " already registered");
			return;
		}

		GridCoordinate position = null;
		switch (grid) {
		case Grid.WorldGrid:
			position = new GridCoordinate(entity.transform.position);
			break;
		case Grid.ScreenGrid:
			position = new GridCoordinate(entity.transform.position - instance.screenGridOrigin.position);
			break;
		}

		gridToUse.Add(entity, position);
	}

	public void UnregisterEntity(Grid grid, GameEntity entity) {
		Dictionary<GameEntity, GridCoordinate> gridToUse = null;
		
		switch (grid) {
		case Grid.WorldGrid:
			gridToUse = occupiedWorldPositions;
			break;
		case Grid.ScreenGrid:
			gridToUse = occupiedScreenPositions;
			break;
		}
		
		gridToUse.Remove(entity);
	}
	
	public static GridCoordinate WorldToScreenGridCoords(Transform t) {
		return WorldToScreenGridCoords(t.position);
	}
	
	public static GridCoordinate WorldToScreenGridCoords(GridCoordinate coord) {
		return WorldToScreenGridCoords(coord.ToVector3());
	}
	
	public static GridCoordinate WorldToScreenGridCoords(Vector3 position) {
		if (instance == null) return new GridCoordinate(position);

		return new GridCoordinate(position - instance.screenGridOrigin.position);
	}
	
	public static Vector3 ScreenCoordsToWorldPosition(GridCoordinate coords) {
		Vector3 position = coords.ToVector3();
		return position + instance.screenGridOrigin.position;
	}
	
	public static GridCoordinate ScreenCoordsToWorldGrid(GridCoordinate coords) {
		return new GridCoordinate(ScreenCoordsToWorldPosition(coords));
	}
}
