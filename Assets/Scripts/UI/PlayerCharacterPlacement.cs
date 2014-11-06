using UnityEngine;
using System.Collections;

public class PlayerCharacterPlacement : MonoBehaviour {
	
	private Vector3 mySnapPoint;
	private Vector3 myWorldPos;
	private Vector3 defaultHighlightPosition;

	private BuildPlayerUnitButton purchaseButton;

	public BuildPlayerUnitButton PurchaseButton {
		get {
			return purchaseButton;
		}
		set {
			purchaseButton = value;
		}
	}

	[SerializeField] private bool released = false;

	private float initialClick = 0;
	private float clickThreshold = 0.2f;

	private GameObject theHighLight;
	private GameEntity entity;

	void Awake()
	{
		entity = GetComponent<GameEntity>();
		theHighLight = GameObject.FindGameObjectWithTag ("HL");
		defaultHighlightPosition = theHighLight.transform.position;
	}

	void OnEnable() {
		released = false;
		initialClick = Time.time;
		entity.enabled = false;
	}


	void Update()
	{
		if(released)
		{
			return;
		}

		StickToCursor();
		CheckForRelease();
	}


	void StickToCursor()
	{
		//Snaps character to cursor
		
		Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 8f);
		transform.position = new Vector3(Mathf.RoundToInt(newPosition.x), Mathf.RoundToInt(newPosition.y), Mathf.RoundToInt(newPosition.z));
		
		//Clamps highlight over last valid lane position
		
		if(transform.position.y < 3 && transform.position.y > -3 && transform.position.x > ChaseCollider.Instance.LeadingEdge)
		{
			entity.UpdateSortingLayer();
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
		if (Time.time - initialClick < clickThreshold) return;

		//Checks for release of character. Snaps to lane or returns to pool if no valid lane.
		
		if(CheckInputType.TOUCH_TYPE == InputType.TOUCHRELEASE_TYPE && 
		   mySnapPoint != Vector3.zero && !GridManager.Instance.IsOccupied(Camera.main.ScreenToWorldPoint(Input.mousePosition) as GridCoordinate))
		{
			entity.enabled = true;
			entity.Targetable = true;
			theHighLight.transform.position = defaultHighlightPosition;
			purchaseButton.SendMessage("StartCooldown");
			released = true;
		}
		else if(CheckInputType.TOUCH_TYPE == InputType.TOUCHRELEASE_TYPE)
		{
			gameObject.SetActive(false);
			purchaseButton.Cancelled();
			theHighLight.transform.position = defaultHighlightPosition;
		}
	}
}
