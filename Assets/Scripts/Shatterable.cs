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
		if(!other.transform.root.CompareTag("Target")) return;
		
		if(!ShatterableParent.IsThisAPossibleShatterer(other.transform.root)) return;

		_parent.isShattered = true;
		_parent.ShatterTheShatterables(other.contacts[0].point);
	}

	public void Shatter(Vector3 point, float explosionForce, float explosionRadius)
	{
		if (_parent.shouldUnparent)
			transform.parent = null;
		_rb.isKinematic = false;
		_rb.useGravity = true;

		if (point == Vector3.negativeInfinity) return;
		
		_rb.AddExplosionForce(explosionForce, point, explosionRadius, 1f, ForceMode.Impulse);
	}

	public ShatterableParent GetShatterableParent() => _parent;
}