using UnityEngine;
using System.Collections;

public class DistanceCounter : MonoBehaviour {
	
	[SerializeField] float updateRate;
	private float count = 0;
	private int localDistanceReference;
	private TextMesh myTextMesh;
	
	void Start ()
	{
		myTextMesh = GetComponent<TextMesh> ();
	}
	
	void Update ()
	{
		//Fix updates per second.
		count += Time.deltaTime;
		if(count > updateRate)
		{
			count = 0;
			return;
		}
		
		//Update Distance & UI Values.
		localDistanceReference = SpawnObject.cameraDistanceCovered;
		myTextMesh.text = "Distance:" + localDistanceReference.ToString ();
	}
}
