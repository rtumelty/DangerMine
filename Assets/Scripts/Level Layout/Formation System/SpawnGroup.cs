using UnityEngine;
using System.Collections;

public class SpawnGroup : ScriptableObject {
	public string name = "new formation";
	
	public int height = 5;
	public int width;
	
	public float minimumDistance;
	public float maximumDistance;
	
	public float probabilityWeight;
	

}
