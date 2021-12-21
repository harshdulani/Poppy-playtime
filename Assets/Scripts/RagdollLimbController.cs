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
	}
	
	public void GetPunched(Vector3 direction, float punchForce)
	{
		_parent.GoRagdoll(direction);
		_rb.AddForce(Vector3.forward * punchForce + Vector3.up * punchForce / 3, ForceMode.Impulse);
	}

	public Rigidbody AskParentForHook() => _parent.chest;

	private void OnCollisionEnter(Collision other)
	{
		if(!_parent.isRagdoll) return;
		
		if(other.transform.root == transform.root) return;
		if(!other.collider.CompareTag("Target")) return;

		if (other.gameObject.TryGetComponent(out RagdollLimbController raghu))
		{
			raghu.GetPunched(other.transform.position - transform.position, 0f);
		}
		else
		{
			other.gameObject.GetComponent<BarrelController>().Explode();
		}
	}
}
