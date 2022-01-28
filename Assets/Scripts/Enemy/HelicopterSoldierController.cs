using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class HelicopterSoldierController : MonoBehaviour
{
	[SerializeField] private Rigidbody[] rigidbodies;
	
	[Header("Bullets"), SerializeField] private Transform bulletHole;
	[SerializeField] private GameObject bulletMuzzle;
	[SerializeField] private float muzzleScale;

	private Animator _anim;
	private AudioSource _audioSource;

	private Sequence _mySeq;
	private bool _isRagdoll;
	
	private static readonly int Fire = Animator.StringToHash("Fire");
	private static readonly int HitReaction = Animator.StringToHash("hitReaction");

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_audioSource = GetComponent<AudioSource>();
	}
	
	public void GoRagdoll(Vector3 direction)
	{
		if(_isRagdoll) return;
		
		_anim.enabled = false;
		_anim.applyRootMotion = false;
		_isRagdoll = true;

		foreach (var rb in rigidbodies)
		{
			rb.isKinematic = false;
			rb.AddForce(direction * 5f, ForceMode.Impulse);
			rb.tag = "Untagged";
		}

		Invoke(nameof(GoKinematic), 4f);
		
		GameEvents.only.InvokeEnemyKill();

		_audioSource.Play();
		Vibration.Vibrate(25);
	}

	public void GetHit() => _anim.SetTrigger(HitReaction);
	
	private void GoKinematic()
	{
		foreach (var rb in rigidbodies)
			rb.isKinematic = false;
	}

	public void Shoot()
	{
		var muzzle = Instantiate(bulletMuzzle).transform;

		muzzle.localScale = Vector3.one * muzzleScale;
		muzzle.parent = bulletHole;
	
		muzzle.localPosition = Vector3.zero;
		muzzle.localRotation = Quaternion.identity;

		_anim.SetTrigger(Fire);
		GameEvents.only.InvokeEnemyHitPlayer(transform);

		Destroy(muzzle.gameObject, 3f);
		Vibration.Vibrate(15);
	}
}