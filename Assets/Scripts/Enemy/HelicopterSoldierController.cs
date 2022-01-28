using DG.Tweening;
using UnityEngine;

public class HelicopterSoldierController : MonoBehaviour
{
	[SerializeField] private GameObject bulletPrefab, bulletMuzzle;
	[SerializeField] private Transform bulletHole;
	[SerializeField] private Rigidbody[] rigidbodies;
	[SerializeField] private float shootInterval, bulletForce, shootIntervalVariation;

	private Animator _anim;
	private AudioSource _audioSource;

	private Sequence _mySeq;
	private float _myShootInterval, _myShootElapsed;
	private bool _isRagdoll;
	
	private static readonly int Fire = Animator.StringToHash("Fire");
	private static readonly int HitReaction = Animator.StringToHash("hitReaction");

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_audioSource = GetComponent<AudioSource>();
		
		_myShootInterval = shootInterval + Random.Range(-shootIntervalVariation, shootIntervalVariation);
	}

	public void StartShooting()
	{
		_mySeq = DOTween.Sequence();
		
		_mySeq.AppendInterval(Random.Range(0, 2 * shootIntervalVariation));
		_mySeq.AppendCallback(() => _myShootElapsed = 0);

		_mySeq.Append(DOTween.To(() => _myShootElapsed, value => _myShootElapsed = value, _myShootInterval,
			_myShootInterval));
		//_mySeq.Join()
		
		_mySeq.AppendCallback(Shoot);
		_mySeq.SetLoops(-1);
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
			rb.AddForce(direction * 10f, ForceMode.Impulse);
		}

		Invoke(nameof(GoKinematic), 4f);
		
		GameEvents.only.InvokeEnemyKill();

		foreach (var rb in rigidbodies)
			rb.tag = "Untagged";
		
		
		
		_audioSource.Play();
		Vibration.Vibrate(25);
	}

	public void GetHit() => _anim.SetTrigger(HitReaction);
	
	private void GoKinematic()
	{
		foreach (var rb in rigidbodies)
			rb.isKinematic = false;
	}

	private void Shoot()
	{
		_anim.SetTrigger(Fire);
		var bullet = Instantiate(bulletPrefab, bulletHole.position, bulletHole.rotation);
		
		bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * bulletForce);
		
		Destroy(bullet, 3f);
		Destroy(Instantiate(bulletMuzzle, bulletHole.position, bulletHole.rotation), 3f);
		Vibration.Vibrate(15);
	}
}