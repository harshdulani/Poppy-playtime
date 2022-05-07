public class IdleState : InputStateBase
{
	public override void OnEnter()
	{
		IsPersistent = false;
		HandController.UpdateRope();
	}
}