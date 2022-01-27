using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HelicopterController : MonoBehaviour
{
	[SerializeField] private int myArea;
	[SerializeField] private Transform startTransform;
	
	[Header("Sine Wave Noise"), SerializeField] private float magnitude;
	[SerializeField] private float rotationMagnitude, recenterLerp;
	private Vector3 _previousSine, _previousSineRot;

	[Header("Damage"), SerializeField] private List<Transform> passengers;
	[SerializeField] private GameObject explosionPrefab;
	[SerializeField] private float deathExplosionForce, explosionScale;
	private HealthController _health;
	
	private Rigidbody _rb;
	private bool _isDead;

	private void OnEnable()
	{
		GameEvents.only.reachNextArea += OnReachNextArea;
	}

	private void OnDisable()
	{
		GameEvents.only.reachNextArea -= OnReachNextArea;
	}

	private void Start()
	{
		_health = GetComponent<HealthController>();
		_rb = GetComponent<Rigidbody>();
		_health.VisibilityToggle(false);
		
		OnReachNextArea();
	}

	private void Update()
	{
		if(_isDead) return;
		
		SineWave();
		//Recenter();
	}

	private void Recenter()
	{
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.back), Time.deltaTime * recenterLerp);
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
		_rb.AddTorque(Vector3.up * 360f, ForceMode.Acceleration);
	}

	private void GetHit(Transform hitter)
	{
		if(!_health.AddHit(hitter)) return;
		
		Vibration.Vibrate(20);

		if (passengers.Count > 0)
		{
			ThrowPassenger(passengers[0]);
			passengers.RemoveAt(0);
		}

		if (!_health.IsDead()) return;
		
		HeliDeath();
	}

	private static void ThrowPassenger(Transform passenger)
	{
		passenger.transform.parent = null;
		var rb = passenger.GetComponent<Rigidbody>();
		rb.isKinematic = false;
		rb.useGravity = true;
		rb.AddForce(rb.transform.up * 12f);
	}
	
	private void GoToStartPoint()
	{
		transform.DOMove(startTransform.position, 1.5f);
		transform.DORotateQuaternion(startTransform.rotation, 1.5f);
	}
	
	private void TakeCareOfThis(Transform victim)
	{
		victim.DOScale(Vector3.zero, 1f).OnComplete(() => victim.gameObject.SetActive(false));
		GetHit(victim);
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

	private void OnReachNextArea()
	{
		if(LevelFlowController.only.currentArea != myArea) return;
		
		_health.VisibilityToggle(false);
		GoToStartPoint();
	}
}