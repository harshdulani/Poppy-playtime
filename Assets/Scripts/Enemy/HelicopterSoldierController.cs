using UnityEngine;

public class HelicopterSoldierController : MonoBehaviour
{
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private Transform bulletHole;
	[SerializeField] private Rigidbody[] rigidbodies;
	
	private Animator _anim;
	private AudioSource _audioSource;
	
	private bool _isRagdoll;

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
			rb.AddForce(direction * 10f, ForceMode.Impulse);A
		}

		Invoke(nameof(GoKinematic), 4f);
		
		GameEvents.only.InvokeEnemyKill();

		foreach (var rb in rigidbodies)
			rb.tag = "Untagged";
		
		_audioSource.Play();
		Vibration.Vibrate(25);
	}
	
	private void GoKinematic()
	{
		foreach (var rb in rigidbodies)
			rb.isKinematic = false;
	}

	private void Shoot()
	{
		Instantiate(bulletPrefab, bulletHole.position, bulletHole.rotation);
	}
}