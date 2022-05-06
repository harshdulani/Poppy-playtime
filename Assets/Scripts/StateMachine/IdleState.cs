public class IdleState : InputStateBase
{
	public IdleState() { }

	public override void OnEnter()
	{
		IsPersistent = false;
		HandController.UpdateRope();
	}
}