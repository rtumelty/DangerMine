using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Character {
	
	private static List<Enemy> activeEnemies;
	
	public static int ActiveEnemies {
		get {
			return activeEnemies.Count;
		}
	}
	

	protected override void Awake() {
		if (activeEnemies == null) {
			activeEnemies = new List<Enemy>();

			foreach (Enemy enemy in FindObjectsOfType<Enemy>()) 
			{
				if (!activeEnemies.Contains(enemy))
					activeEnemies.Add(enemy);
			}
		}

		_allegiance = Allegiance.Enemy;
		
		moveDirection = -1;
		
		base.Awake ();
	}
	
	protected override void OnEnable() {
		base.OnEnable ();
		
		if (!activeEnemies.Contains(this))
			activeEnemies.Add(this);
	}
	
	protected override void OnDisable() {
		base.OnDisable ();
		
		if (activeEnemies.Contains(this))
			activeEnemies.Remove(this);
	}
}
