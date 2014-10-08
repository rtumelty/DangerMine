using UnityEngine;
using System.Collections;
 
public enum InputType
{
	NO_TYPE,
	TAP_TYPE,
	DRAG_TYPE,
	DUALTOUCH_TYPE
}
public class CheckInputType : MonoBehaviour
{
	public static InputType TOUCH_TYPE;

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

		if(Input.touches[0].phase == TouchPhase.End)
#endif
		{
			TOUCH_TYPE = InputType.TAP_TYPE;
		}

#if UNITY_STANDALONE || UNITY_EDITOR

		if(false)
#else
	
		else if(Input.touchCount == 2)
#endif
		{
			TOUCH_TYPE = InputType.DUALTOUCH_TYPE;
		}

#if UNITY_STANDALONE || UNITY_EDITOR
		
		if(Input.GetMouseButton(0))
#else
		
		else if(Input.touches[0] > 0)
#endif
		{
			TOUCH_TYPE = InputType.DRAG_TYPE;
		}

		else
		{
			TOUCH_TYPE = InputType.NO_TYPE;
		}
	}
}






