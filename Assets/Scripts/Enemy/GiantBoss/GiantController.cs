using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public struct BoxBounds { public float x, y, z, distance; }

public class GiantController : MonoBehaviour
{
	[HideInInspector] public bool isDead;
	[SerializeField] private Renderer[] rends;
	
	[SerializeField] private bool isRagdoll;
	[SerializeField] private Rigidbody[] rigidbodies;
	[SerializeField] private List<GameObject> headgear;

	[SerializeField] private bool showOverlapBoxDebug;
	[SerializeField] private BoxBounds overlapBoxBounds;
	[SerializeField] private Transform carHolderSlot;
	[Range(0.1f, 1f)] public float explosionScale = 1f;
	[SerializeField] private float throwForce, waitBetweenAttacks;

	[SerializeField] private float endYValue;
	[SerializeField] private GameObject trailPrefab;
	[SerializeField] private GameObject smokeEffectOnLand;
	[SerializeField] private AudioClip landing, roar, propPull;

	private Transform _player, _grabbedTargetTransform;
	private CarController _grabbedTargetCarController;
	private PropController _grabbedTargetPropController;

	private Animator _anim;
	private AudioSource _audioSource;
	private HealthController _health;

	private Tweener _tweener;
	private bool _isAttacking;
	
	private static readonly int Hit = Animator.StringToHash("Hit");
	private static readonly int Jump = Animator.StringToHash("Jump");
	private static readonly int Attack = Animator.StringToHash("Attack");
	private static readonly int Grab = Animator.StringToHash("Grab");

	private void OnEnable()
	{
		GameEvents.Only.ReachNextArea += OnReachNextArea;
	}

	private void OnDisable()
	{
		GameEvents.Only.ReachNextArea -= OnReachNextArea;
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_audioSource = GetComponent<AudioSource>();
		_health = GetComponent<HealthController>();

		foreach (var rend in rends)
			rend.enabled = false;

		foreach (var rb in rigidbodies) rb.isKinematic = true;

		_player = GameObject.FindGameObjectWithTag("Player").transform;
		_health.VisibilityToggle(false);
		
		foreach (var item in headgear)
			item.SetActive(false);
	}

	private void OnDrawGizmos()
	{
		if(!showOverlapBoxDebug) return;
		
		// cache previous Gizmos settings
		Matrix4x4 prevMatrix = Gizmos.matrix;

		Gizmos.color = new Color(0f, 0.75f, 1f, 0.5f);
		Gizmos.matrix = transform.localToWorldMatrix;

		var boxPosition = transform.position + transform.forward * overlapBoxBounds.distance;

		// convert from world position to local position
		boxPosition = transform.InverseTransformPoint(boxPosition); 

		var boxSize = new Vector3(overlapBoxBounds.x, overlapBoxBounds.y, overlapBoxBounds.z);
		Gizmos.DrawCube(boxPosition, boxSize);

		// restore previous Gizmos settings
		Gizmos.matrix = prevMatrix;
	}

	private void GoRagdoll(Vector3 direction)
	{
		if(isRagdoll) return;
		
		_anim.enabled = false;
		_anim.applyRootMotion = false;
		isRagdoll = true;

		foreach (var rb in rigidbodies)
		{
			rb.isKinematic = false;
			rb.AddForce(direction * 10f, ForceMode.Impulse);
		}

		Invoke(nameof(GoKinematic), 4f);
		
		GameEvents.Only.InvokeEnemyKill();

		foreach (var rb in rigidbodies)
			rb.tag = "Untagged";
		
		_audioSource.Play();
		Vibration.Vibrate(25);
	}
	
	private void GoKinematic()
	{
		foreach (var rb in rigidbodies)
			rb.isKinematic = false;
	}

	private IEnumerator AttackCycle()
	{
		var playerTemp = _player.position;
		var myTemp = transform.position;

		playerTemp.y = myTemp.y = 0f;
		
		while (!isDead)
		{
			_anim.SetTrigger(Grab);
			_audioSource.PlayOneShot(roar);
			_isAttacking = true;
			
			//grab animation calls get car on animation -> grab vehicle -> attack anim -> throws car
			
			yield return new WaitUntil(() => !_isAttacking);
			yield return GameExtensions.GetWaiter(waitBetweenAttacks);
			transform.DORotateQuaternion(Quaternion.LookRotation(playerTemp - myTemp), 0.2f);
			transform.DOMoveY(endYValue, 0.2f);
		}
	}
	
