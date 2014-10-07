using UnityEngine;
using System.Collections;

public class PlayerCharacterPlacementComponent : MonoBehaviour {

	private bool released = false;
	private Vector3 mySnapPoint;
	private Vector3 myWorldPos;
	private GameObject theHighLight;
	private Vector3 defaultHighlightPosition;

	void Start()
	{
		theHighLight = GameObject.FindGameObjectWithTag ("HL");
		defaultHighlightPosition = theHighLight.transform.position;
	}


	void Update()
	{
		if(released == false)
		{
			//Snaps character to cursor

			Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10f);
			transform.position = new Vector3(Mathf.RoundToInt(newPosition.x), Mathf.RoundToInt(newPosition.y), Mathf.RoundToInt(newPosition.z));

			theHighLight.transform.position = new Vector3(transform.position.x, transform.position.y, theHighLight.transform.position.z);
			myWorldPos = transform.TransformPoint(transform.position);

			//Checks if character is placed over valid lane and set snap position

			if(myWorldPos.y/2 <= 2.5 && 
			   myWorldPos.y/2 >= -2.5)
			{
				mySnapPoint = new Vector3((int)myWorldPos.x/2, (int) myWorldPos.y/2, myWorldPos.z);
			}
			else
			{
				mySnapPoint = Vector3.zero;
			}
		
			//Checks for release of character. Snaps to lane or returns to pool if no valid lane.

			if(Input.GetMouseButtonUp(0) && 
			   mySnapPoint != Vector3.zero)
			{
				released = true;
				GetComponent<Character>().enabled = true;
				transform.position = mySnapPoint;
				theHighLight.transform.position = defaultHighlightPosition;
			}
			else if(Input.GetMouseButtonUp(0))
			{
				gameObject.SetActive(false);
				theHighLight.transform.position = defaultHighlightPosition;
			}
		}
	}
}
