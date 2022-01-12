using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class GiantController : MonoBehaviour
{
	[HideInInspector] public bool isDead;
	[SerializeField] private Renderer rend;
	
	[SerializeField] private bool isRagdoll;
	[SerializeField] private Rigidbody[] rigidbodies;

	[SerializeField] private bool showOverlapBoxDebug;
	[SerializeField] private Transform carHolderSlot;
	[SerializeField] private float overlapBoxDistance, throwForce, waitBetweenAttacks;

	[SerializeField] private GameObject trailPrefab;
	
	private Transform _player;
	private CarController _grabbedCar;
	private Coroutine _grabCarCoroutine, _attackCycleCoroutine;
	
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
		GameEvents.only.playerPickupCar += OnPlayerPickupCar;
	}

	private void OnDisable()
	{
		GameEvents.only.reachNextArea -= OnReachNextArea;
		GameEvents.only.playerPickupCar -= OnPlayerPickupCar;
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_audioSource = GetComponent<AudioSource>();
		_health = GetComponent<HealthController>();
		rend.enabled = false;

		_player = GameObject.FindGameObjectWithTag("Player").transform;
		_health.VisibilityToggle(false);
	}

	private void OnDrawGizmos()
	{
		if(!showOverlapBoxDebug) return;
		
		Gizmos.color = new Color(0f, 0.75f, 1f, 0.5f);
		Gizmos.DrawCube(transform.position + transform.forward * overlapBoxDistance, new Vector3(20, 10f, 20f));
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
		
		_grabbedCar = null;
		
		do
		{
			var colliders = Physics.OverlapBox(transform.position + transform.forward * overlapBoxDistance, new Vector3(10, 5f, 10f), Quaternion.identity);

			foreach (var item in colliders)
			{
				if(!item.CompareTag("Target")) continue;

				_grabbedCar = item.transform.GetComponent<CarController>();
				_grabbedCar.tag = "EnemyAttack";
				GameEvents.only.InvokeGiantPickupCar(_grabbedCar.transform);
				break;
			}

			yield return GameExtensions.GetWaiter(0.2f);
		} while (!_grabbedCar);

		_health.AddGrabbedCar(_grabbedCar.transform);
		_grabbedCar.StopMoving();
		_grabbedCar.AddTrail(trailPrefab);

		_grabbedCar.transform.DOMove(carHolderSlot.position, 0.5f).OnComplete(() => _anim.SetTrigger(Attack));
		_tweener = _grabbedCar.transform.DOLocalRotate(Vector3.up * 360f, 2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
	}

	public void GetCarOnAnimation()
	{
		_grabCarCoroutine = StartCoroutine(GrabVehicle());
	}
	
	public void ThrowVehicleOnAnimation()
	{
		var rb = _grabbedCar.GetComponent<Rigidbody>();
		_grabbedCar.transform.parent = null;

		rb.isKinematic = false;
		rb.AddForce((GameObject.FindGameObjectWithTag("Player").transform.position - _grabbedCar.transform.position).normalized * throwForce, ForceMode.Impulse);
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

		_attackCycleCoroutine = StartCoroutine(AttackCycle());
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
		_grabbedCar.DropVehicle();
		_tweener?.Kill();
	}

	private void OnPlayerPickupCar(Transform car)
	{
		if(car != _grabbedCar.transform) return;
		
		StopCoroutine(_grabCarCoroutine);
		_isAttacking = false;
		StopCoroutine(_attackCycleCoroutine);
		_attackCycleCoroutine = StartCoroutine(AttackCycle());
	}
	
	private void OnReachNextArea()
	{
		if(!LevelFlowController.only.IsThisLastEnemy()) return;
		
		_health.VisibilityToggle(true);
		rend.enabled = true;
		_anim.SetTrigger(Jump);
	}
}