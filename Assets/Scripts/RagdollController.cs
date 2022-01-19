using DG.Tweening;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
	public bool isPoppy, isGiant;
	public Rigidbody chest;
	[SerializeField] private Rigidbody[] rigidbodies;
	[HideInInspector] public bool isRagdoll, isWaitingForPunch;

	[SerializeField] private bool shouldTurnToGrey;
	[SerializeField] private Renderer skin;
	[SerializeField] private int toChangeMatIndex;
	private Material _material;
	[SerializeField] private Color deadColor;

	[Header("Audio"), SerializeField] private AudioClip punch1;
	[SerializeField] private AudioClip punch2;

	[SerializeField] private float giantStompForce;
	
	private Animator _anim;
	private EnemyPatroller _patroller;
	private AudioSource _audioSource;
	
	private bool _isAttacking;

	private static readonly int IsFlying = Animator.StringToHash("isFlying");
	private static readonly int Attack1 = Animator.StringToHash("attack1");
	private static readonly int Attack2 = Animator.StringToHash("attack2");
	private static readonly int IsMirrored = Animator.StringToHash("isMirrored");	
	private static readonly int HasWon = Animator.StringToHash("hasWon");
	private static readonly int Idle1 = Animator.StringToHash("idle1");
	private static readonly int Idle2 = Animator.StringToHash("idle2");
	private static readonly int Idle3 = Animator.StringToHash("idle3");
	
	private void OnEnable()
	{
		GameEvents.only.moveToNextArea += OnMoveToNextArea;
		GameEvents.only.giantLanding += OnGiantLanding;

		GameEvents.only.enemyKillPlayer += OnEnemyReachPlayer;
	}

	private void OnDisable()
	{
		GameEvents.only.moveToNextArea -= OnMoveToNextArea;
		GameEvents.only.giantLanding -= OnGiantLanding;
		
		GameEvents.only.enemyKillPlayer -= OnEnemyReachPlayer;
	}
	
	private void Start()
	{
		_anim = GetComponent<Animator>();
		_audioSource = GetComponent<AudioSource>();
		TryGetComponent(out _patroller);
		
		_anim.SetBool(IsMirrored, Random.value > 0.5f);

		if (shouldTurnToGrey)
			_material = skin.materials[toChangeMatIndex];

		if (isGiant) return;
		
		if(isPoppy) return;
		PlayRandomAnim();
	}

	public void HoldInAir()
	{
		isWaitingForPunch = true;
		_anim.SetBool(IsFlying, true);
		_anim.applyRootMotion = false;
		if(_patroller)
			_patroller.ToggleAI(false);
		foreach (var rb in rigidbodies)
			rb.isKinematic = true;
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
		
		var x = GetComponentInChildren<EnemyWeaponController>();
		if (x)
			x.OnDeath();

		_material.DOColor(deadColor, 1f);
		
		_audioSource.Play();
		Vibration.Vibrate(25);
	}

	private void PlayRandomAnim()
	{
		var random = Random.Range(0f, 1f);
		if(random < 0.33f)
			_anim.SetTrigger(Idle1);
		else if(random < 0.66f)
			_anim.SetTrigger(Idle2);
		else
			_anim.SetTrigger(Idle3);
	}
	
	public void AttackEnemy()
	{
		if(_isAttacking) return;

		_anim.applyRootMotion = true;
		_anim.SetTrigger(Random.value > 0.5f ? Attack1 : Attack2);
		_patroller.ToggleAI(false);
		_isAttacking = true;
		InputHandler.Only.AssignDisabledState();
	}

	private void GoKinematic()
	{
		foreach (var rb in rigidbodies)
			rb.isKinematic = false;
	}
	
	public void PopScale() => transform.DOPunchScale(Vector3.one * 0.125f, 0.25f);

	public void WalkOnAnimation()
	{
		CameraController.only.ScreenShake(1f);
	}

	public void HitOnAnimation()
	{
		Vibration.Vibrate(35);
		CameraController.only.ScreenShake(2f);
		_audioSource.PlayOneShot(Random.Range(0f, 1f) > 0.5f ? punch1 : punch2);
	}
	
	private void OnMoveToNextArea()
	{
		if(isPoppy) return;
		
		PlayRandomAnim();
	}

	private void OnGiantLanding(Transform giant)
	{
		if (!LevelFlowController.only.isGiantLevel) return;
		
		var direction = (transform.position - giant.position).normalized;
		direction.x *= 3f;
		direction.z = 0f;
		direction.y *= 30f;
		foreach (var rb in rigidbodies)
		{
			rb.isKinematic = false;
			rb.AddForce(direction * giantStompForce, ForceMode.Impulse);
		}
	}

	private void OnEnemyReachPlayer()
	{
		if(_isAttacking) return;
		_anim.SetBool(HasWon, true);
	}
}
