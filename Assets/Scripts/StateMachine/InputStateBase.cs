using UnityEngine;

public class InputStateBase
{
	public static bool IsPersistent;
	protected static HandController LeftHand;
	protected static Camera Cam;
	protected static float RaycastDistance;

	public static RaycastHit EmptyHit = new RaycastHit();
	private static float _decreaseMultiplier;

	protected InputStateBase() { }

	public InputStateBase(HandController leftHand, Camera cam, float raycastDistance)
	{
		LeftHand = leftHand;
		Cam = cam;
		RaycastDistance = raycastDistance;
		
		EmptyHit.point = Vector3.negativeInfinity;
	}

	public virtual void OnEnter() { }

	public virtual void Execute() { }

	public virtual void FixedExecute() {}
	
	public virtual void OnExit() {}

	protected static void Print(object message)
	{
		Debug.Log(message);
	}
}