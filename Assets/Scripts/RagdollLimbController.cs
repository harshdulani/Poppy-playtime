using UnityEngine;

public class RagdollLimbController : MonoBehaviour
{
	private RagdollController _parent;

	private void Start()
	{
		_parent = transform.root.GetComponent<RagdollController>();
	}

	public void TellParent()
	{
		_parent.GoRagdoll();
	}
}
