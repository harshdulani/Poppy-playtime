using UnityEngine;

public class RagdollController : MonoBehaviour
{
	public Rigidbody chest;
	[SerializeField] private Rigidbody[] rigidbodies;
	private Animator _anim;
	private bool _isRagdoll;
	
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

	public void GoRagdoll()
	{
		if(_isRagdoll) return;
		
		_anim.enabled = false;
		_isRagdoll = true;

		foreach (var rb in rigidbodies)
			rb.isKinematic = false;
	}
}
