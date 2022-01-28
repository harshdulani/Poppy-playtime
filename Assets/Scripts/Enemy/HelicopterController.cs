using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HelicopterController : MonoBehaviour
{
	[SerializeField] private AnimationCurve easeCurve;
	[SerializeField] private Transform startTransform;
	[SerializeField] private int myArea;
	
	[Header("Sine Wave Noise"), SerializeField] private float magnitude;
	[SerializeField] private float rotationMagnitude, recenterLerp;
	private Vector3 _previousSine, _previousSineRot;

	[Header("Damage"), SerializeField] private List<HelicopterSoldierController> passengers;
	[SerializeField] private GameObject explosionPrefab;
	[SerializeField] private float deathExplosionForce, explosionScale;
	private HealthController _health;

	[Header("Attack"), SerializeField] private GuiProgressBarUI progressBarUi;
	[SerializeField] private float shootInterval;
	public float ProgressBarValue
	{
		get => progressBarUi.Value;
		set => progressBarUi.Value = value;
	}
	private Sequence _mySeq;
	private int _currentSoldierFire;

	private Rigidbody _rb;
	private bool _isDead;

	private void OnEnable()
	{
		GameEvents.only.tapToPlay += OnTapToPlay;
		GameEvents.only.reachNextArea += OnReachNextArea;
	}

	private void OnDisable()
	{
		GameEvents.only.tapToPlay -= OnTapToPlay;
		GameEvents.only.reachNextArea -= OnReachNextArea;
	}

	private void Start()
	{
		_health = GetComponent<HealthController>();
		_rb = GetComponent<Rigidbody>();
		_health.VisibilityToggle(false);

		_currentSoldierFire = 0;
	}

	private void Update()
	{
		if(_isDead) return;
		
		SineWave();
	}

	private void SineWave()
	{
		var sineY = Mathf.Sin(Time.time);
		
		var sine = Vector3.up * sineY;
		var sineRot = Vector3.forward * sineY;
		
		transform.position += (sine - _previousSine) * magnitude;
		transform.rotation *= Quaternion.Euler((sineRot - _previousSineRot) * rotationMagnitude);
		
		_previousSine = sine;
		_previousSineRot = sineRot;
	}

	private void HeliDeath()
	{
		_isDead = true;
		
		_rb.useGravity = true;
		_rb.isKinematic = false;
		_rb.constraints = RigidbodyConstraints.None;

		_rb.AddForce(Vector3.left * deathExplosionForce + Vector3.down * deathExplosionForce, ForceMode.Impulse);
		_rb.AddTorque(Vector3.up * 720f, ForceMode.Acceleration);

		EndShooterSequence();
	}

	private void GetHit()
	{
		if(!_health.AddHit()) return;

		if (passengers.Count > 0)
		{
			ThrowPassenger(passengers[0]);
			passengers.RemoveAt(0);
			for(var i = 1; i < passengers.Count - 1; i++)
				passengers[i].GetHit();
		}

		if (!_health.IsDead())
		{
			transform.DOShakeRotation(1.5f, 15f, 5);
			return;
		}
		
		HeliDeath();
		Vibration.Vibrate(20);
	}

	private void ThrowPassenger(HelicopterSoldierController passenger)
	{
		passenger.transform.parent = null;
		passenger.GoRagdoll((passenger.transform.position - transform.position).normalized);
	}

	private void StartSoldierShooterSequence()
	{
		_mySeq = DOTween.Sequence();

		foreach (var soldier in passengers)
		{
			_mySeq.AppendCallback(ResetProgressValue);
			_mySeq.Append(DOTween.To(() => ProgressBarValue, value => ProgressBarValue = value, 1f, shootInterval));
			_mySeq.AppendCallback(soldier.Shoot);
		}

		_mySeq.AppendCallback(StartSoldierShooterSequence);
	}

	private void EndShooterSequence()
	{
		_mySeq.Kill();
		progressBarUi.transform.parent.gameObject.SetActive(false);
	}

	private void ResetProgressValue() => ProgressBarValue = 0f;

	private void GoToStartPoint()
	{
		transform.DOMove(startTransform.position, 3.5f).SetEase(easeCurve);
		transform.DORotateQuaternion(startTransform.rotation, 3.5f).SetEase(easeCurve).OnComplete(StartSoldierShooterSequence);
	}
	
	private void TakeCareOfThis(Transform victim)
	{
		victim.DOScale(Vector3.zero, 1f).OnComplete(() => victim.gameObject.SetActive(false));
		GetHit();
	}

	private void OnCollisionEnter(Collision other)
	{
		if(_isDead) return;
		if(!other.collider.CompareTag("Target")) return;
		
		var exploder = Instantiate(explosionPrefab, other.contacts[0].point,
			Quaternion.LookRotation(other.contacts[0].normal));

		if(other.collider.CompareTag("Giant")) return;
		
		exploder.transform.localScale *= explosionScale;
		Destroy(exploder, 3f);
		TakeCareOfThis(other.transform);
	}

	private void OnTapToPlay()
	{
		if(0 != myArea) return;
		 
		GoToStartPoint();
	}

	private void OnReachNextArea()
	{
		if(LevelFlowController.only.currentArea != myArea) return;
		
		_health.VisibilityToggle(false);
		GoToStartPoint();
	}
}