using UnityEngine;
using System.Collections;

public class MadScreenStats : MonoBehaviour {

	private TextMesh score;
	private TextMesh distance;
	private TextMesh goldEarned;

	void Start()
	{
		score = transform.Find ("Score").GetComponent<TextMesh> ();
		distance = transform.Find ("Distance").GetComponent<TextMesh> ();
		goldEarned = transform.Find ("Gold Earned").GetComponent<TextMesh> ();


		score.text = "Score  " + GlobalManagement.SCORE.ToString();
		distance.text = "Distance  " + GlobalManagement.LAST_DISTANCE_COVERED.ToString();
		goldEarned.text = "Gold Earned  " + GlobalManagement.GOLD_COLLECTED.ToString();

		WipeLevelResults ();
	}


	void WipeLevelResults()
	{
		GlobalManagement.SCORE = 0;
		GlobalManagement.LAST_DISTANCE_COVERED = 0;
		GlobalManagement.GOLD_COLLECTED = 0;
	} 
}
