﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Miner : Ally {
	[SerializeField] protected string animationNamePrefix;
	[SerializeField] protected string walkAnimationSuffix;
	[SerializeField] protected string attackAnimationSuffix;
	[SerializeField] protected string deathAnimationSuffix;

	protected override void OnDisable() {
		
		base.OnDisable ();

		int minerNumber = Random.Range (1, 7);
		/*
		walkAnimation = animationNamePrefix + minerNumber.ToString () + walkAnimationSuffix;
		attackAnimation = animationNamePrefix + minerNumber.ToString () + attackAnimationSuffix;
		deathAnimation = animationNamePrefix + minerNumber.ToString () + deathAnimationSuffix;

		mySpineMultiSkeleton.SetAnimation (walkAnimation, 0);
		*/

		animationManager.ChangeAnimations(animationNamePrefix + minerNumber.ToString () + walkAnimationSuffix,
		                                  animationNamePrefix + minerNumber.ToString () + attackAnimationSuffix,
		                                  deathAnimation = animationNamePrefix + minerNumber.ToString () + deathAnimationSuffix);
		animationManager.State = AnimationStateManager.AnimationState.Walk;
	}

	public override void UpdateTargets(List<GameEntity> targets) {
		attackTargets.Clear();
		for (int i = 0; i < targets.Count; i++) {
			if (targets[i] is Enemy || targets[i] is Rock )
				attackTargets.Add(targets[i]);
		}
		
		if (attacking && attackTargets.Count == 0) {
			attacking = false;
		} else if (!attacking && attackTargets.Count > 0) {
			StartCoroutine("Attack");
		}
	}
}
