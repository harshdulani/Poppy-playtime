using System.Collections;
using DG.Tweening;
using UnityEngine;

public class HelicopterSoldierController : MonoBehaviour
{
	[SerializeField] private Rigidbody[] rigidbodies;
	
	[Header("Bullets"), SerializeField] private Transform bulletHole;
	[SerializeField] private GameObject bulletMuzzle, bulletPrefab;
	[SerializeField] private float muzzleScale;

	private static Transform _player;
	private Animator _anim;
	private AudioSource _audioSource;

	private Sequence _mySeq;
	private bool _isRagdoll;
	
	private static readonly int Fire = Animator.StringToHash("Fire");
	private static readonly int HitReaction = Animator.StringToHash("hitReaction");

	private void OnDisable()
	{
		_player = null;
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_audioSource = GetComponent<AudioSource>();

		if(!_player)
			_player = GameObject.FindGameObjectWithTag("Player").transform;
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
			rb.AddForce(direction, ForceMode.Impulse);
			rb.tag = "Untagged";
		}

		Invoke(nameof(GoKinematic), 4f);
		
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

		AudioManager.instance.Play("Rifle");
		StartCoroutine(ShootBullet());
		
		_anim.SetTrigger(Fire);
		GameEvents.Only.InvokeEnemyHitPlayer(transform);

		Destroy(muzzle.gameObject, 3f);
		Vibration.Vibrate(15);
	}

	private IEnumerator ShootBullet()
	{
		var bullet = Instantiate(bulletPrefab, bulletHole.position, bulletHole.rotation);

		bullet.transform.parent = bulletHole;
		bullet.transform.localPosition = Vector3.zero;
		yield return null;
		yield return null;

		var x = bulletHole.transform.TransformPoint(bullet.transform.localPosition);
		Debug.DrawLine(x, x + Vector3.up * 2, Color.red, 3f);
		bullet.transform.parent = null;
		
		bullet.GetComponent<Rigidbody>().AddForce((_player.position + Vector3.up * 2.5f - bulletHole.position).normalized * 200f, ForceMode.Impulse);
	}
}