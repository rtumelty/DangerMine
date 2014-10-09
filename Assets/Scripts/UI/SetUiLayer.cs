using UnityEngine;
using System.Collections;

public class SetUiLayer : MonoBehaviour 
{
	void Start () 
	{
		gameObject.renderer.sortingLayerName = "Overlay";
	}

}
