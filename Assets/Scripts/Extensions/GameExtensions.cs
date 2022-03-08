using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class GameExtensions
{
	private static readonly Dictionary<float, WaitForSeconds> WaitForSecondsMap = new Dictionary<float, WaitForSeconds>();

	public static WaitForSeconds GetWaiter(float time)
	{
		if (WaitForSecondsMap.ContainsKey(time))
			return WaitForSecondsMap[time];

		var waiter = new WaitForSeconds(time);
		WaitForSecondsMap.Add(time, waiter);
		return waiter;
	}

	public static float LerpUnclamped(float a, float b, float t)
	{
		return (1f - t) * a + b * t;
	}

	public static float InverseLerpUnclamped(float a, float b, float v)
	{
		return (v - a) / (b - a);
	}
	
	public static Color RemapColor(float iMin, float iMax, Color oMin, Color oMax, float v)
	{
		return Color.Lerp(oMin, oMax, InverseLerpUnclamped(iMin, iMax, v));
	}

	public static float Remap(float iMin, float iMax, float oMin, float oMax, float v)
	{
		return Mathf.LerpUnclamped(oMin, oMax, InverseLerpUnclamped(iMin, iMax, v));
	}

	public static float RemapClamped(float iMin, float iMax, float oMin, float oMax, float v)
	{
		return Mathf.Lerp(oMin, oMax, Mathf.InverseLerp(iMin, iMax, v));
	}

	/// <summary>
	/// Use whenever you can't find out why a canvas element is not being pressed
	/// </summary>
	public static void GetObjectUnderPointer()
	{
		if (!Input.GetMouseButtonDown(0)) return;
		
		var pointerData = new PointerEventData(EventSystem.current) {pointerId = -1, position = Input.mousePosition};

		var results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerData, results);
			
		Print(results[0].gameObject);
	}
	
	private static void Print(object msg)
	{
		Debug.Log(msg);
	}
}
