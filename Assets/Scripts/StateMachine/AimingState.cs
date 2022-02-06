using UnityEngine;

public class AimingState : InputStateBase
{
	private static AimController _aimer;
	private RaycastHit _hit;
	private static bool _hasTarget;
	private static float _screenPercentageOnY;

	public AimingState(AimController aimer)
	{
		_aimer = aimer;
		_screenPercentageOnY = _aimer.screenPercentageOnY;
	}
	
	public override void OnEnter()
	{
		IsPersistent = false;
		_aimer.SetReticleStatus(true);
	}

	public override void Execute()
	{
		base.Execute();
		
		_aimer.Aim(InputExtensions.GetInputDelta());

		//the magic number percentage 0.5905f is the screen Y pos when you center the crosshair on anchorY as minY = 0.565, maxY = 0.615
		var ray = Cam.ScreenPointToRay(InputExtensions.GetCenterOfScreen(_screenPercentageOnY));
		
		if (!Physics.Raycast(ray, out _hit, RaycastDistance)) 
		{
			_aimer.LoseTarget(); //if raycast didn't hit anything
			return;
		}
		if (!_hit.collider.CompareTag("Target") && !_hit.collider.CompareTag("Button"))
		{
			_aimer.LoseTarget();
			return;
		}
		if (_hit.collider.TryGetComponent(out EnemyPatroller patrol))
		{
			if (!patrol.IsInCurrentPatrolArea())
			{
				_aimer.LoseTarget();
				return;
			}
		}

		if (_hit.collider.TryGetComponent(out ShatterEnemyController shat))
		{
			if (!shat.IsInCurrentArea())
			{
				_aimer.LoseTarget();
				return;
			}
		}
		
		_aimer.FindTarget();
	}

	public override void OnExit()
	{
		base.OnExit();
		_aimer.SetReticleStatus(false);
		
		var ray = Cam.ScreenPointToRay(InputExtensions.GetCenterOfScreen(_screenPercentageOnY));
		if (!Physics.Raycast(ray, out var hit, RaycastDistance))
		{
			InputHandler.AssignNewState(InputHandler.IdleState, false);
			return;
		}
		if (!hit.collider.CompareTag("Target") && !_hit.collider.CompareTag("Button"))
		{
			InputHandler.AssignNewState(InputHandler.IdleState, false);
			return;
		}
		if (hit.collider.transform.root.TryGetComponent(out RagdollController rag))
			if (rag.isRagdoll)
			{
				InputHandler.AssignNewState(InputHandler.IdleState, false);
				return;
			}
		
		InputHandler.AssignNewState(new InTransitState(false, hit), false);
	}
}