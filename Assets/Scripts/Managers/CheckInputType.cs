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
				GameObject go = new GameObject("_InputManager");
				instance = go.AddComponent<CheckInputType>();
			}
			return instance;
		}
	}
	 

	void Awake() 
	{
		if (instance != null) {	
			Destroy (this);
		} else
			instance = this;

	}


	void Update()
	{
#if !(UNITY_STANDALONE || UNITY_EDITOR)
		if (Input.touchCount == 0) return;
#endif

#if UNITY_STANDALONE || UNITY_EDITOR

		if(Input.GetMouseButtonUp(0))
#else

		if(Input.touches[0].phase == TouchPhase.Ended)
#endif
		{
			TOUCH_TYPE = InputType.TOUCHRELEASE_TYPE;
		}

#if UNITY_STANDALONE || UNITY_EDITOR

		else if(Input.GetMouseButtonDown(0))
#else
	
		else if(Input.touches[0].phase == TouchPhase.Began)
#endif
		{
			TOUCH_TYPE = InputType.TOUCHBEGAN_TYPE;
		}

#if !UNITY_STANDALONE || !UNITY_EDITOR
		else if(Input.touchCount == 2)
		{
			TOUCH_TYPE = InputType.DUALTOUCH_TYPE;
		}
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
		
		else if(Input.GetMouseButton(0))
		{
		
#elif UNITY_ANDROID
		
		else if(Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary)
		{
#endif
		TOUCH_TYPE = InputType.DRAG_TYPE;
		}

		else
		{
			TOUCH_TYPE = InputType.NO_TYPE;
		}
	}
}






