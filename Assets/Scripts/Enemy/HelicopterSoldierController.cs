using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class HelicopterSoldierController : MonoBehaviour
{
	[SerializeField] private Rigidbody[] rigidbodies;
	
	[Header("Bullets"), SerializeField] private Transform bulletHole;
	[SerializeField] private GameObject bulletMuzzle;
	[SerializeField] private float shootInterval, shootIntervalVariation, muzzleScale;

	[Header("Canvas"), SerializeField] private TextMeshProUGUI seconds;
	[SerializeField] private GuiProgressBarUI progressBarUi;
	
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
		
		_mySeq.AppendCallback(() =>
		{
			progressBarUi.Value = 0f;
			_myShootElapsed = _myShootInterval;
		});
		_mySeq.Append(DOTween.To(() => progressBarUi.Value, value => progressBarUi.Value = value, 1f, _myShootInterval));
		_mySeq.Join(DOTween.To(() => _myShootElapsed, value => _myShootElapsed = value, 0f, _myShootInterval).OnUpdate(() => seconds.text = _myShootElapsed.ToString("0.00")));

		_mySeq.AppendCallback(Shoot);
		
		_mySeq.AppendInterval(Random.Range(0, shootIntervalVariation));
		
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
			rb.AddForce(direction * 5f, ForceMode.Impulse);
			rb.tag = "Untagged";
		}

		Invoke(nameof(GoKinematic), 4f);
		
		GameEvents.only.InvokeEnemyKill();

		_mySeq.Kill();
		
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
		print("shoot start");
		var muzzle = Instantiate(bulletMuzzle).transform;

		muzzle.localScale = Vector3.one * muzzleScale;
		muzzle.parent = bulletHole;
	
		muzzle.localPosition = Vector3.zero;
		muzzle.localRotation = Quaternion.identity;

		_anim.SetTrigger(Fire);
		GameEvents.only.InvokeEnemyHitPlayer(transform);
		//invoke enemy hit player
		
		print("shoot end");
		
		Destroy(muzzle.gameObject, 3f);
		Vibration.Vibrate(15);
	}
}