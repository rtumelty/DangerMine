using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FormationProfile))]
public class ProfileEditor : Editor {

	public override void OnInspectorGUI() {
		FormationProfile formationProfile = target as FormationProfile;
		formationProfile.ShowGUI();

		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
	}
}
