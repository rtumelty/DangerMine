using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Formation : SpawnGroup {
	public int interval;
	public List<GridCoordinate> spawnPoints;
	public List<FormationProfile> profiles;

	public Vector2 scrollPos = default(Vector2);
	public bool expandSpawnPoints = false;
	public Vector2 scrollSpawnPoints = default(Vector2);
	public bool expandProfiles = false;
	public Vector2 scrollProfiles = default(Vector2);

	public void Initialize() {
		
		if (spawnPoints == null)
			spawnPoints = new List<GridCoordinate>();
		
		if (profiles == null)
			profiles = new List<FormationProfile>();
	}

	List<FormationProfile> GetActiveProfiles(int distance) {
		List<FormationProfile> activeProfiles = new List<FormationProfile>();
		for(int i = 0; i < profiles.Count; i++)
		{ 
			if (profiles[i].maximumDistance == 0) profiles[i].maximumDistance = Mathf.Infinity;

			if (profiles[i].minimumDistance <= distance && profiles[i].maximumDistance >= distance)
				activeProfiles.Add(profiles[i]);
		}
		
		if (activeProfiles.Count == 0) {
			Debug.LogError("No active spawn groups at distance " + distance);
			return null;
		}
		
		return activeProfiles;
	}
	
	public FormationProfile ChooseProfile(int distance) {
		List<FormationProfile> activeProfiles = GetActiveProfiles(distance);
		if (activeProfiles == null) return null;
		
		float combinedWeights = 0;
		foreach (FormationProfile profile in activeProfiles)
			combinedWeights += profile.probabilityWeight;
		
		float spawnValue = Random.value;
		
		for (int i = 0; i < activeProfiles.Count; i++) {
			spawnValue -= activeProfiles[i].probabilityWeight / combinedWeights;
			if (spawnValue < 0 || i+1 == activeProfiles.Count)
				return activeProfiles[i];
		}
		
		return null;
	}

}
