using UnityEngine;
using System.Collections;

public class CheckInputType : MonoBehaviour
{
	public const int NO_TYPE = -1;
	public const int TAP_TYPE = 0;
	public const int DRAG_TYPE = 1;
	public const int DUALTOUCH_TYPE = 2;

	public static int TOUCH_TYPE;

	static CheckInputType instance;
	public static CheckInputType Instance 
	{
		get 
		{
			if (instance == null) 
			{
				GameObject go = new GameObject("_GridManager");
				instance = new CheckInputType();
			}
			return instance;
		}
	}
	 

	void Awake() 
	{
		if (instance != null) 
		{	
			Destroy(this);
		}
		
		DontDestroyOnLoad(this);
	}


	void Update()
	{
#if UNITY_STANDALONE || UNITY_EDITOR

		if(Input.GetMouseButtonUp(0))
#else

		if(Input.touchCount == 1)
#endif
		{
			TOUCH_TYPE = TAP_TYPE;
		}

#if UNITY_STANDALONE || UNITY_EDITOR

		if(false)
#else
	
		else if(Input.touchCount == 2)
#endif
		{
			TOUCH_TYPE = DUALTOUCH_TYPE;
		}

#if UNITY_STANDALONE || UNITY_EDITOR
		
		if(Input.GetMouseButton(0))
#else
		
		else if(Input.touches[0] > 0)
#endif
		{
			TOUCH_TYPE = DRAG_TYPE;
		}

		else
		{
			TOUCH_TYPE = NO_TYPE;
		}
	}
}






