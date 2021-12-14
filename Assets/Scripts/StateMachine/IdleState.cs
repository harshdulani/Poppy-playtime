public class IdleState : InputStateBase
{
	public IdleState() { }

	public override void OnEnter()
	{
		IsPersistent = false;
		/*
		LeftHand.SetUpdateMode(Dreamteck.Splines.SplineComputer.UpdateMode.None);
		Print("set to none");*/
	}

	public override void Execute()
	{
		//base.Execute();
	}

	public override void OnExit()
	{
		base.OnExit();
		/*LeftHand.SetUpdateMode(Dreamteck.Splines.SplineComputer.UpdateMode.Update);
		Print("set to update");*/
	}
}

public class DisabledState : InputStateBase
{
	public DisabledState() { }
	
	public override void OnEnter()
	{
		IsPersistent = true;
		/*
		LeftHand.SetUpdateMode(Dreamteck.Splines.SplineComputer.UpdateMode.None);
		Print("set to none");*/
	}

	public override void Execute()
	{
		//base.Execute();
	}
	
	public override void OnExit()
	{
		base.OnExit();
		/*LeftHand.SetUpdateMode(Dreamteck.Splines.SplineComputer.UpdateMode.Update);
		Print("set to update");*/
	}
}