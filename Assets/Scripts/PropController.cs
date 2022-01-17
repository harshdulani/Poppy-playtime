using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PropController : MonoBehaviour
{
	public bool shouldExplode, hasBeenInteractedWith;
	[SerializeField] private float explosionForce;
	[SerializeField] private List<Rigidbody> rigidbodies;
	[SerializeField] private List<Collider> colliders;
	[SerializeField] private float magnitude;
	[SerializeField] private float rotationMagnitude;

	[SerializeField] private Transform trailParent; 
	[SerializeField] private GameObject explosion;
	[SerializeField] private AudioClip[] explosionFx;
	private AudioSource _source;
	private static int _explosionSoundCounter;
	[SerializeField, Range(0.1f, 1f)] private float shrinkSpeedMultiplier, explosionScale;

	private readonly List<Transform> _pieces = new List<Transform>();
	private Rigidbody _rb;
	private Collider _collider;
	private Vector3 _previousPerlin, _previousPerlinRot;

	private bool _inHitBox, _hasBeenPickedUp, _amDestroyed;
	private GameObject _trail;

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
		_source = GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (_inHitBox)
			PerlinNoise();
		
		if(_pieces.Count == 0) return;

		for(var i = 0; i < _pieces.Count; i++)
		{
			if (_pieces[i].localScale.x < 0.05f)
			{
				_pieces[i].gameObject.SetActive(false);
				_pieces.Remove(_pieces[i]);
				continue;
			}

			_pieces[i].localScale -= Vector3.one * (Time.deltaTime * shrinkSpeedMultiplier);
		}
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

			_pieces.Add(rigidbodies[i].transform);
		}
		GameEvents.only.InvokePropDestroy(transform);
		_amDestroyed = true;

		_source.PlayOneShot(explosionFx[_explosionSoundCounter++ % explosionFx.Length]);
		Vibration.Vibrate(25);
	}

	public void GetPunched(Vector3 direction, float punchForce)
	{
		_rb.isKinematic = false;
		_rb.AddForce(direction * punchForce, ForceMode.Impulse);
	}
	
	public void AddTrail(GameObject trailPrefab)
	{
		_trail = Instantiate(trailPrefab, transform.position, transform.rotation);
		_trail.transform.parent = trailParent ? trailParent : transform;
	}
	
	public void DropProp()
	{
		_rb.isKinematic = false;
		transform.parent = null;
		_trail.SetActive(false);
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
	
	private void OnCollisionEnter(Collision other)
	{
		if(CompareTag("EnemyAttack"))
		{
			if (!(other.gameObject.CompareTag("HitBox") || other.gameObject.CompareTag("Arm") ||
				  other.gameObject.CompareTag("Player"))) return;

			GameEvents.only.InvokeEnemyHitPlayer(transform);

			var exploder = Instantiate(explosion, other.contacts[0].point,
				Quaternion.LookRotation(other.contacts[0].normal));

			exploder.transform.localScale *= explosionScale;
			
			Destroy(exploder, 3f);
			transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => gameObject.SetActive(false));
			return;
		}
		
		if (!other.collider.CompareTag("Target") && !other.collider.CompareTag("Ground")) return;
		
		if(shouldExplode)
			Invoke(nameof(Explode), .2f);

		if(!hasBeenInteractedWith) return;
		
		if (!other.transform.root.CompareTag("Target")) return;

		if(other.transform.root.TryGetComponent(out RagdollController raghu))
			raghu.GoRagdoll((other.contacts[0].point - transform.position).normalized);
	}
}
