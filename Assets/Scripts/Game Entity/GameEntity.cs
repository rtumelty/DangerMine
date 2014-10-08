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
	protected float currentHealth;
	
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

	protected void Hit(int attackStr) {
		currentHealth = Mathf.Clamp (currentHealth - attackStr, 0, 9999);
		if (currentHealth == 0)
			Die ();
	}
	
	protected void Die() {
		gameObject.SetActive (false);
	}
}
