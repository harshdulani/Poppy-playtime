using UnityEngine;

public class RagdollController : MonoBehaviour
{
	[SerializeField] private Rigidbody[] rigidbodies;
	private Animator _anim;

	private void Start()
	{
		_anim = GetComponent<Animator>();
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

	public void StartGettingCarried()
	{
		foreach (var rb in rigidbodies)
			rb.useGravity = false;
	}
}