	private IEnumerator GrabVehicle()
	{
		if(Math.Abs(endYValue) < 0.01f)
			while (transform.position.y > 0.5f)
				yield return GameExtensions.GetWaiter(0.25f);

		_grabbedTargetCarController = null;
		do
		{
			var colliders = Physics.OverlapBox(transform.position + transform.forward * overlapBoxBounds.distance, 
				new Vector3(overlapBoxBounds.x / 2, overlapBoxBounds.y / 2, overlapBoxBounds.z / 2), Quaternion.identity);

			foreach (var item in colliders)
			{
				if(!item.CompareTag("Target")) continue;

				_grabbedTargetTransform = item.transform;
				_grabbedTargetTransform.TryGetComponent(out _grabbedTargetPropController);
				if(_grabbedTargetPropController.hasBeenInteractedWith)
				{
					_grabbedTargetTransform = null;
					_grabbedTargetPropController = null;
					continue;
				}
					
				_grabbedTargetTransform.TryGetComponent(out _grabbedTargetCarController);				
				_grabbedTargetTransform.tag = "EnemyAttack";

				GameEvents.Only.InvokeGiantPickupProp(_grabbedTargetTransform);
				_audioSource.PlayOneShot(propPull);
				break;
			}

			yield return GameExtensions.GetWaiter(0.2f);
		} while (!_grabbedTargetTransform);

		if (_grabbedTargetCarController)
		{
			_grabbedTargetCarController.StopMoving();
			_grabbedTargetCarController.AddTrail(trailPrefab);
		}
		else
			_grabbedTargetPropController.AddTrail(trailPrefab);

		_grabbedTargetTransform.DOMove(carHolderSlot.position, 0.5f).OnComplete(() => _anim.SetTrigger(Attack));
		_tweener = _grabbedTargetTransform.DOLocalRotate(Vector3.up * 360f, 2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
	}

	public void GetCarOnAnimation()
	{
		StartCoroutine(GrabVehicle());
	}
	
	public void ThrowVehicleOnAnimation()
	{
		var rb = _grabbedTargetTransform.GetComponent<Rigidbody>();
		_grabbedTargetTransform.parent = null;

		rb.isKinematic = false;
		rb.useGravity = true;
		//if it collides w another barrel mid air, it falls and lays there - you know, just killing the vibe
		var seq = DOTween.Sequence();
		seq.AppendCallback(() => rb.AddForce((GameObject.FindGameObjectWithTag("Player").transform.position - _grabbedTargetTransform.position).normalized * throwForce, ForceMode.Impulse));
		seq.AppendInterval(3f);
		seq.AppendCallback(() =>
		{
			if (rb)
				rb.AddForce(Vector3.right * 200f, ForceMode.Impulse);
			
			_grabbedTargetTransform = null;
			_grabbedTargetCarController = null;
			_grabbedTargetPropController = null;
		});

		_tweener.Kill();
		_tweener = null;
		_isAttacking = false;
	}

	public void StartJumpOnAnimation()
	{
		//this duration comes from animation - event keyframe time with anim fps, approx 32 * 1/60 of a second 
		transform.DOMoveY(endYValue, 0.5f).SetEase(Ease.InQuad);
	}

	public void ScreenShakeOnAnimation()
	{
		CameraController.only.ScreenShake(2f);
		_audioSource.PlayOneShot(landing);
		smokeEffectOnLand.SetActive(true);
		
		GameEvents.Only.InvokeGiantLanding(transform);
		StartCoroutine(AttackCycle());
		Vibration.Vibrate(20);
	}

	public void GetHit(bool isPartOfMultipleHits = false)
	{
		/*
		 this will try adding a hit
		 if this hit is part of multiple hits, we dont want to care about cooldowns
		 */
		if(!_health.AddHit(!isPartOfMultipleHits)) return;
		
		_anim.SetTrigger(Hit);
		Vibration.Vibrate(20);

		if (headgear.Count > 0)
		{
			ThrowHeadgear(headgear[0]);
			headgear.RemoveAt(0);
		}

		if (!_health.IsDead()) return;
		
		GoRagdoll(-transform.forward);
		ReleaseVehicle();
	}

	private static void ThrowHeadgear(GameObject obj)
	{
		obj.transform.parent = null;
		var rb = obj.GetComponent<Rigidbody>();
		rb.isKinematic = false;
		rb.useGravity = true;
		rb.AddForce(rb.transform.up * 12f);
	}

	private void ReleaseVehicle()
	{
		if(_grabbedTargetCarController)
			_grabbedTargetCarController.DropVehicle();
		else
			_grabbedTargetPropController.DropProp();
		
		_tweener?.Kill();
	}

	private void OnReachNextArea()
	{
		if(!LevelFlowController.only.IsThisLastEnemy()) return;

		_health.VisibilityToggle(true);
		
		
		foreach (var item in headgear)
			item.SetActive(true);
		
		foreach (var rend in rends)
			rend.enabled = true;
		_anim.SetTrigger(Jump);
	}

	public Vector3 GetBoundsCenter() => rends[0].bounds.center;
}