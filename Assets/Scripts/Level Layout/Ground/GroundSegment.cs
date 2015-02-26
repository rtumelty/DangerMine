using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundSegment : MonoBehaviour {

	[System.Serializable]
	struct FormationEntry {
		[SerializeField] Formation formation;
		[SerializeField] FormationProfile profile;
	}

	public Transform connectPoint;
	[SerializeField] List<FormationEntry> formationEntries;

}
