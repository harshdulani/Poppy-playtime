using UnityEngine;

public class RagdollController : MonoBehaviour
{
	public Rigidbody chest;
	[SerializeField] private Rigidbody[] rigidbodies;
	private Animator _anim;
	
	private static readonly int IsFlying = Animator.StringToHash("isFlying");

	private void Start()
	{
		_anim = GetComponent<Animator>();
	}

	public void HoldInAir()
	{
		_anim.SetBool(IsFlying, true);
		foreach (var rb in rigidbodies)
			rb.isKinematic = false;
	}

	public void GoRagdoll()
	{
		_anim.enabled = false;

		foreach (var rb in rigidbodies)
		{
			rb.isKinematic = false;
			//rb.useGravity = false;
		}
	}
}
