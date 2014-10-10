using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SpineMultiSkeleton))]
public class SpineMultiSkeletonEditor : Editor {
	SpineMultiSkeleton multiSkeleton;
	int currentDataIndex = 0;
	int currentAnimationIndex = 0;

	bool expandData = false;
	Vector2 animationScrollPos = Vector2.zero;

	void OnEnable() {
		multiSkeleton = target as SpineMultiSkeleton;
		multiSkeleton.Init ();

		if (multiSkeleton.skeleton == null)
			multiSkeleton.skeleton = multiSkeleton.GetComponent<SkeletonAnimation> ();

		if (multiSkeleton.currentSkeletonData == null) {
				multiSkeleton.currentSkeletonData = multiSkeleton.skeleton.skeletonDataAsset;
		} else {
			currentDataIndex = multiSkeleton.skeletonDataAssets.IndexOf(multiSkeleton.currentSkeletonData);
		}
	}

	public override void OnInspectorGUI() {
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		string[] skeletonDataList = new string[multiSkeleton.skeletonDataAssets.Count];
		for (int i = 0; i < multiSkeleton.skeletonDataAssets.Count; i++) {
			if (multiSkeleton.skeletonDataAssets [i] != null)
				skeletonDataList [i] = multiSkeleton.skeletonDataAssets [i].name;
		}

		if (multiSkeleton.skeletonDataAssets.Count == 0 )
			EditorGUILayout.HelpBox ("Add SkeletonDataAssets to list!", MessageType.Warning);
		else if (multiSkeleton.currentSkeletonData == null)
			multiSkeleton.currentSkeletonData = multiSkeleton.skeletonDataAssets[0];

		int newDataIndex = EditorGUILayout.Popup ("Active skeleton: ", currentDataIndex, skeletonDataList);

		if (multiSkeleton.currentSkeletonData != null) {
			DisplayActiveSkeletonData();
		}

		if (GUI.changed) {
			if (currentDataIndex != newDataIndex) {
				currentDataIndex = newDataIndex;
				multiSkeleton.currentSkeletonData = multiSkeleton.skeletonDataAssets[currentDataIndex];
			}
		}
	}

	void DisplayActiveSkeletonData() {

		Spine.SkeletonData activeData = multiSkeleton.currentSkeletonData.GetSkeletonData(true);

		string[] animations = new string[activeData.Animations.Count + 1];
		animations[0] = "<None>";

		for (int i = 0; i < activeData.Animations.Count; i++) 
			animations[i+1] = activeData.Animations[i].Name;

		int newAnimationIndex = EditorGUILayout.Popup("Preview animation: ", currentAnimationIndex, animations);

		if (GUI.changed) {
			if (newAnimationIndex != currentAnimationIndex) {
				currentAnimationIndex = newAnimationIndex;

				multiSkeleton.SetAnimation(animations[currentAnimationIndex], 0, true);
			}
		}
	}
}
