using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Formation : ScriptableObject {
	public string name = "new formation";

	public int height;
	public int width;

	public int minimumDistance;
	public int maximumDistance;
	public int interval;

	public float probabilityWeight;

	public List<GridCoordinate> spawnPoints;
	public List<FormationProfile> profiles;

	bool expand = false;
	bool expandSpawnPoints = false;
	Vector2 scrollSpawnPoints = default(Vector2);
	bool expandProfiles = false;
	Vector2 scrollProfiles = default(Vector2);

	public void DisplayFormation() {
		expand = EditorGUILayout.Foldout(expand, name);

		if (expand) {
			name = EditorGUILayout.TextField("Formation name", name);
			
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			height = EditorGUILayout.IntField("Height", height);
			width = EditorGUILayout.IntField("Width", width);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			minimumDistance = EditorGUILayout.IntField("Min Distance", minimumDistance);
			maximumDistance = EditorGUILayout.IntField("Max Distance", maximumDistance);
			EditorGUILayout.EndHorizontal();
			
			interval = EditorGUILayout.IntField("Interval before", interval);
			probabilityWeight = EditorGUILayout.FloatField("Probability weight", probabilityWeight);
			
			EditorGUILayout.Space();
			
			expandSpawnPoints = EditorGUILayout.Foldout(expandSpawnPoints, "Spawn points");
			if (expandSpawnPoints) {
				scrollSpawnPoints = EditorGUILayout.BeginScrollView(scrollSpawnPoints, GUILayout.MaxHeight(100));
                for (int i = 0; i < spawnPoints.Count; i++) {
					EditorGUILayout.LabelField("Spawn point " + i);
					EditorGUILayout.BeginHorizontal();
					spawnPoints[i].x = EditorGUILayout.IntField("X", spawnPoints[i].x);
					spawnPoints[i].y = EditorGUILayout.IntField("Y", spawnPoints[i].y);
					if (GUILayout.Button("-", GUILayout.Width(10))) {
						spawnPoints.RemoveAt(i);
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndScrollView();

				if (GUILayout.Button("Add")) {
					spawnPoints.Add(new GridCoordinate(0, 0));
				}
			}
			
			expandProfiles = EditorGUILayout.Foldout(expandProfiles, "Profiles");
			if (expandSpawnPoints) {
				scrollProfiles = EditorGUILayout.BeginScrollView(scrollProfiles, GUILayout.MinHeight(100));
				for (int i = 0; i < profiles.Count; i++) {
					EditorGUILayout.BeginHorizontal();
					profiles[i].Expand = EditorGUILayout.Foldout(profiles[i].Expand, profiles[i].name);
					if (GUILayout.Button("-", GUILayout.Width(10))) {
						spawnPoints.RemoveAt(i);
					}
					EditorGUILayout.EndHorizontal();
					if (profiles[i].Expand)
						profiles[i].DisplayProfile();
				}
				EditorGUILayout.EndScrollView();
				
				if (GUILayout.Button("Add")) {
					FormationProfile profile = CreateInstance<FormationProfile>();
					profile.formation = this;
					profile.name = "Profile " + profiles.Count.ToString();
					profiles.Add(profile);
				}
        	}
		}
	}
}
