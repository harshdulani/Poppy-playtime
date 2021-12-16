using UnityEngine;

public class InTransitState : InputStateBase
{
	public RaycastHit Hit;
	public readonly bool GoHome, IsCarryingBody;
	private readonly bool _isLeftHand;

	public InTransitState(bool goHome, RaycastHit hitInfo, bool isLeftHand = true, bool isCarryingBody = false)
	{
		GoHome = goHome;
		IsCarryingBody = isCarryingBody;
		_isLeftHand = isLeftHand;
		
		Hit = hitInfo;
	}
	
	public override void OnEnter()
	{
		IsPersistent = false;
	}

	public override void Execute()
	{
		base.Execute();
		
		if(GoHome)
			(_isLeftHand ? LeftHand : RightHand).MoveRopeEndTowards(Vector3.zero, Vector3.zero, true);
		else
			(_isLeftHand ? LeftHand : RightHand).MoveRopeEndTowards(Hit.point, Hit.normal);
	}

	public override void OnExit()
	{
		base.OnExit();
	}
}

public class WaitingToPunchState : InputStateBase
{
	public static Transform Target;

	public WaitingToPunchState(Transform target)
	{
		Target = target;
	}

	public override void OnEnter()
	{
		base.OnEnter();
		IsPersistent = true;
	}
}