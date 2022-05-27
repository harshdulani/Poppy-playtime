public sealed class DragToSmashState : InputStateBase
{
	private static AimController _aimer;
	private bool _sentDown;
	
	public DragToSmashState() {}
	public DragToSmashState(AimController aimer)
	{
		_aimer = aimer;
	}
	
	public override void OnEnter()
	{
		IsPersistent = true;
		_sentDown = false;
		_aimer.CalculateTargetDistance();
	}

	public override void Execute()
	{
		base.Execute();

		if(InputExtensions.GetFingerUp())
		{
			InputHandler.Only.GetRightHand().TryGivePunch();
			_aimer.BringTargetBackUp();
			return;
		}
		
		if(!InputExtensions.GetFingerHeld()) return;
		
		if(!_sentDown)
		{
			_aimer.MoveTargetDown();
			_sentDown = true;
		}
		
		_aimer.AimWithTargetHeld(InputExtensions.GetInputDelta());
	}
}