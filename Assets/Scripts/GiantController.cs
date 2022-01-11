using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GiantController : MonoBehaviour
{
	[HideInInspector] public bool isDead;
	[SerializeField] private Renderer rend;
	
	[SerializeField] private bool isRagdoll;
	[SerializeField] private Rigidbody[] rigidbodies;
	
	[SerializeField] private Transform carHolderSlot;
	[SerializeField] private float carInterpDuration, throwForce;

	private Transform _grabbedCar;

	private Tweener _tween;
	
	private Animator _anim;
	private AudioSource _audioSource;
	private HealthController _health;

	private static readonly int Hit = Animator.StringToHash("Hit");
	private static readonly int Jump = Animator.StringToHash("Jump");
	private static readonly int Attack = Animator.StringToHash("Attack");
	private static readonly int Grab = Animator.StringToHash("Grab");

	private void OnEnable()
	{
		GameEvents.only.reachNextArea += ReachNextArea;
	}

	private void OnDisable()
	{
		GameEvents.only.reachNextArea -= ReachNextArea;
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_audioSource = GetComponent<AudioSource>();
		_health = GetComponent<HealthController>();
		rend.enabled = false;
	}

	public void GoRagdoll(Vector3 direction)
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

	private IEnumerator GrabVehicle()
	{
		while(transform.position.y > 0.2f)
			yield return GameExtensions.GetWaiter(0.25f);

		_grabbedCar = null;
		
		do
		{
			var colliders = Physics.OverlapBox(transform.position + transform.forward * 25f, new Vector3(10, 5f, 10f), Quaternion.identity);

			Debug.DrawRay(transform.position + transform.forward * 25f, Vector3.right * 10f, Color.red, 0.2f);
			Debug.DrawRay(transform.position + transform.forward * 25f, Vector3.up * 5f, Color.green, 0.2f);
			Debug.DrawRay(transform.position + transform.forward * 25f, Vector3.forward * 10, Color.blue, 0.2f);
			
			foreach (var item in colliders)
			{
				if(!item.CompareTag("Target")) continue;

				_grabbedCar = item.transform;
				break;
			}

			yield return GameExtensions.GetWaiter(0.2f);
		} while (!_grabbedCar);
		
		_health.AddGrabbedCar(_grabbedCar);
		_grabbedCar.GetComponent<CarController>().StopMoving();

		_grabbedCar.DOMove(carHolderSlot.position, 0.5f).OnComplete(() => _anim.SetTrigger(Attack));
		_grabbedCar.DOLocalRotate(Vector3.up * 360f, 2f, RotateMode.LocalAxisAdd);
	}

	public void GetCarOnAnimation()
	{
		StartCoroutine(GrabVehicle());
	}
	
	public void ThrowVehicleOnAnimation()
	{
		var rb = _grabbedCar.GetComponent<Rigidbody>();
		_grabbedCar.transform.parent = null;

		rb.isKinematic = false;
		_grabbedCar.tag = "EnemyAttack";
		rb.AddForce((GameObject.FindGameObjectWithTag("Player").transform.position - _grabbedCar.position).normalized * throwForce, ForceMode.Impulse);
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
		
		_anim.SetTrigger(Grab);
	}

	public void GetHit(Transform hitter)
	{
		if(!_health.AddHit(hitter)) return;
		_anim.SetTrigger(Hit);
		Vibration.Vibrate(20);

		if (_health.IsDead())
			GoRagdoll(-transform.forward);
	}
	
	private void ReachNextArea()
	{
		if(!LevelFlowController.only.IsThisLastEnemy()) return;
		
		rend.enabled = true;
		_anim.SetTrigger(Jump);
	}
}