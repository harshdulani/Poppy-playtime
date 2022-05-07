using UnityEngine;
using DG.Tweening;

public class InTransitState : InputStateBase
{
	private readonly RaycastHit _hit;
	public readonly bool GoHome, IsCarryingBody;
	
	//Player cannot be in Transit forever
	private Tween _timer;
	private const float MaxTransitTime = 2f;

	public InTransitState(bool goHome, RaycastHit hitInfo, bool isCarryingBody = false)
	{
		GoHome = goHome;
		IsCarryingBody = isCarryingBody;
		
		_hit = hitInfo;
	}
	
	public override void OnEnter()
	{
		IsPersistent = InputHandler.Only.isUsingTapAndPunch;
		_timer = DOVirtual.DelayedCall(MaxTransitTime, () => InputHandler.AssignNewState(InputHandler.IdleState));
		
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
		else
			LeftHand.MoveRopeEndTowards(_hit);

		HandController.UpdateRope();
	}

	public override void OnExit()
	{
		base.OnExit();
		
		if(_timer.IsActive()) _timer.Kill();
	}
}