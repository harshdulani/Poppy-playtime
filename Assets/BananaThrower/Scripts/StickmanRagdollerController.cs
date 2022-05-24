using UnityEngine;
using DG.Tweening;

public class StickmanRagdollerController : MonoBehaviour
{
	[SerializeField] private Rigidbody[] rigidbodies;
	[HideInInspector] public bool isRagdoll;

	[Header("Change color on death"), SerializeField] private Renderer skin;
	[SerializeField] private bool shouldTurnToGrey, shouldMirror = true;
	[SerializeField] private int toChangeMatIndex;
	[SerializeField] private Color deadColor;
	private Material _material;

	private Animator _anim;
	private AudioSource _audioSource;
	private StickmanMovementController _stickmanMovementController;
	
	private bool _isAttacking;

	private static readonly int IsMirrored = Animator.StringToHash("isMirrored");
	private static readonly int HasWon = Animator.StringToHash("hasWon");
	private static readonly int Idle1 = Animator.StringToHash("idle1");
	private static readonly int Idle2 = Animator.StringToHash("idle2");
	private static readonly int Idle3 = Animator.StringToHash("idle3");

	private void OnEnable()
	{
		GameEvents.Only.EnemyKillPlayer += OnEnemyReachPlayer;
	}

	private void OnDisable()
	{
		GameEvents.Only.EnemyKillPlayer -= OnEnemyReachPlayer;
	}
	
	private void Start()
	{
		_anim = GetComponent<Animator>();
		_audioSource = GetComponent<AudioSource>();
		
		_stickmanMovementController = GetComponent<StickmanMovementController>();
		
		if(BananaThrower.LevelFlowController.only)
			if (BananaThrower.LevelFlowController.only.isBeingChased)
				MakeKinematic();
		
		if(shouldMirror)
			_anim.SetBool(IsMirrored, Random.value > 0.5f);

		if (shouldTurnToGrey)
			_material = skin.materials[toChangeMatIndex];

		PlayRandomAnim();
	}

	private void MakeKinematic()
	{
		foreach (var rigidbody in rigidbodies) rigidbody.isKinematic = true;
	}
	
	public void GoRagdoll(Vector3 direction)
	{
		if(isRagdoll) return;
		
		_anim.enabled = false;
		_anim.applyRootMotion = false;
		isRagdoll = true;
		_stickmanMovementController.ToggleAI(false);
		
		foreach (var rb in rigidbodies)
		{
			rb.isKinematic = false;
			rb.AddForce(direction * 70f, ForceMode.Impulse);
			rb.tag = "Untagged";
		}
		
		GameEvents.Only.InvokeEnemyDied(transform);

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

		DOVirtual.DelayedCall(0.02f, () =>
		{
			var random = Random.Range(0f, 1f);
			if (random < 0.33f)
				_anim.SetBool(Idle1, true);
			else if (random < 0.66f)
				_anim.SetBool(Idle2, true);
			else
				_anim.SetBool(Idle3, true);
		});
	}
	
	private void OnEnemyReachPlayer()
	{
		if(_isAttacking) return;
		_anim.SetBool(HasWon, true);
	}
}
