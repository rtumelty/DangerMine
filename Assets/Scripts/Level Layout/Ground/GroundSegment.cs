﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct FormationEntry {
	[SerializeField] public Formation formation;
	[SerializeField] public FormationProfile profile;
	[SerializeField] public int difficulty;
}

public class GroundSegment : MonoBehaviour {
	public Transform connectPoint;
	[SerializeField] FormationEntry defaultFormation;
	[SerializeField] List<FormationEntry> goldEntries;
	[SerializeField] List<FormationEntry> enemyEntries;
	[SerializeField] List<FormationEntry> avoidEntries;
	[SerializeField] List<FormationEntry> obstacleEntries;
	[SerializeField] List<FormationEntry> mixedEntries;

	[SerializeField] Transform decalTransform;
	[SerializeField] List<GameObject> decals;
	GameObject activeDecal;
	static int nextDecalDistance = 0;
	static int minDecalInterval = 0;
	static int maxDecalInterval = 0;
	static float decalOdds = 50f;

	Hole[] holes = null;

	bool debug = false;

	void Awake() {
		holes = GetComponentsInChildren<Hole>();
	}

	void OnEnable() {
		foreach (Hole hole in holes) 
			hole.gameObject.SetActive(true);

		if (decalTransform != null && (LevelManager.CameraDistanceCoveredInt > nextDecalDistance)) {
			if (Random.value < decalOdds) {
				if (decals.Count == 0) {
					Debug.Log("No decals in list");
				}
				else {
					activeDecal = PrefabPool.GetPool(decals[Random.Range(0, decals.Count)]).Spawn(decalTransform.position);
					nextDecalDistance = LevelManager.CameraDistanceCoveredInt + Random.Range(maxDecalInterval, maxDecalInterval);
				}
			}
		}
	}

	void OnDisable() {
		if (activeDecal != null) {
			activeDecal.SetActive(false);
			activeDecal = null;
		}
	}

	public void UpdateSortingOrder(int order) {
		renderer.sortingOrder = order;

		if (activeDecal != null)
			activeDecal.renderer.sortingOrder = order + 1;
	}

	public FormationEntry GetActiveProfile(int difficulty) {
		if (difficulty == 0) {
			PrintDebug("Difficulty 0 - placing empty formation");
			return defaultFormation;
		}
		
		List<FormationEntry> sectionEntries = null;
		List<FormationEntry> validEntries = new List<FormationEntry>();

		switch (LevelManager.CurrentSection.sectionType) {
		case Section.SectionType.Gold:
			sectionEntries = goldEntries;
			break;
		case Section.SectionType.Enemy:
			sectionEntries = enemyEntries;
			break;
		case Section.SectionType.Avoid:
			sectionEntries = avoidEntries;
			break;
		case Section.SectionType.Obstacle:
			sectionEntries = obstacleEntries;
			break;
		case Section.SectionType.Mixed:
			sectionEntries = mixedEntries;
			break;
		}

		PrintDebug("Section type: " + LevelManager.CurrentSection.sectionType);

		foreach (FormationEntry entry in sectionEntries) {
			if (entry.difficulty == difficulty)
				validEntries.Add(entry);
		}

		if (validEntries.Count == 0 && difficulty != 0)
			return GetActiveProfile(difficulty - 1);

		FormationEntry formationEntry = validEntries[Random.Range(0, validEntries.Count)];
		PrintDebug("Placing formation " + formationEntry.formation + ", profile " + formationEntry.profile);
		return formationEntry;
	}

	void PrintDebug(string content) {
		if (!debug) return;

		Debug.Log(content);
	}
}
