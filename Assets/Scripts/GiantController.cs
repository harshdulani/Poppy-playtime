using DG.Tweening;
using UnityEngine;

public class GiantController : MonoBehaviour
{
	[HideInInspector] public bool isDead;

	[SerializeField] private Renderer rend;

	private Animator _anim;
	private AudioSource _audioSource;
	private HealthController _health;
	
	[SerializeField] private bool isRagdoll;
	[SerializeField] private Rigidbody[] rigidbodies;

	private static readonly int Hit = Animator.StringToHash("Hit");
	private static readonly int Jump = Animator.StringToHash("Jump");
	
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
	
	public void StartJumpOnAnimation()
	{
		//this duration comes from animation - event keyframe time with anim fps, approx 32 * 1/60 of a second 
		transform.DOMoveY(0f, 0.5f).SetEase(Ease.InQuad);
	}

	public void ScreenShakeOnAnimation()
	{
		Vibration.Vibrate(20);
		CameraController.only.ScreenShake(2f);
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