using UnityEngine;

public class VaultTrap : SerializableITrap
{
	[SerializeField] private Rigidbody vaultRigidbody;

	public override void TrapBehaviour() => vaultRigidbody.isKinematic = false;

	public override float GetTweenDuration() => 0;
}