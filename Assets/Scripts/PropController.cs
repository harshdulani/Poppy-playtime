using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PropController : MonoBehaviour, IWantsAds
{
	public bool shouldShowAds, shouldExplode, hasBeenInteractedWith;
	[SerializeField] private float explosionForce;
	[SerializeField] private List<Rigidbody> rigidbodies;
	[SerializeField] private List<Collider> colliders;
	[SerializeField] private float magnitude;
	[SerializeField] private float rotationMagnitude;

	[SerializeField] private Transform trailParent;
	[SerializeField] private GameObject explosion;
	[SerializeField] private AudioClip[] explosionFx;
	[SerializeField] private bool alwaysSpawnVfx;
	[SerializeField, Range(0.1f, 1f)] private float shrinkSpeedMultiplier, explosionScale;
	[SerializeField] private Transform adButton;

	private Tweener _adScaleTween;
	private static int _explosionSoundCounter;
	private readonly List<Transform> _pieces = new List<Transform>();

	private bool _inHitBox, _amDestroyed, _isHeldByPlayer;
	private GameObject _trail;
	
	private AudioSource _source;
	private CompositeProp _parent;
	private Rigidbody _rb;
	private Collider _collider;
	private Vector3 _previousPerlin, _previousPerlinRot;

	private void OnEnable()
	{
		GameEvents.only.enterHitBox += OnEnterHitBox;
		GameEvents.only.punchHit += OnPunchHit;
		GameEvents.only.gameEnd += OnGameEnd;
	}

	private void OnDisable()
	{
		GameEvents.only.enterHitBox -= OnEnterHitBox;
		GameEvents.only.punchHit -= OnPunchHit;
		GameEvents.only.gameEnd -= OnGameEnd;
	}

	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
		_collider = GetComponent<Collider>();
		_source = GetComponent<AudioSource>();
		if (transform.parent)
			transform.parent.TryGetComponent(out _parent);

		if(!adButton) return;
		
		var initScale = adButton.localScale;
		_adScaleTween = adButton.DOScale(initScale * 1.25f, 0.5f).SetLoops(-1, LoopType.Yoyo);
	}

	private void Update()
	{
		if (_inHitBox)
			PerlinNoise();

		if (_pieces.Count == 0) return;

		for (var i = 0; i < _pieces.Count; i++)
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

	public void PlayerPicksUp()
	{
		_isHeldByPlayer = true;
		
		if(!_adScaleTween.IsActive()) return;

		adButton.gameObject.SetActive(false);
		_adScaleTween.Kill();
	}

	public void PlayerDrops() => _isHeldByPlayer = false;

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
		if (_amDestroyed) return;

		_collider.enabled = false;
		var parent = new GameObject(gameObject.name + " debris").transform;

		if(colliders.Count == 0 || !colliders[0])
			print($"{gameObject} has no/null colliders");
		else
			for (var i = 0; i < rigidbodies.Count; i++)
			{
				colliders[i].enabled = true;
				rigidbodies[i].transform.parent = parent;
				rigidbodies[i].isKinematic = false;
				rigidbodies[i].AddExplosionForce(explosionForce, transform.position, 10f);

				_pieces.Add(rigidbodies[i].transform);
			}

		if(alwaysSpawnVfx)
			CreateVFX(transform.position, Quaternion.identity);
		
		GameEvents.only.InvokePropDestroy(transform);
		_amDestroyed = true;
		_isHeldByPlayer = false;

		if (explosionFx.Length > 0)
			_source.PlayOneShot(explosionFx[_explosionSoundCounter++ % explosionFx.Length]);
		Vibration.Vibrate(25);
	}

	public void GetPunched(Vector3 direction, float punchForce)
	{
		_rb.isKinematic = false;
		_rb.AddForce(direction * punchForce, ForceMode.Impulse);
		_isHeldByPlayer = false;
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
		if (_trail)
			_trail.SetActive(false);
	}

	public bool IsACompositeProp => _parent;

	public void GetTouchedComposite(Vector3 direction, bool collapseMe) =>
		_parent.StopBeingKinematic(direction, collapseMe ? null : transform);

	public void Collapse(Vector3 direction)
	{
		_rb.isKinematic = false;
		transform.parent = null;
		//_rb.AddForce(direction, ForceMode.Impulse);
	}

	public void MakeKinematic()
	{
		_rb.isKinematic = true;
	}

	public void TryShowAds()
	{
		if (!shouldShowAds) return;

		TimeController.only.SlowDownTime(0f);

		if(!ApplovinManager.instance) return;
		
		StartWaiting();
		AdsMediator.StartListeningForAds(this);
		ApplovinManager.instance.ShowRewardedAds();
	}

	private void CreateVFX(Vector3 position, Quaternion rotation)
	{
		var exploder = Instantiate(explosion, position, rotation);

		exploder.transform.localScale *= explosionScale;

		Destroy(exploder, 3f);
	}

	private void OnCollisionEnter(Collision other)
	{
		//behaviour when thrown by giant at player
		if(CompareTag("EnemyAttack"))
		{
			if (!(other.gameObject.CompareTag("HitBox") || other.gameObject.CompareTag("Arm") ||
				  other.gameObject.CompareTag("Player"))) return;

			GameEvents.only.InvokeEnemyHitPlayer(transform);

			if(explosion)
			{
				CreateVFX(other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal));
			}
			transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => gameObject.SetActive(false));
			return;
		}
		
		if(other.collider.CompareTag("ChainLink"))
		{
			Explode();
			CreateVFX(other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal));
		}
		
		if (!other.collider.CompareTag("Target") && !other.collider.CompareTag("Ground")) return;
		
		if(shouldExplode)
			Invoke(nameof(Explode), LevelFlowController.only.IsThisLastEnemyOfArea() ? 0f : 0.2f);

		if (other.transform.TryGetComponent(out PropController prop))
		{
			if (prop.IsACompositeProp)
				prop.GetTouchedComposite(Vector3.up, true);
			else
				prop.DropProp();
			
			return;
		}
		
		if(!hasBeenInteractedWith) return;
		
		if (!other.transform.root.CompareTag("Target")) return;

		if (other.transform.root.TryGetComponent(out RagdollController raghu))
		{
			raghu.TryGoRagdoll((other.contacts[0].point - transform.position).normalized);

			if (shouldShowAds) 
				raghu.TryGoRagdoll(Vector3.zero, true);
		}

		if (raghu || other.transform.TryGetComponent(out HelicopterController _))
			GameEvents.only.InvokePropHitsEnemy();
	}

	private void OnEnterHitBox(Transform target)
	{
		if(target != transform) return;

		_inHitBox = true;
	}

	private void OnPunchHit()
	{
		if(!_inHitBox) return;

		_inHitBox = false;
	}

	private void OnGameEnd()
	{
		if(!_isHeldByPlayer) return;

		Explode();
	}
	
	private void StartWaiting()
	{
		InputHandler.Only.userIsWatchingAnAdForPickupProp = true;
	}

	private void StopWaiting()
	{
		InputHandler.Only.userIsWatchingAnAdForPickupProp = false;
	}

	public void OnAdRewardReceived(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
	{
		TimeController.only.RevertTime();
		StopWaiting();
		AdsMediator.StopListeningForAds(this);
	}

	public void OnAdFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
	{
		Explode();
		TimeController.only.RevertTime();
		
		StopWaiting();
		AdsMediator.StopListeningForAds(this);
	}

	public void OnAdFailedToLoad(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
	{
		Explode();
		TimeController.only.RevertTime();
		
		StopWaiting();
		AdsMediator.StopListeningForAds(this);
	}

	public void OnAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		Explode();
		TimeController.only.RevertTime();
		
		StopWaiting();
		AdsMediator.StopListeningForAds(this);
	}
}
