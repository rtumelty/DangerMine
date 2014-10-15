using UnityEngine;
using System.Collections;

public class Miner : Ally {
	[SerializeField] protected string animationNamePrefix;
	[SerializeField] protected string walkAnimationSuffix;
	[SerializeField] protected string attackAnimationSuffix;
	[SerializeField] protected string deathAnimationSuffix;

	protected override void OnDisable() {
		
		base.OnDisable ();

		int minerNumber = Random.Range (1, 7);
		
		walkAnimation = animationNamePrefix + minerNumber.ToString () + walkAnimationSuffix;
		attackAnimation = animationNamePrefix + minerNumber.ToString () + attackAnimationSuffix;
		deathAnimation = animationNamePrefix + minerNumber.ToString () + deathAnimationSuffix;

		
		mySpineMultiSkeleton.SetAnimation (walkAnimation, 0);

	}
}
