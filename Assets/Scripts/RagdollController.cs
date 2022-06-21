using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
	[SerializeField] private bool shouldPlayRandomAnims, doesTeaseWhenIdle, isGiant;
	public Rigidbody chest;
	[SerializeField] private Rigidbody[] rigidbodies;
	[HideInInspector] public bool isRagdoll, isWaitingForPunch, isAttackerSoCantRagdoll;
	[SerializeField] private float giantStompForce;

	[Header("Change color on death"), SerializeField] private Renderer skin;
	[SerializeField] private bool shouldTurnToGrey, shouldMirror = true;
	[SerializeField] private int toChangeMatIndex;
	[SerializeField] private Color deadColor;
	private Material _material;

	[Header("Health/ Death Modifiers")] public bool touchToKill;
	[SerializeField] private List<Rigidbody> armor;

	[Header("Audio"), SerializeField] private AudioClip punch1;
	[SerializeField] private AudioClip punch2;

	private Animator _anim;
	private AudioSource _audioSource;
	private EnemyPatroller _patroller;
	private ThrowAtPlayer _throwAtPlayer;
	private HealthController _health;
	private ShatterEnemyController _shatterEnemy;
	
	private bool _isAttacking;

	private static readonly int IsFlying = Animator.StringToHash("isFlying");
	private static readonly int Attack1 = Animator.StringToHash("attack1");
	private static readonly int Attack2 = Animator.StringToHash("attack2");
	private static readonly int IsMirrored = Animator.StringToHash("isMirrored");
	private static readonly int HasWon = Animator.StringToHash("hasWon");
	private static readonly int Idle1 = Animator.StringToHash("idle1");
	private static readonly int Idle2 = Animator.StringToHash("idle2");
	private static readonly int Idle3 = Animator.StringToHash("idle3");
	private static readonly int Idle4 = Animator.StringToHash("idle4");
	private static readonly int Idle5 = Animator.StringToHash("idle5");
	private static readonly int Idle6 = Animator.StringToHash("idle6");
	private static readonly int HitReaction = Animator.StringToHash("hitReaction");

	private void OnEnable()
	{
		GameEvents.Only.MoveToNextArea += OnMoveToNextArea;
		GameEvents.Only.ReachNextArea += OnReachNextArea;
		GameEvents.Only.GiantLanding += OnGiantLanding;

		GameEvents.Only.EnemyKillPlayer += OnEnemyReachPlayer;
	}

	private void OnDisable()
	{
		GameEvents.Only.MoveToNextArea -= OnMoveToNextArea;
		GameEvents.Only.ReachNextArea -= OnReachNextArea;
		GameEvents.Only.GiantLanding -= OnGiantLanding;
		
		GameEvents.Only.EnemyKillPlayer -= OnEnemyReachPlayer;
	}
	
	private void Start()
	{
		_anim = GetComponent<Animator>();
		_audioSource = GetComponent<AudioSource>();
		TryGetComponent(out _patroller);
		TryGetComponent(out _throwAtPlayer);
		TryGetComponent(out _health);
		TryGetComponent(out _shatterEnemy);
		
		if(BananaThrower.LevelFlowController.only)
			if (BananaThrower.LevelFlowController.only.isBeingChased)
				MakeKinematic();
		
		if(shouldMirror)
			_anim.SetBool(IsMirrored, Random.value > 0.5f);

		if (shouldTurnToGrey)
			_material = skin.materials[toChangeMatIndex];

		if (isGiant) return;
		
		if(!shouldPlayRandomAnims) return;
		PlayRandomAnim();
	}

	private void MakeKinematic()
	{
		foreach (var rigidbody in rigidbodies) rigidbody.isKinematic = true;
	}

	public bool IsInPatrolArea()
	{
		return _patroller ? _patroller.IsInCurrentPatrolArea() : _shatterEnemy.IsInCurrentArea();
	}

	public bool TryHoldInAir()
	{
		if (_health)
		{
			_health.AddHit();
			if (!_health.IsDead())
			{
				DropArmor();
				GameEvents.Only.InvokeDropArmor();
				return false;
			}
		}
		
		isWaitingForPunch = true;
		_anim.SetBool(IsFlying, true);
		_anim.applyRootMotion = false;
		
		if(_patroller)
			_patroller.ToggleAI(false);
		foreach (var rb in rigidbodies)
			rb.isKinematic = true;
		
		return true;
	}

	private void DropArmor()
	{
		_anim.SetTrigger(HitReaction);
		
		if(armor.Count == 0) return;
		if(!armor[0]) return;
		
		armor[0].isKinematic = false;
		armor[0].AddForce(Vector3.up * 2f);
		armor[0].transform.parent = null;
		armor.RemoveAt(0);
	}

	public void TryGoRagdoll(Vector3 direction, bool isPartOfMultipleHits = false)
	{
		if (!_health)
		{
			GoRagdoll(direction);
			return;
		}
		
		_health.AddHit(!isPartOfMultipleHits);
		if (_health.IsDead())
			GoRagdoll(direction);
		else
		{
			DropArmor();
			GameEvents.Only.InvokeDropArmor();
		}
	}

	public void GoRagdoll(Vector3 direction)
	{
		if(isRagdoll) return;
		if(isAttackerSoCantRagdoll) return;
		
		_anim.enabled = false;
		_anim.applyRootMotion = false;
		isRagdoll = true;
		if(_patroller)
			_patroller.ToggleAI(false);
		
		foreach (var rb in rigidbodies)
		{
			rb.isKinematic = false;
			rb.AddForce(direction * 10f, ForceMode.Impulse);
			rb.tag = "Untagged";
		}
		//GameEvents.Only.InvokeEnemyKill();
		GameEvents.Only.InvokeEnemyDied(transform);
		InputHandler.Only.GetLeftHand().InformAboutRagdollDeath(this);
		if(_throwAtPlayer)
			_throwAtPlayer.StopThrowing();

		var x = GetComponentInChildren<EnemyWeaponController>();
		if (x)
			x.OnDeath();

		if(shouldTurnToGrey)
			_material.DOColor(deadColor, 1f);
		
		_audioSource.Play();
		Vibration.Vibrate(25);
	}

	private void PlayRandomAnim()
	{
		_anim.SetBool(Idle1, false);
		_anim.SetBool(Idle2, false);
		_anim.SetBool(Idle3, false);
		_anim.SetBool(Idle4, false);
		_anim.SetBool(Idle5, false);
		_anim.SetBool(Idle6, false);

		DOVirtual.DelayedCall(0.02f, () =>
		{
			var random = Random.Range(0f, 1f);
			var anims = doesTeaseWhenIdle ? 6 : 3;
			var step = 1f / anims;

			var value = (int)(random / step);
			
			switch (value)
			{
				case 0:
					_anim.SetBool(Idle1, true);
					break;
				case 1:
					_anim.SetBool(Idle2, true);
					break;
				case 2:
					_anim.SetBool(Idle3, true);
					break;
				case 3:
					_anim.SetBool(Idle4, true);
					break;
				case 4:
					_anim.SetBool(Idle5, true);
					break;
				case 5:
					_anim.SetBool(Idle6, true);
					break;
			}
		});
	}
	
	public void AttackEnemy()
	{
		if(_isAttacking) return;

		_anim.applyRootMotion = true;
		_anim.SetTrigger(Random.value > 0.5f ? Attack1 : Attack2);
		if(_patroller)
			_patroller.ToggleAI(false);
		_isAttacking = true;
		InputHandler.AssignTemporaryDisabledState();
	}

	public void PopScale() => transform.DOPunchScale(Vector3.one * 0.125f, 0.25f);

	public void WalkOnAnimation()
	{
		CameraController.only.ScreenShake(0.5f);
	}

	public void HitOnAnimation()
	{
		Vibration.Vibrate(35);
		CameraController.only.ScreenShake(2f);
		_audioSource.PlayOneShot(Random.Range(0f, 1f) > 0.5f ? punch1 : punch2);
	}
	
	private void OnMoveToNextArea()
	{
		if(_patroller)
			if (_patroller.myPatrolArea > LevelFlowController.only.currentArea)
			{
				_anim.enabled = false;
				gameObject.SetActive(false);
				DOVirtual.DelayedCall(0.015f, () =>
				{
					_anim.enabled = true;
					gameObject.SetActive(true);
				});
			}

		if(!shouldPlayRandomAnims) return;
		PlayRandomAnim();
	}

	private void OnReachNextArea()
	{
		if(LevelFlowController.only.isGiantLevel) return;
		if (!_patroller) return;
		
		if (_patroller.myPatrolArea < LevelFlowController.only.currentArea) gameObject.SetActive(false);
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