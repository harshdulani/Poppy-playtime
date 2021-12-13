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
		//if catching
		//move rope end to that point at speed
		//else
		//move rope end back to host at speed
		
		//rope end se juda hua hai palm humara
		//if collider says reached,
		//changestate to ontarget if isCatching, else idle
		
		if(GoHome)
			LeftHand.MoveRopeEndTowards(Vector3.zero, Vector3.zero, true);
		else
			LeftHand.MoveRopeEndTowards(_hit.point, _hit.normal);
	}
}