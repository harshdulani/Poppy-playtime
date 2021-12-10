using UnityEngine;
 
public static class InputExtensions
{
	//make sure you set these somewhere on first touch
	public static bool IsUsingTouch; 
	/// <summary>
	/// Higher the value, lesser the touch delta
	/// </summary>
	public static float TouchInputDivisor;

	private static Vector3 previousViewportPos;
	
	/// <summary>
	/// Returns Screen position co-ordinates. Ignorant to where the input is coming from.  
	/// </summary>
	/// <returns></returns>
	public static Vector2 GetInputPosition ()
	{
		if (!GetFingerHeld() && !GetFingerDown()) return Vector2.zero;

		if (IsUsingTouch)
			return Input.GetTouch(0).position;
		
		return Input.mousePosition;
	}
	
	/// <summary>
	/// Returns change in Screen position co-ordinates. Ignorant to where the input is coming from.
	/// </summary>
	/// <returns></returns>
	public static Vector2 GetInputDelta ()
	{
		if (!GetFingerHeld()) return Vector2.zero;

		if (IsUsingTouch)
			return Input.GetTouch(0).deltaPosition / TouchInputDivisor;
		
		return new Vector2( Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
	}

	/// <summary>
	/// Returns if input is just pressed this frame.
	/// </summary>
	/// <returns></returns>
	public static bool GetFingerDown ()
	{
		if (!IsUsingTouch) return Input.GetMouseButtonDown(0);
		
		if (Input.touchCount == 0) return false;

		return Input.GetTouch(0).phase == TouchPhase.Began;

	}

	/// <summary>
	/// Returns if input is just released this frame. Ignorant to where the input is coming from.
	/// </summary>
	/// <returns></returns>
	public static bool GetFingerUp ()
	{
		if (!IsUsingTouch) return Input.GetMouseButtonUp(0);
		
		if (Input.touchCount == 0) return false;
			
		return Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled;

	}
	
	/// <summary>
	/// Returns if input has been pressed since previous frame. Ignorant to where the input is coming from.
	/// </summary>
	/// <returns></returns>
	public static bool GetFingerHeld ()
	{
		if (!IsUsingTouch) return Input.GetMouseButton(0);
		
		if (Input.touchCount == 0) return false;
		
		return Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary;
	}
}
