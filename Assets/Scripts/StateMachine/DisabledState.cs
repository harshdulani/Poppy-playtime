public class DisabledState : InputStateBase
{
	public readonly bool IsTemporary;
	private static HandController _rightHand;

	public DisabledState(HandController rightHand) => _rightHand = rightHand;

	public DisabledState(bool isTemporary) => IsTemporary = isTemporary;

	public override void OnEnter()
	{
		IsPersistent = true;
	}

	public override void Execute()
	{
		if(!IsTemporary) return;
		
		if(!InputExtensions.GetFingerDown()) return;
		if(!_rightHand.TryGivePunch()) return;
			
		InputHandler.Only.PutInTapCoolDown();
	}
}