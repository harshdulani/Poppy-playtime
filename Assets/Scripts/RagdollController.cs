using UnityEngine;

public class RagdollController : MonoBehaviour
{
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
	
	private void OnEnable()
	{
		GameEvents.only.enemyReachPlayer += OnEnemyReachPlayer;
	}

	private void OnDisable()
	{
		GameEvents.only.enemyReachPlayer -= OnEnemyReachPlayer;
	}
	
	private void Start()
	{
		_anim = GetComponent<Animator>();
		TryGetComponent(out _patroller);
		
		_anim.SetBool(IsMirrored, Random.value > 0.5f);
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

	public void AttackEnemy()
	{
		if(_isAttacking) return;
		
		_anim.SetTrigger(Random.value > 0.5f ? Attack1 : Attack2);
		_patroller.ToggleAI(false);
		_isAttacking = true;
	}
	
	private void OnEnemyReachPlayer()
	{
		_anim.SetBool(HasWon, true);
	}
}
