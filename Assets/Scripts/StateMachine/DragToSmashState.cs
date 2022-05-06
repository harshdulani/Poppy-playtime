using UnityEngine;

public sealed class DragToSmashState : InputStateBase
{
	private static AimController _aimer;
	private RaycastHit _hit;
	private static bool _hasTarget;
	private static float _screenPercentageOnY;
	
	public DragToSmashState(AimController aimer)
	{
		_aimer = aimer;
		_screenPercentageOnY = _aimer.screenPercentageOnY;
	}
	
	public override void OnEnter()
	{
		IsPersistent = false;
		_aimer.CalculateTargetDistance();
		//hand controller dot tween grabbed target downwards 
	}

	public override void Execute()
	{
		base.Execute();
		
		_aimer.AimWithTargetHeld(InputExtensions.GetInputDelta());
	}

	public override void OnExit()
	{
		base.OnExit();
		_aimer.SetReticleStatus(false);
		
		
	}
}