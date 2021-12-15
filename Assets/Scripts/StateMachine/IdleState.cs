public class IdleState : InputStateBase
{
	public IdleState() { }

	public override void OnEnter()
	{
		IsPersistent = false;
	}

	public override void Execute()
	{
		//base.Execute();
	}

	public override void OnExit()
	{
		base.OnExit();
	}
}

public class DisabledState : InputStateBase
{
	public DisabledState() { }
	
	public override void OnEnter()
	{
		IsPersistent = true;
	}

	public override void Execute()
	{
		//base.Execute();
	}
	
	public override void OnExit()
	{
		base.OnExit();
	}
}