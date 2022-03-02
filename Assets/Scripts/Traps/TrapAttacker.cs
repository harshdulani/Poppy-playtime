using UnityEngine;

public class TrapAttacker : MonoBehaviour
{
	[SerializeField] private Transform desiredPosition;

	private Rigidbody _rb;

	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (desiredPosition)
			_rb.MovePosition(desiredPosition.position);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!other.collider.CompareTag("Target")) return;

		if (!other.gameObject.TryGetComponent(out RagdollLimbController raghu)) return;

		var dir = other.transform.position - transform.position;

		raghu.GetPunched(dir, dir.magnitude);
	}
}