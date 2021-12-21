using UnityEngine;

public class RagdollController : MonoBehaviour
{
	public Rigidbody chest;
	[SerializeField] private Rigidbody[] rigidbodies;
	private Animator _anim;
	public bool isRagdoll;
	
	private static readonly int IsFlying = Animator.StringToHash("isFlying");

	private void Start()
	{
		_anim = GetComponent<Animator>();
	}

	public void HoldInAir()
	{
		_anim.SetBool(IsFlying, true);
		foreach (var rb in rigidbodies)
			rb.isKinematic = true;
	}

	public void GoRagdoll(Vector3 direction)
	{
		if(isRagdoll) return;
		
		_anim.enabled = false;
		isRagdoll = true;

		foreach (var rb in rigidbodies)
		{
			rb.isKinematic = false;
			rb.AddForce(direction * 10f, ForceMode.Impulse);
		}

		GameEvents.only.InvokeEnemyKill();

		foreach (var rb in rigidbodies)
			rb.tag = "Untagged";
	}
}
