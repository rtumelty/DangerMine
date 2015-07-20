using UnityEngine;
using System.Collections;
 
public enum InputType
{
	NONE,
	TOUCH_RELEASED,
	TOUCH_BEGAN,
	DRAG,
	DUAL_TOUCH
}

public class InputManager : MonoBehaviour
{
	public static InputType TOUCH_TYPE;

	static InputManager instance;
	public static InputManager Instance 
	{
		get 
		{
			if (instance == null) 
			{
				GameObject go = new GameObject("_InputManager");
				instance = go.AddComponent<InputManager>();
			}
			return instance;
		}
	}

    LayerMask inputMask;

	void Awake() 
	{
		if (instance != null) {	
			Destroy (this);
		} else
			instance = this;

        DontDestroyOnLoad(this);

        inputMask = 1 << LayerMask.NameToLayer("UI");
	}


	void Update()
	{
#if !(UNITY_STANDALONE || UNITY_EDITOR)
		if (Input.touchCount == 0) TOUCH_TYPE = InputType.NONE;
#endif

#if UNITY_STANDALONE || UNITY_EDITOR

		if(Input.GetMouseButtonUp(0))
#else

		else if(Input.touches[0].phase == TouchPhase.Ended)
#endif
		{
			TOUCH_TYPE = InputType.TOUCH_RELEASED;
		}

#if UNITY_STANDALONE || UNITY_EDITOR

		else if(Input.GetMouseButtonDown(0))
#else
	
		else if(Input.touches[0].phase == TouchPhase.Began)
#endif
		{
			TOUCH_TYPE = InputType.TOUCH_BEGAN;
		}

#if !UNITY_STANDALONE || !UNITY_EDITOR
		else if(Input.touchCount == 2)
		{
			TOUCH_TYPE = InputType.DUAL_TOUCH;
		}
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
		
		else if(Input.GetMouseButton(0))
		{
		
#elif UNITY_ANDROID
		
		else if(Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary)
		{
#endif
		TOUCH_TYPE = InputType.DRAG;
		}

		else
		{
			TOUCH_TYPE = InputType.NONE;
		}

        // Input processing
        switch (TOUCH_TYPE) { 
            case InputType.TOUCH_BEGAN:

                Vector2 touchPosition;

#if UNITY_EDITOR || UNITY_STANDALONE
			    touchPosition = Input.mousePosition;
#elif UNITY_ANDROID || UNITY_IPHONE
			    touchPosition = Input.touches[0].position;
#endif

		        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
		        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, inputMask);
                
                Ally ally = hit.collider.GetComponentInParent<Ally>();
                if (ally != null)
                {
                    if (Ally.SelectedCharacter == null)
                    {
                        Ally.SelectedCharacter = ally;
                        ally.SendMessage("HandleInput");
                    }

                    ally.SendMessage("Drag");
                }
                break;
        }
	}
}






