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
	}

	private void MakeKinematic()
	{
		foreach (var rigidbody in rigidbodies) rigidbody.isKinematic = true;
	}
	
	public void GoRagdoll(Vector3 direction)
	{
		if(_stickmanMovementController.hasWon) return;
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
		CameraController.only.ScreenShake(2f);
		Vibration.Vibrate(25);
	}

	public void HitOnAnimation()
	{
		Vibration.Vibrate(35);
		CameraController.only.ScreenShake(2f);
	}
}
