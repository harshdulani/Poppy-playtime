using UnityEngine;

public class InTransitState : InputStateBase
{
	private RaycastHit _hit;
	public readonly bool GoHome;

	public InTransitState(bool goHome, RaycastHit hit)
	{
		GoHome = goHome;
		_hit = hit;
	}
	
	public override void OnEnter()
	{
		IsPersistent = false;
	}

	public override void Execute()
	{
		base.Execute();
		
		if(GoHome)
			LeftHand.MoveRopeEndTowards(Vector3.zero, Vector3.zero, true);
		else
			LeftHand.MoveRopeEndTowards(_hit.point, _hit.normal);
	}
}