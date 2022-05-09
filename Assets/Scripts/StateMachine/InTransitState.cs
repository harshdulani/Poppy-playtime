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
		if (GoHome)
			PlayerSoundController.only.ZiplineCome();
		else
			PlayerSoundController.only.ZiplineGo();

		//to prevent guard boss slow mo bug
		if (HandController.PropHeldToPunch && HandController.PropHeldToPunch.isCar)
		{
			var tut = Object.FindObjectOfType<TutorialCanvasController>();
			if (tut)
			{
				tut.knowsHowToPickUpCars = true;
			}
		}

		_timer = DOVirtual.DelayedCall(MaxTransitTime, () =>
		{
			if(LevelFlowController.only.isGiantLevel) return;
			if(GoHome)
				//i was here, problem was: when player pulls car and slo mo starts after that
				//on clicking during this slow mo, everything breaks,
				//maybe set a flag to not go into slow mo in the first place if you already have picked a car up
				InputHandler.Only.GetLeftHand().HandReachHome();
			else
			{
				InputHandler.Only.GetLeftHand().HandReachTarget(_hit.transform);
				InputHandler.Only.GetLeftHand().PalmController.EnableAdoptability();
			}
		});
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