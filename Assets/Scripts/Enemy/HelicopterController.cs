using System.Collections.Generic;
using UnityEngine;

public class HelicopterController : MonoBehaviour
{
	[Header("Sine wave Noise"), SerializeField] private float magnitude;
	[SerializeField] private float rotationMagnitude, recenterLerp;
	private Vector3 _previousSine, _previousSineRot;

	[Header("Damage"), SerializeField] private List<Transform> passengers;
	[SerializeField] private float deathExplosionForce;
	private HealthController _health;
	private Rigidbody _rb;
	private bool _isDead;

	private void Start()
	{
		_health = GetComponent<HealthController>();
		_health.VisibilityToggle(false);
	}

	private void Update()
	{
		if(_isDead) return;
		
		SineWave();
		Recenter();
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

		_rb.AddForce(Vector3.left * 5f * deathExplosionForce + Vector3.down * deathExplosionForce + Vector3.back * 40f * deathExplosionForce, ForceMode.Impulse);
		_rb.AddTorque(Vector3.up * 180f, ForceMode.Acceleration);
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

	private void OnCollisionEnter(Collision other)
	{
		if(_isDead) return;
		if(!other.collider.CompareTag("Target")) return;
		
		GetHit(other.transform);
	}
}