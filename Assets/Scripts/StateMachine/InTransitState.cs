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
		IsPersistent = InputHandler.Only.isUsingTapAndPunch;
		if (GoHome)
			PlayerSoundController.only.ZiplineCome();
		else
			PlayerSoundController.only.ZiplineGo();
	}

	public override void Execute()
	{
		base.Execute();
		
		if(GoHome)
			LeftHand.MoveRopeEndTowards(EmptyHit, true);
		else if(!_hit.collider) //if empty hit
			InputHandler.Only.AssignReturnTransitState();
		else
			LeftHand.MoveRopeEndTowards(_hit);

		HandController.UpdateRope();
	}

	public override void OnExit()
	{
		base.OnExit();
	}
}