using UnityEngine;
using System.Collections;

public class AnimationStateManager : MonoBehaviour {
	public enum AnimationState {
		Walk,
		Attack,
		Dying
	}

	SpineMultiSkeleton multiSkeleton;

	public SpineMultiSkeleton MultiSkeleton {
		get {
			return multiSkeleton;
		}
	}

	AnimationState state;

	public AnimationState State {
		get {
			return state;
		}
		set {
			state = value;

			StopAllCoroutines();
			switch (state) {
			case AnimationState.Walk:
				StartCoroutine(SetAnimation(walkAnimation, true));
            	break;
			case AnimationState.Attack:
				StartCoroutine(SetAnimation(attackAnimation, true));
				break;
			case AnimationState.Dying:
				StartCoroutine(SetAnimation(deathAnimation, false));
				break;
			}
		}
	}

	public string walkAnimation;
	public string attackAnimation;
	public string deathAnimation;

	void Awake() {
		multiSkeleton = GetComponentInChildren<SpineMultiSkeleton>();
	}

	public void ChangeAnimations(string walk, string attack, string death) {
		walkAnimation = walk;
		attackAnimation = attack;
		deathAnimation = death;
	}

	IEnumerator SetAnimation(string name, bool loop) {
		while (true) {
			if (multiSkeleton.skeleton.state.GetCurrent(0).Animation.Name != name) 
				multiSkeleton.SetAnimation(name, 0, loop);
			yield return new WaitForSeconds(Time.deltaTime);
		}

	}
}
