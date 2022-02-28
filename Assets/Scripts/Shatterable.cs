using UnityEngine;

public class Shatterable : MonoBehaviour
{
	private Rigidbody _rb;
	private ShatterableParent _parent;
	private bool _canShatter;
	
	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
		
		_parent = transform.parent.GetComponent<ShatterableParent>();
	}

	public void MakeShatterable()
	{
		_canShatter = true;
	}

	private void OnCollisionEnter(Collision other)
	{
		if(!_canShatter) return;
		if(_parent.isShattered) return;
		//if(!other.transform.root.CompareTag("Target")) return;
		
		if(!other.transform.root.TryGetComponent(out RagdollController _)) return;
		
		//maybe remove this, because the ones knocked out can also shatter the tower but theyre not in this list
		if(!ShatterableParent.IsThisAPossibleShatterer(other.transform.root)) return;

		_parent.isShattered = true;
		_parent.ShatterTheShatterables();
	}

	public void Shatter()
	{
		if (_parent.shouldUnparent)
			transform.parent = null;
		_rb.isKinematic = false;
		_rb.useGravity = true;
	}
}