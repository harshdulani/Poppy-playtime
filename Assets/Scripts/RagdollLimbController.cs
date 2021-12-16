using UnityEngine;

public class RagdollLimbController : MonoBehaviour
{
	private RagdollController _parent;

	private Rigidbody _rb;
	
	private void Start()
	{
		_parent = transform.root.GetComponent<RagdollController>();
		_rb = GetComponent<Rigidbody>();
	}

	public void TellParent()
	{
		_parent.HoldInAir();
		//_parent.GoRagdoll();
	}
	
	public void GetPunched(Vector3 direction, float punchForce)
	{
		print("ouch");
		_parent.GoRagdoll();
		_rb.AddForce(Vector3.forward * punchForce, ForceMode.Impulse);
	}

	public Rigidbody AskParentForHook() => _parent.chest;
	
}
