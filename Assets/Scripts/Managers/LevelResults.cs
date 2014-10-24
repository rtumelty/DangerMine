using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelResults : MonoBehaviour {

	[SerializeField] private Text score;
	[SerializeField] private Text distance;
	[SerializeField] private Text goldEarned;

	[SerializeField] float displayTime = .66f;
	bool displayingResults = false;

	public void DisplayResults()
	{	
		displayingResults = true;
		StartCoroutine("_DisplayResults");
	}

	IEnumerator _DisplayResults() {

		for (float elapsedTime = 0; elapsedTime < displayTime; elapsedTime += Time.deltaTime) {
			score.text = ((int)Mathf.Lerp(0, GlobalManagement.SCORE, elapsedTime / displayTime)).ToString();

			yield return new WaitForSeconds(Time.deltaTime);
		}
		score.text = GlobalManagement.SCORE.ToString();

		for (float elapsedTime = 0; elapsedTime < displayTime; elapsedTime += Time.deltaTime) {
			distance.text = ((int)Mathf.Lerp(0, GlobalManagement.LAST_DISTANCE_COVERED, elapsedTime / displayTime)).ToString();

			yield return new WaitForSeconds(Time.deltaTime);
		}
		distance.text = GlobalManagement.LAST_DISTANCE_COVERED.ToString();

		for (float elapsedTime = 0; elapsedTime < displayTime; elapsedTime += Time.deltaTime) {
			goldEarned.text = ((int)Mathf.Lerp(0, (int)GlobalManagement.GOLD_COLLECTED, elapsedTime / displayTime)).ToString();
			
			yield return new WaitForSeconds(Time.deltaTime);
		}
		
		goldEarned.text = GlobalManagement.GOLD_COLLECTED.ToString();
		
		displayingResults = false;
	}

	void Update() {
		if (displayingResults && CheckInputType.TOUCH_TYPE == InputType.TOUCHBEGAN_TYPE) {
			StopCoroutine("_DisplayResults");

			score.text = GlobalManagement.SCORE.ToString();
			distance.text = GlobalManagement.LAST_DISTANCE_COVERED.ToString();
			goldEarned.text = GlobalManagement.GOLD_COLLECTED.ToString();
		}
	}
	
	public void WipeLevelScores()
	{
		GlobalManagement.SCORE = 0;
		GlobalManagement.LAST_DISTANCE_COVERED = 0;
		GlobalManagement.GOLD_COLLECTED = 0;
	} 
	
	public void ResetCounters()
	{
		score.text = "0";
		distance.text = "0";
		goldEarned.text = "0";
	} 
}
