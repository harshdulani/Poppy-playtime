using UnityEngine;

public class RagdollController : MonoBehaviour
{
	public bool isPoppy;
	public Rigidbody chest;
	[SerializeField] private Rigidbody[] rigidbodies;
	public bool isRagdoll, isWaitingForPunch;

	private Animator _anim;
	private EnemyPatroller _patroller;
	
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
		GameEvents.only.enemyReachPlayer += OnEnemyReachPlayer;
	}

	private void OnDisable()
	{
		GameEvents.only.moveToNextArea -= OnMoveToNextArea;
		GameEvents.only.enemyReachPlayer -= OnEnemyReachPlayer;
	}
	
	private void Start()
	{
		_anim = GetComponent<Animator>();
		TryGetComponent(out _patroller);
		
		_anim.SetBool(IsMirrored, Random.value > 0.5f);
		
		if(isPoppy) return;
		PlayRandomAnim();
	}

	public void HoldInAir()
	{
		isWaitingForPunch = true;
		_anim.SetBool(IsFlying, true);
		_patroller?.ToggleAI(false);
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

		GameEvents.only.InvokeEnemyKill();

		foreach (var rb in rigidbodies)
			rb.tag = "Untagged";
	}

	public void PlayRandomAnim()
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
		
		_anim.SetTrigger(Random.value > 0.5f ? Attack1 : Attack2);
		_patroller.ToggleAI(false);
		_isAttacking = true;
	}

	public void WalkOnAnimation()
	{
		print("screenshake light");
		CameraController.only.ScreenShake(1f);
	}

	public void HitOnAnimation()
	{
		print("screenshake heavy");
		//vibration heavy
		CameraController.only.ScreenShake(2f);
	}
	
	private void OnMoveToNextArea()
	{
		PlayRandomAnim();
	}
	
	private void OnEnemyReachPlayer()
	{
		if(_isAttacking) return;
		_anim.SetBool(HasWon, true);
	}
}
