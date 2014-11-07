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
	Vector2 scrollPos = default(Vector2);
	bool expandSpawnPoints = false;
	Vector2 scrollSpawnPoints = default(Vector2);
	bool expandProfiles = false;
	Vector2 scrollProfiles = default(Vector2);

	public void Initialize() {
		
		if (spawnPoints == null)
			spawnPoints = new List<GridCoordinate>();
		
		if (profiles == null)
			profiles = new List<FormationProfile>();
	}

	public void DisplayFormation() {

		expand = EditorGUILayout.Foldout(expand, name);

		if (expand) {
			name = EditorGUILayout.TextField("Formation name", name);
			
			EditorGUILayout.Space();
			
			height = EditorGUILayout.IntField("Height", height);
			width = EditorGUILayout.IntField("Width", width);

			EditorGUILayout.Space();
			
			minimumDistance = EditorGUILayout.IntField("Min Distance", minimumDistance);
			maximumDistance = EditorGUILayout.IntField("Max Distance", maximumDistance);
			
			interval = EditorGUILayout.IntField("Interval before", interval);
			probabilityWeight = EditorGUILayout.FloatField("Probability weight", probabilityWeight);
			
			EditorGUILayout.Space();
			
			expandSpawnPoints = EditorGUILayout.Foldout(expandSpawnPoints, "Spawn points");
			if (expandSpawnPoints) {
				scrollSpawnPoints = EditorGUILayout.BeginScrollView(scrollSpawnPoints, false, true, GUILayout.MinHeight(200));
	                for (int i = 0; i < spawnPoints.Count; i++) {
						EditorGUILayout.LabelField("Spawn point " + i);
							spawnPoints[i].x = EditorGUILayout.IntField("X", spawnPoints[i].x);
							spawnPoints[i].y = EditorGUILayout.IntField("Y", spawnPoints[i].y);
							if (GUILayout.Button("Remove")) {
								spawnPoints.RemoveAt(i);
							}
					}
				EditorGUILayout.EndScrollView();

				if (GUILayout.Button("Add")) {
					spawnPoints.Add(new GridCoordinate(0, 0));
				}
			}
			
			expandProfiles = EditorGUILayout.Foldout(expandProfiles, "Profiles");
			if (expandProfiles) {
				scrollProfiles = EditorGUILayout.BeginScrollView(scrollProfiles, false, true, GUILayout.MinHeight(100), GUILayout.ExpandWidth(true));
					for (int i = 0; i < profiles.Count; i++) {
						if (profiles[i] == null) {
							profiles[i] = CreateInstance<FormationProfile>();
							profiles[i].name = "Profile " + profiles.Count.ToString();
							profiles[i].formation = this;
						}

						EditorGUILayout.BeginHorizontal();
							profiles[i].Expand = EditorGUILayout.Foldout(profiles[i].Expand, profiles[i].name);
						EditorGUILayout.EndHorizontal();
						
						if (profiles[i].Expand)
							profiles[i].DisplayProfile();

						if (GUILayout.Button("Remove")) {
							profiles.RemoveAt(i);
						}
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
