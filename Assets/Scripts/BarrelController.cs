using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelController : MonoBehaviour
{	
	[SerializeField] private float explosionForce;
	[SerializeField] private List<Rigidbody> rigidbodies;
	[SerializeField] private List<Collider> colliders;
	[SerializeField] private float magnitude;
	[SerializeField] private float rotationMagnitude;

	private Rigidbody _rb;
	private Collider _collider;
	private Vector3 _previousPerlin, _previousPerlinRot;
	private bool _inHitBox, _hasBeenPickedUp;

	private void OnEnable()
	{
		GameEvents.only.enterHitBox += OnEnterHitBox;
		GameEvents.only.punchHit += OnPunchHit;
	}

	private void OnDisable()
	{
		GameEvents.only.enterHitBox -= OnEnterHitBox;
		GameEvents.only.punchHit -= OnPunchHit;
	}
	
	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
		_collider = GetComponent<Collider>();
	}

	private void Update()
	{
		if(_inHitBox)
			PerlinNoise();
	}
	
	private void PerlinNoise()
	{
		var perlinY = Mathf.PerlinNoise(0f, Time.time);
		
		var perlin = Vector3.up * perlinY;
		var perlinRot = Vector3.up * perlinY;
		
		transform.position += (perlin - _previousPerlin) * magnitude;
		transform.rotation *= Quaternion.Euler((perlinRot - _previousPerlinRot) * rotationMagnitude);
		
		_previousPerlin = perlin;
		_previousPerlinRot = perlinRot;
	}

	private void OnCollisionEnter(Collision other)
	{
		Invoke(nameof(Explode), .2f);
		
		if(!other.transform.root.CompareTag("Target")) return;
		
		other.transform.root.GetComponent<RagdollController>().GoRagdoll((other.contacts[0].point - transform.position).normalized);
	}

	public void Explode()
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

	private void OnEnterHitBox(Transform target)
	{
		if(target != transform) return;

		_inHitBox = true;
	}

	private void OnPunchHit()
	{
		if(!_inHitBox) return;

		_inHitBox = true;
	}
}
