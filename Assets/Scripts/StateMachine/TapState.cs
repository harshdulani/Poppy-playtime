using UnityEngine;

public class TapState : InputStateBase
{
	private static AimController _aimer;
	private RaycastHit _hit;
	private static bool _hasTarget;
	private static float _screenPercentageOnY;
	
	public TapState(AimController aimer)
	{
		_aimer = aimer;
		_screenPercentageOnY = _aimer.screenPercentageOnY;
	}
	
	public override void OnEnter()
	{
		IsPersistent = false;
		_aimer.SetReticleStatus(false);
	}
	
	public override void Execute()
	{
		base.Execute();
		
		_aimer.SetReticleStatus(false);
		
		var ray = Cam.ScreenPointToRay(InputExtensions.GetInputPosition());

		if (!Physics.Raycast(ray, out var hit))
		{
			InputHandler.AssignNewState(InputHandler.IdleState, false);
			return;
		}
		if (!hit.collider.CompareTag("Target"))
		{
			InputHandler.AssignNewState(InputHandler.IdleState, false);
			return;
		}
		if (hit.collider.transform.root.TryGetComponent(out RagdollController raghu))
			if (raghu.isRagdoll)
			{
				InputHandler.AssignNewState(InputHandler.IdleState, false);
				return;
			}
		if (hit.collider.TryGetComponent(out EnemyPatroller patrol))
			if (!patrol.IsInCurrentPatrolArea())
				return;

		if (hit.collider.TryGetComponent(out ShatterEnemyController shat))
			if (!shat.IsInCurrentArea())
				return;

		raghu.PopScale();
		InputHandler.AssignNewState(new InTransitState(false, hit), false);
	}

	public override void OnExit()
	{
		base.OnExit();
	}
}