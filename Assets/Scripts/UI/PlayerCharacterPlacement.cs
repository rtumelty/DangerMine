using UnityEngine;
using System.Collections;

public class PlayerCharacterPlacement : MonoBehaviour {
	
	private Vector3 mySnapPoint;
	private Vector3 myWorldPos;
	private Vector3 defaultHighlightPosition;

	private bool released = false;

	private GameObject theHighLight;

	void Start()
	{
		theHighLight = GameObject.FindGameObjectWithTag ("HL");
		defaultHighlightPosition = theHighLight.transform.position;
	}


	void Update()
	{
		if(released)
		{
			return;
		}

		if(BuildPlayerUnitButton.click == true)
		{
			StickToCursor();
			if(BuildPlayerUnitButton.stillOverButton == false)
			{
				CheckForRelease();
			}
		}
		else
		{
			StickToCursor();
			if(BuildPlayerUnitButton.stillOverButton == false)
			{
				CheckForRelease();
			}
		}
	}


	void StickToCursor()
	{
		//Snaps character to cursor
		
		Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 8f);
		transform.position = new Vector3(Mathf.RoundToInt(newPosition.x), Mathf.RoundToInt(newPosition.y), Mathf.RoundToInt(newPosition.z));
		
		//Clamps highlight over last valid lane position
		
		if(transform.position.y < 3 && transform.position.y > -3)
		{
			theHighLight.transform.position = new Vector3(transform.position.x, transform.position.y, theHighLight.transform.position.z);
			mySnapPoint = transform.position;
		}
		else
		{
			mySnapPoint = Vector3.zero;
		}
	}


	void CheckForRelease()
	{
		//Checks for release of character. Snaps to lane or returns to pool if no valid lane.
		
		if(CheckInputType.TOUCH_TYPE == InputType.TOUCHRELEASE_TYPE && 
		   mySnapPoint != Vector3.zero)
		{
			GetComponent<Character>().enabled = true;
			theHighLight.transform.position = defaultHighlightPosition;
			released = true;
		}
		else if(CheckInputType.TOUCH_TYPE == InputType.TOUCHRELEASE_TYPE)
		{
			gameObject.SetActive(false);
			theHighLight.transform.position = defaultHighlightPosition;
		}
	}
}
