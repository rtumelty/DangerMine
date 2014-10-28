using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

[ExecuteInEditMode]
[AddComponentMenu("Spine/MultiSkeleton")]
public class SpineMultiSkeleton : MonoBehaviour {
	public List<SkeletonDataAsset> skeletonDataAssets;
	
	Dictionary<string, int> animationReferences;
	public SkeletonAnimation skeleton;

	public SkeletonDataAsset currentSkeletonData {
		get {
			if (skeleton == null) return null;
			return skeleton.skeletonDataAsset;
		}
		set {
			if (!skeletonDataAssets.Contains(value))
				skeletonDataAssets.Add(value);

			SetSkeleton(value);
		}
	}

	public bool reserved;
	
	string currentAnimation = "";

	public string CurrentAnimation {
		get {
			return currentAnimation;
		}
	}
	
	void Awake() {
		Init ();
	}

	public void Init() {
		if (skeleton == null)
			skeleton = GetComponent<SkeletonAnimation> ();

		if (animationReferences == null)
			animationReferences = new Dictionary<string, int> ();

		if (skeletonDataAssets == null)
			skeletonDataAssets = new List<SkeletonDataAsset> ();
		
		for (int i = 0; i < skeletonDataAssets.Count; i++) {
			if (skeletonDataAssets[i] != null) {
				SkeletonData data = skeletonDataAssets[i].GetSkeletonData(true);
				
				foreach (Spine.Animation animation in data.Animations) {
					if (!animationReferences.ContainsKey(animation.Name))
					    animationReferences.Add(animation.Name, i);
					else {
						int index;
						animationReferences.TryGetValue(animation.Name, out index);
						if (i != index) {
							Debug.LogWarning("Multiple animations with the same name. All animations must have unique names");
							Debug.Log(gameObject + " " + animation.name);
						}
					}
				}
			}
		}
	}
	
	public void SetAnimation(string animationName, int track, bool loop = true, float length = 0f) {
		StartCoroutine (_SetAnimation (animationName, track, loop, length));
	}

	IEnumerator _SetAnimation(string animationName, int track, bool loop = true, float length = 0f) {
		if (reserved)
			yield break;

		if (skeleton.state == null) {
		}
		else if (skeleton.state.GetCurrent (track) != null) {
			if (skeleton.state.GetCurrent (track).Animation.name == animationName)
				yield break;
		}

		currentAnimation = "";

		try {
			int skeletonDataIndex;
			if (animationReferences.TryGetValue (animationName, out skeletonDataIndex)) {
				if (!skeleton.skeletonDataAsset.Equals(skeletonDataAssets[skeletonDataIndex])) {
					SetSkeleton(skeletonDataAssets[skeletonDataIndex]);
				}

				skeleton.AnimationName = "";
				skeleton.state.SetAnimation(track, animationName, loop);
				/*
				while (skeleton.state.GetCurrent(track) == null || currentAnimation != animationName) {
					skeleton.AnimationName = "";
					skeleton.state.SetAnimation(track, animationName, loop);
					Debug.Log ("setting animation");

					TrackEntry currentTrack = skeleton.state.GetCurrent(track);
					if (currentTrack != null) {
						Debug.Log ("loaded animation: " + currentTrack.Animation.Name);
						if (currentTrack.Animation.Name == animationName) {
							Debug.Log("matches expected: " + currentTrack.Animation.Name);
							currentAnimation = animationName;
						}
					}
					else {
						Debug.Log ("track null - retrying next frame");
						yield return new WaitForSeconds(Time.deltaTime);
					}
				}*/
				
				if (loop && length != 0)
					ClearTrack(track, length);
			} else
				yield break;
			yield break;
		}
		catch (Exception e) { 
			Debug.LogError(e.Message);
			yield break; 
		}
	}
	
	public void ClearTrack(int track, float delay = 0f) {
		StartCoroutine(_ClearTrack(track, delay));
	}
	
	IEnumerator _ClearTrack(int track, float delay) {
		if (delay != 0)
			yield return new WaitForSeconds (delay);
	
		skeleton.state.ClearTrack (track);
		reserved = false;
	}

	void SetSkeleton(SkeletonDataAsset skeletonDataAsset) {
		skeleton.skeletonDataAsset = skeletonDataAsset;

		if (skeleton.skeletonDataAsset.atlasAsset != null)
			skeleton.skeletonDataAsset.atlasAsset.Reset();
		skeleton.Reset();
	}
}
