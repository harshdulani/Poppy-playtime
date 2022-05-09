public sealed class DragToSmashState : InputStateBase
{
	private static AimController _aimer;
	
	public DragToSmashState() {}
	public DragToSmashState(AimController aimer)
	{
		_aimer = aimer;
	}
	
	public override void OnEnter()
	{
		IsPersistent = true;
		_aimer.CalculateTargetDistance();

		//AimController.SendTargetDown(1.5f);
		//hand controller dot tween grabbed target downwards 
	}

	public override void Execute()
	{
		base.Execute();

		if(InputExtensions.GetFingerUp())
		{
			InputHandler.Only.GetLeftHand().TryGivePunch();
			
			return;
		}
		
		if(!InputExtensions.GetFingerHeld()) return;
		
		_aimer.AimWithTargetHeld(InputExtensions.GetInputDelta());
	}

	public override void OnExit()
	{
		base.OnExit();
		//AimController.BringTargetBackUp(1.5f);
	}
}