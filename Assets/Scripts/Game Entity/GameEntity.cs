using UnityEngine;
using System.Collections;

public enum Allegiance {
	Ally,
	Enemy,
	Neutral
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class GameEntity : MonoBehaviour {
	
	[SerializeField] protected Allegiance _allegiance;
	[SerializeField] protected int health = 10;
	[SerializeField] protected float currentHealth;
	protected GridCoordinate worldGridCoords;
	protected GridCoordinate screenGridCoords;
	protected Renderer[] renderers;

	public GridCoordinate ScreenGridCoords {
		get {
			return screenGridCoords;
		}
		set {
			screenGridCoords = value;
		}
	}
	
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
		worldGridCoords = transform.position as GridCoordinate;
		screenGridCoords = null;

		UpdateSortingLayer();
		GridManager.Instance.RegisterEntity(GridManager.Grid.WorldGrid, this);
		
		currentHealth = health;

		foreach (Renderer r in renderers) {
			if (r is SpriteRenderer) {
				MaterialPropertyBlock block = new MaterialPropertyBlock();

				r.GetPropertyBlock(block);
				block.AddFloat("_FlashAmount", 0);
				r.SetPropertyBlock(block);
			}
		}
	}
	
	protected virtual void OnDisable() {
		targetable = false;

		if (GridManager.Instance != null) {
			GridManager.Instance.UnregisterEntity(GridManager.Grid.WorldGrid, this);
			GridManager.Instance.UnregisterEntity(GridManager.Grid.ScreenGrid, this);
		}
	}

	protected virtual void Hit(Character character) {
		/*
		Debug.Log ("Attacked, taking " + character.AttackStrength + " damage per second");
		currentHealth = Mathf.Clamp (currentHealth - (character.AttackStrength * Time.deltaTime), 0, 9999);
		*/

		currentHealth = Mathf.Clamp(currentHealth - character.AttackStrength, 0, 9999);

		foreach (Renderer r in renderers) {
			if (r is SpriteRenderer)
				StartCoroutine(TintSpriteRenderer(r as SpriteRenderer));
		}

		if (currentHealth == 0)
			Die ();
	}

	protected IEnumerator TintSpriteRenderer(SpriteRenderer r) {
		MaterialPropertyBlock block = new MaterialPropertyBlock();

		r.GetPropertyBlock(block);
		block.AddFloat("_FlashAmount", .7f);
		r.SetPropertyBlock(block);

		yield return new WaitForSeconds(.2f);
		
		block.AddFloat("_FlashAmount", 0);
		r.SetPropertyBlock(block);
	}
	
	protected virtual void Die() {
		gameObject.SetActive (false);
	}

	public void UpdateSortingLayer() {
//		Debug.Log("New layer: " + "Lane_" + worldGridCoords.y);
		foreach ( Renderer rend in renderers) {
			rend.sortingLayerName = "Lane_" + worldGridCoords.y;
		}

	}
}
