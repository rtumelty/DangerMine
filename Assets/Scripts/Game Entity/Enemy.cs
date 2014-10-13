using UnityEngine;
using System.Collections;

public class Enemy : Character {
	
	protected static int activeEnemies = 0;
	
	public static int ActiveEnemies {
		get {
			return activeEnemies;
		}
	}
	
	protected override void Awake() {
		_allegiance = Allegiance.Enemy;
		
		moveDirection = -1;
		
		base.Awake ();
	}
	
	protected override void OnEnable() {
		base.OnEnable ();
		activeEnemies++;
	}
	
	protected override void OnDisable() {
		base.OnDisable ();
		activeEnemies--;
	}
}
