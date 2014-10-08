using UnityEngine;
using System.Collections;

public class PlayerCharacterPlacement : MonoBehaviour {

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

			//Checks if character is placed over valid lane and set snap position

			if(transform.position.y < 3 && transform.position.y > -3)
			{
				theHighLight.transform.position = new Vector3(transform.position.x, transform.position.y, theHighLight.transform.position.z);
				mySnapPoint = transform.position;
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
