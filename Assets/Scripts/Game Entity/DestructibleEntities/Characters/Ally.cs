using UnityEngine;
using System.Collections;

public class Ally : Character {
	protected override void Update() {
		targetPosition = (WorldCoords + new GridCoordinate(1, 0)).ToVector3();

		base.Update();
	}
}
