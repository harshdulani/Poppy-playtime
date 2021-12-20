using UnityEngine;

public class InTransitState : InputStateBase
{
	private readonly RaycastHit _hit;
	public readonly bool GoHome, IsCarryingBody;

	public InTransitState(bool goHome, RaycastHit hitInfo, bool isCarryingBody = false)
	{
		GoHome = goHome;
		IsCarryingBody = isCarryingBody;
		
		_hit = hitInfo;
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
			LeftHand.MoveRopeEndTowards(_hit);
	}

	public override void OnExit()
	{
		base.OnExit();
	}
}