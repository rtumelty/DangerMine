using UnityEngine;
using System.Collections;

public enum Allegiance {
	Ally,
	Enemy,
	Neutral
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class GameEntity : MonoBehaviour {
	
	[SerializeField] protected Allegiance _allegiance;
	[SerializeField] protected int health = 10;
	[SerializeField] protected float currentHealth;
	protected GridCoordinate gridCoords;
	private Renderer[] renderers;
	
	protected bool targetable = false;
	
	public bool Targetable {
		get {
			return targetable;
		}
		set {
			targetable = value;
		}
	}
	
	public Allegiance allegiance {
		get {
			return _allegiance;
		}
	}

	protected virtual void Awake() {
		renderers = GetComponentsInChildren<Renderer>();

		rigidbody2D.gravityScale = 0;
		rigidbody2D.isKinematic = true;
	}

	protected virtual void OnEnable() {
		gridCoords = transform.position as GridCoordinate;
		UpdateSortingLayer();
		GridManager.Instance.RegisterEntity(this);
		
		currentHealth = health;
	}
	
	protected virtual void OnDisable() {
		targetable = false;

		if (GridManager.Instance != null)
			GridManager.Instance.UnregisterEntity(this);
	}

	protected virtual void Hit(Character character) {
		/*
		Debug.Log ("Attacked, taking " + character.AttackStrength + " damage per second");
		currentHealth = Mathf.Clamp (currentHealth - (character.AttackStrength * Time.deltaTime), 0, 9999);
		*/

		currentHealth = Mathf.Clamp(currentHealth - character.AttackStrength, 0, 9999);

		if (currentHealth == 0)
			Die ();
	}
	
	protected virtual void Die() {
		gameObject.SetActive (false);
	}

	public void UpdateSortingLayer() {
		
		foreach ( Renderer rend in renderers) {
			rend.sortingLayerName = "Lane_" + gridCoords.y;
		}

	}
}
