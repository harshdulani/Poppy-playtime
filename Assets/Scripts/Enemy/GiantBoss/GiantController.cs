using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

[System.Serializable]
public struct BoxBounds { public float x, y, z, distance; }

public class GiantController : MonoBehaviour
{
	[HideInInspector] public bool isDead;
	[SerializeField] private Renderer[] rends;
	
	[SerializeField] private bool isRagdoll;
	[SerializeField] private Rigidbody[] rigidbodies;
	[SerializeField] private GameObject[] headgear;

	[SerializeField] private bool showOverlapBoxDebug;
	[SerializeField] private BoxBounds overlapBoxBounds;
	[SerializeField] private Transform carHolderSlot;
	[Range(0.1f, 1f)] public float explosionScale = 1f;
	[SerializeField] private float throwForce, waitBetweenAttacks;
	
	[SerializeField] private GameObject trailPrefab;
	[SerializeField] private GameObject smokeEffectOnLand;
	
	private Transform _player, _grabbedTargetTransform;
	private CarController _grabbedTargetCarController;
	private PropController _grabbedTargetPropController;

	private Animator _anim;
	private AudioSource _audioSource;
	private HealthController _health;

	private TweenerCore<Quaternion, Vector3, QuaternionOptions> _tweener;
	private bool _isAttacking;
	
	private static readonly int Hit = Animator.StringToHash("Hit");
	private static readonly int Jump = Animator.StringToHash("Jump");
	private static readonly int Attack = Animator.StringToHash("Attack");
	private static readonly int Grab = Animator.StringToHash("Grab");

	private void OnEnable()
	{
		GameEvents.only.reachNextArea += OnReachNextArea;
	}

	private void OnDisable()
	{
		GameEvents.only.reachNextArea -= OnReachNextArea;
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_audioSource = GetComponent<AudioSource>();
		_health = GetComponent<HealthController>();

		foreach (var rend in rends)
			rend.enabled = false;

		_player = GameObject.FindGameObjectWithTag("Player").transform;
		_health.VisibilityToggle(false);
		
		foreach (var item in headgear)
			item.SetActive(false);

		LevelFlowController.only.SetGiant(this);
	}

	private void OnDrawGizmos()
	{
		if(!showOverlapBoxDebug) return;
		
		Gizmos.color = new Color(0f, 0.75f, 1f, 0.5f);
		Gizmos.DrawCube(transform.position + transform.forward * overlapBoxBounds.distance, new Vector3(overlapBoxBounds.x, overlapBoxBounds.y, overlapBoxBounds.z));
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
		
		GameEvents.only.InvokeEnemyKill();

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
			_isAttacking = true;

			//grab animation calls get car on animation -> grab vehicle -> attack anim -> throws car
			
			yield return new WaitUntil(() => !_isAttacking);
			yield return GameExtensions.GetWaiter(waitBetweenAttacks);
			transform.DORotateQuaternion(Quaternion.LookRotation(playerTemp - myTemp), 0.2f);
			transform.DOMoveY(0f, 0.2f);
		}
	}
	
	private IEnumerator GrabVehicle()
	{
		while(transform.position.y > 0.5f)
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
				_grabbedTargetTransform.TryGetComponent(out _grabbedTargetCarController);
				_grabbedTargetTransform.TryGetComponent(out _grabbedTargetPropController);
				
				_grabbedTargetTransform.tag = "EnemyAttack";

				GameEvents.only.InvokeGiantPickupCar(_grabbedTargetTransform);
				break;
			}

			yield return GameExtensions.GetWaiter(0.2f);
		} while (!_grabbedTargetTransform);

		_health.AddGrabbedCar(_grabbedTargetTransform);

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
		//if it collides w another barrel mid air, it falls and lays there - you know, just killing the vibe
		var seq = DOTween.Sequence();
		seq.AppendCallback(() => rb.AddForce((GameObject.FindGameObjectWithTag("Player").transform.position - _grabbedTargetTransform.position).normalized * throwForce, ForceMode.Impulse));
		seq.AppendInterval(3f);
		seq.AppendCallback(() =>
		{
			if (rb)
				rb.AddForce(Vector3.right * 200f, ForceMode.Impulse);
		});
		
		_tweener.Kill();
		_tweener = null;
		_isAttacking = false;
	}

	public void StartJumpOnAnimation()
	{
		//this duration comes from animation - event keyframe time with anim fps, approx 32 * 1/60 of a second 
		transform.DOMoveY(0f, 0.5f).SetEase(Ease.InQuad);
	}

	public void ScreenShakeOnAnimation()
	{
		Vibration.Vibrate(20);
		CameraController.only.ScreenShake(2f);

		smokeEffectOnLand.SetActive(true);
		
		GameEvents.only.InvokeGiantLanding(transform);
		StartCoroutine(AttackCycle());
	}

	public void GetHit(Transform hitter)
	{
		if(!_health.AddHit(hitter)) return;
		_anim.SetTrigger(Hit);
		Vibration.Vibrate(20);

		if (!_health.IsDead()) return;
		
		GoRagdoll(-transform.forward);
		ReleaseVehicle();
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