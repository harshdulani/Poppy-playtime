using UnityEngine;

public class AimingState : InputStateBase
{
	private static AimController _aimer;
	private RaycastHit _hit;
	private static bool _hasTarget;

	public AimingState(AimController aimer)
	{
		_aimer = aimer;
	}

	//on tap/hold, you enter aiming state
	
	//onexecute, rotate camera + hands around Y axis, reticle color changes according to raycast hit
	//on exit,check the bool holding is hitting raycast
	//if yes, execute onexit
	//assign new state intransit state
	//else inputhandler will autoassign idle state

	public override void OnEnter()
	{
		IsPersistent = false;
		_aimer.SetReticleStatus(true);
	}

	public override void Execute()
	{
		base.Execute();
		
		_aimer.Aim(InputExtensions.GetInputDelta());

		var ray = Cam.ScreenPointToRay(InputExtensions.GetCenterOfScreen());
		
		if (!Physics.Raycast(ray, out _hit)) 
		{
			_aimer.LoseTarget(); //if raycast didn't hit anything
			return;
		}
		if (!_hit.collider.CompareTag("Target"))
		{
			_aimer.LoseTarget();
			return;
		}
		
		_aimer.FindTarget();
	}

	public override void OnExit()
	{
		base.OnExit();
		_aimer.SetReticleStatus(false);
		
		var ray = Cam.ScreenPointToRay(InputExtensions.GetCenterOfScreen());
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
		if (hit.collider.transform.root.TryGetComponent(out RagdollController rag))
			if (rag.isRagdoll)
			{
				InputHandler.AssignNewState(InputHandler.IdleState, false);
				return;
			}
		
		InputHandler.Only.SetCurrentTransform(hit.transform);
		InputHandler.AssignNewState(new InTransitState(false, hit), false);
	}
}