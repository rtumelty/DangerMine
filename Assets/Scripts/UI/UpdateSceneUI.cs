using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UpdateSceneUI : MonoBehaviour {
	
	[SerializeField] Text distanceCounter;
	[SerializeField] Text goldCounter;
	[SerializeField] Text diamondCounter;
	
	void Update ()
	{
		distanceCounter.text = FormationManager.CameraDistanceCovered.ToString();
		goldCounter.text = GlobalManagement.PLAYERGOLD.ToString();
		diamondCounter.text = GlobalManagement.PLAYERDIAMONDS.ToString();
	}
}
