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
			LeftHand.MoveRopeEndTowards(EmptyHit, true);
		else
			LeftHand.MoveRopeEndTowards(Hit);
	}

	public override void OnExit()
	{
		base.OnExit();
	}
}