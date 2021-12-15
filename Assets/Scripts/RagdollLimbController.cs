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
		_parent.GoRagdoll();
	}
	
	public void GetPunched(Vector3 direction, float punchForce)
	{
		_rb.AddForce(direction * punchForce, ForceMode.Impulse);
		
		//detach left hand from body
		//assign left hand state to no body go home
	}
}
