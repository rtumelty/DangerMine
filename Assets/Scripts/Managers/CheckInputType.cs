using UnityEngine;
using System.Collections;
 
public enum InputType
{
	NO_TYPE,
	TOUCHRELEASE_TYPE,
	TOUCHBEGAN_TYPE,
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
				instance = go.AddComponent<CheckInputType>();
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
			TOUCH_TYPE = InputType.TOUCHRELEASE_TYPE;
		}

#if UNITY_STANDALONE || UNITY_EDITOR

		if(Input.GetMouseButtonDown(0))
#else
	
		if(Input.touches[0].phase == TouchPhase.Began)
#endif
		{
			TOUCH_TYPE = InputType.TOUCHBEGAN_TYPE;
		}

#if UNITY_STANDALONE || UNITY_EDITOR

		else if(false)
#else
	
		else if(Input.touchCount == 2)
#endif
		{
			TOUCH_TYPE = InputType.DUALTOUCH_TYPE;
		}

#if UNITY_STANDALONE || UNITY_EDITOR
		
		else if(!Input.GetMouseButtonUp(0))
		{
			if(Input.GetMouseButton(0))
		
#else
		
		else if(Input.touches[0] > 0)
		{
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
}






