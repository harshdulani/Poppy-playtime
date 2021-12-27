using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PropController : MonoBehaviour
{
	public bool hasBeenInteractedWith;
	[SerializeField] private float explosionForce;
	[SerializeField] private List<Rigidbody> rigidbodies;
	[SerializeField] private List<Collider> colliders;
	[SerializeField] private float magnitude;
	[SerializeField] private float rotationMagnitude;

	private Rigidbody _rb;
	private Collider _collider;
	private Vector3 _previousPerlin, _previousPerlinRot;
	private bool _inHitBox, _hasBeenPickedUp, _amDestroyed;

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
		if (!other.collider.CompareTag("Target") && !other.collider.CompareTag("Ground")) return;
		
		Invoke(nameof(Explode), .2f);
		
		if(!hasBeenInteractedWith) return;
		
		if (!other.transform.root.CompareTag("Target")) return;
		
		if(other.transform.root.TryGetComponent(out RagdollController raghu))
			raghu.GoRagdoll((other.contacts[0].point - transform.position).normalized);
	}

	public void Explode()
	{
		if(_amDestroyed) return;
		
		_collider.enabled = false;
		var parent = new GameObject(gameObject.name + " debris").transform;
		
		for(var i = 0; i < rigidbodies.Count; i++)
		{
			colliders[i].enabled = true;
			rigidbodies[i].transform.parent = parent;
			rigidbodies[i].isKinematic = false;
			rigidbodies[i].AddExplosionForce(explosionForce, transform.position, 5f);
		}
		GameEvents.only.InvokePropDestroy(transform);
		_amDestroyed = true;
	}

	public void GetPunched(Vector3 direction, float punchForce)
	{
		_rb.isKinematic = false;
		_rb.AddForce(direction * punchForce, ForceMode.Impulse);
	}

	private IEnumerator DisablePiece(GameObject piece, float time)
	{
		yield return GameExtensions.GetWaiter(time);
		piece.transform.DOScale(Vector3.zero, 1f).OnComplete(() => piece.SetActive(false));
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
