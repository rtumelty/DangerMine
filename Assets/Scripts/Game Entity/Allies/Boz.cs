using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boz : Ally {
	public override void UpdateTargets(List<GameEntity> targets) {
		attackTargets.Clear();
		for (int i = 0; i < targets.Count; i++) {
			if (targets[i] is Enemy || targets[i] is Rock )
				attackTargets.Add(targets[i]);
		}
		
		if (attacking && attackTargets.Count == 0) {
			attacking = false;
		} else if (!attacking && attackTargets.Count > 0 && canAttack) {
			StartCoroutine("Attack");
		}
	}
}
