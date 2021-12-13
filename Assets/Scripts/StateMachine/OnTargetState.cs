using UnityEngine;

public class OnTargetState : InputStateBase
{
	private Transform _target;
	
	public OnTargetState(Transform target)
	{
		_target = target;
	}

	public override void OnEnter()
	{
		IsPersistent = false;
	}

	public override void Execute()
	{
		base.Execute();
		//let player shake the enemy ragdoll around and if hit w enough force, surrounding ragdolls fall
	}

	public override void OnExit()
	{
		base.OnExit();
		
		_target = null;
	}
}