using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Formation))]
public class FormationEditor : Editor {

	public override void OnInspectorGUI() {
		Formation formation = target as Formation;
		DisplayFormation(formation);

		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
	}

	public void DisplayFormation(Formation formation) {
		formation.Initialize();
		
		formation.name = EditorGUILayout.TextField("Formation name", formation.name);
		
		EditorGUILayout.Space();
		
		formation.height = EditorGUILayout.IntField("Height", formation.height);
		formation.width = EditorGUILayout.IntField("Width", formation.width);
		
		EditorGUILayout.Space();
		
		formation.minimumDistance = EditorGUILayout.IntField("Min Distance", formation.minimumDistance);
		formation.maximumDistance = EditorGUILayout.IntField("Max Distance", formation.maximumDistance);
		
		formation.interval = EditorGUILayout.IntField("Interval before", formation.interval);
		formation.probabilityWeight = EditorGUILayout.FloatField("Probability weight", formation.probabilityWeight);
		
		EditorGUILayout.Space();
		
		formation.expandSpawnPoints = EditorGUILayout.Foldout(formation.expandSpawnPoints, "Spawn points");
		if (formation.expandSpawnPoints) {
			formation.scrollSpawnPoints = EditorGUILayout.BeginScrollView(formation.scrollSpawnPoints, false, true, GUILayout.MinHeight(200));
			for (int i = 0; i < formation.spawnPoints.Count; i++) {
				EditorGUILayout.LabelField("Spawn point " + i);
				formation.spawnPoints[i].x = EditorGUILayout.IntField("X", formation.spawnPoints[i].x);
				formation.spawnPoints[i].y = EditorGUILayout.IntField("Y", formation.spawnPoints[i].y);
				if (GUILayout.Button("Remove")) {
					formation.spawnPoints.RemoveAt(i);
					foreach (FormationProfile profile in formation.profiles) {
						profile.UpdatePrefabArraySize();
					}
				}
			}
			EditorGUILayout.EndScrollView();
			
			if (GUILayout.Button("Add")) {
				formation.spawnPoints.Add(new GridCoordinate(0, 0));
				foreach (FormationProfile profile in formation.profiles) {
					profile.UpdatePrefabArraySize();
				}
			}
		}
		
		formation.expandProfiles = EditorGUILayout.Foldout(formation.expandProfiles, "Profiles");
		if (formation.expandProfiles) {
			formation.scrollProfiles = EditorGUILayout.BeginScrollView(formation.scrollProfiles, false, true, GUILayout.MinHeight(200), GUILayout.ExpandWidth(true));
			for (int i = 0; i < formation.profiles.Count; i++) {
				if (formation.profiles[i] == null) {
					formation.profiles[i] = new FormationProfile(formation);
				}
				
				EditorGUILayout.BeginHorizontal();
				formation.profiles[i].expand = EditorGUILayout.Foldout(formation.profiles[i].expand, formation.profiles[i].name);
				EditorGUILayout.EndHorizontal();
				
				if (formation.profiles[i].expand)
					DisplayProfile(formation.profiles[i]);
				
				if (GUILayout.Button("Remove")) {
					formation.profiles.RemoveAt(i);
				}
			}
			EditorGUILayout.EndScrollView();
			
			if (GUILayout.Button("Add")) {
				formation.profiles.Add(new FormationProfile(formation));
			}
		}
	}

	public void DisplayProfile(FormationProfile profile) {
		profile.name = EditorGUILayout.TextField("Name", profile.name);
		EditorGUILayout.ObjectField("Formation", profile.formation, typeof(Formation), false);
		
		EditorGUILayout.Space();
		
		profile.minimumDistance = Mathf.RoundToInt( Mathf.Clamp(EditorGUILayout.IntField("Min Distance", profile.minimumDistance), 
		                                                        profile.formation.minimumDistance, profile.formation.maximumDistance));
		
		profile.maximumDistance = Mathf.RoundToInt( Mathf.Clamp(EditorGUILayout.IntField("Max Distance", profile.maximumDistance),
		                                                        profile.formation.minimumDistance, profile.formation.maximumDistance));
		
		profile.probabilityWeight = EditorGUILayout.FloatField("Probability weight", profile.probabilityWeight);
		
		EditorGUILayout.Space();
		
		profile.expandPrefabs = EditorGUILayout.Foldout(profile.expandPrefabs, "Prefabs");
		if (profile.expandPrefabs) {
			if (profile.prefabPoolIds == null) profile.UpdatePrefabArraySize();
			
			profile.scrollPrefabs = EditorGUILayout.BeginScrollView(profile.scrollPrefabs, GUILayout.MinHeight(50));
			
			for (int i = 0; i < profile.prefabPoolIds.Length; i++)
				profile.prefabPoolIds[i] = EditorGUILayout.TextField("Entity " + i.ToString(), profile.prefabPoolIds[i]
				                                             );
			
			EditorGUILayout.EndScrollView();
		}
		
	}
}
