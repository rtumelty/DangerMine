using UnityEngine;
using System.Collections;

public class ButtonHighLight : MonoBehaviour 
{
	public Color myHighLightColor;
	public Color myDefaultColor;
	private TextMesh myButtonText;


	void Start () 
	{
		myButtonText = GetComponentInChildren <TextMesh> ();
	}

/*
	void OnMouseOver()
	{
		myButtonText.color = myHighLightColor;
		myButtonText.text = "PLAY";
		print ("Over");
	}


	void OnMouseExit()
	{
		myButtonText.color = myDefaultColor;
		myButtonText.text = "PLAY";
		print ("Under");
	}
	*/
}
