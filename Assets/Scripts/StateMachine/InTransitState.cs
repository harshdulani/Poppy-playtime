using UnityEngine;
using DG.Tweening;

public class InTransitState : InputStateBase
{
	private readonly RaycastHit _hit;
	public readonly bool GoHome, IsCarryingBody;
	
	//Player cannot be in Transit forever
	private Tween _timer;
	private const float MaxTransitTime = 1f;

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
		if(LevelFlowController.only.isGiantLevel)
			if (HandController.PropHeldToPunch && HandController.PropHeldToPunch.isCar)
			{
				var tut = Object.FindObjectOfType<TutorialCanvasController>();
				if (tut) tut.PlayerKnowsHowToPickUpCars();
			}

		_timer = DOVirtual.DelayedCall(MaxTransitTime , () =>
		{
			if(GoHome)
			{
				HandController.ForceRopeToReturnHome();
				//InputHandler.Only.GetLeftHand().HandReachHome();
			}
			else
			{
				var tween = InputHandler.Only.GetLeftHand().PalmController.ReachPointInstantly(_hit.point);
				tween.OnComplete(() =>
				{
					if (!_timer.IsActive()) return;
					
					InputHandler.Only.GetLeftHand().HandReachTarget(_hit.transform);
					InputHandler.Only.GetLeftHand().PalmController.EnableAdoptability();
				});
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