using UnityEngine;
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
	[SerializeField] List<FormationEntry> goldEntries;
	[SerializeField] List<FormationEntry> enemyEntries;
	[SerializeField] List<FormationEntry> avoidEntries;
	[SerializeField] List<FormationEntry> obstacleEntries;
	[SerializeField] List<FormationEntry> mixedEntries;

	public FormationEntry GetActiveProfile(int difficulty) {
		if (difficulty == 0) {}

		List<FormationEntry> sectionEntries = null;

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

		foreach (FormationEntry entry in sectionEntries) {
			if (entry.difficulty != difficulty)
				sectionEntries.Remove(entry);
		}

		if (sectionEntries.Count == 0 && difficulty >= 0)
			return GetActiveProfile(difficulty - 1);

		return sectionEntries[Random.Range(0, sectionEntries.Count)];
	}
}
