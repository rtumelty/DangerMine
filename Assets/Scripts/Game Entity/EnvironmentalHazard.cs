using UnityEngine;
using System.Collections;

public class EnvironmentalHazard : GameEntity {

	protected override void OnEnable() {
		base.OnEnable();

		collider2D.isTrigger = true;
		State = EntityState.Active;
	}

}
