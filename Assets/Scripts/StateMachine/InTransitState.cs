using UnityEngine;

public class InTransitState : InputStateBase
{
	private RaycastHit _hit;
	private bool _isCatching;

	public InTransitState(bool isCatching, RaycastHit hit = default)
	{
		_isCatching = isCatching;
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
		
		LeftHand.MoveRopeEndTowards(_hit, !_isCatching);
	}
}