using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelController : MonoBehaviour
{
	[SerializeField] private List<Rigidbody> rigidbodies;
	[SerializeField] private List<Collider> colliders;
	[SerializeField] private float explosionForce;
	
	private Rigidbody _rb;
	private Collider _collider;
	
	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
		_collider = GetComponent<Collider>();
	}

	private void OnCollisionEnter(Collision other)
	{
		Explode();
		
		if(!other.collider.CompareTag("Target")) return;
	}

	private void Explode()
	{
		_collider.enabled = false;
		for(int i = 0; i < rigidbodies.Count; i++)
		{
			colliders[i].enabled = true;
			rigidbodies[i].transform.parent = null;
			rigidbodies[i].isKinematic = false;
			rigidbodies[i].AddExplosionForce(explosionForce, transform.position, 5f);
		}
	}

	public void GetPunched(Vector3 direction, float punchForce)
	{
		_rb.isKinematic = false;
		//print(direction);
		_rb.AddForce(direction * punchForce, ForceMode.Impulse);
	}

	private IEnumerator DisablePiece(GameObject piece, float time)
	{
		yield return GameExtensions.GetWaiter(time);
		
		piece.SetActive(false);
	}
}
