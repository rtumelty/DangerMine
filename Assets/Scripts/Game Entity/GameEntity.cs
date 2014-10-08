using UnityEngine;
using System.Collections;

public enum Allegiance {
	Ally,
	Enemy,
	Neutral
}

public class GameEntity : MonoBehaviour {
	
	[SerializeField] protected Allegiance _allegiance;
	[SerializeField] protected int health = 10;
	
	public Allegiance allegiance {
		get {
			return _allegiance;
		}
	}
	
	protected void OnEnable() {
		GridManager.Instance.RegisterEntity(this);
	}
	
	protected void OnDisable() {
		GridManager.Instance.UnregisterEntity(this);
	}

}
