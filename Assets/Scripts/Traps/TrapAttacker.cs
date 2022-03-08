using UnityEngine;

public class TrapAttacker : MonoBehaviour
{
	public bool isEnabled;
	[SerializeField] private bool isAlwaysEnabled = true;
	
	[SerializeField] private Transform desiredPosition;

	private Rigidbody _rb;

	private void Start()
	{
		_rb = GetComponent<Rigidbody>();

		isEnabled = isAlwaysEnabled;
	}

	private void Update()
	{
		if (desiredPosition)
			_rb.MovePosition(desiredPosition.position);
	}

	private void OnCollisionEnter(Collision other)
	{
		if(!isEnabled) return;
		
		if (!other.collider.CompareTag("Target")) return;
		
		if (!other.gameObject.TryGetComponent(out RagdollLimbController raghu)) return;

		var dir = other.transform.position - transform.position;

		raghu.GetPunched(dir, dir.magnitude);
	}

	public void MakeNonKinematic(Vector3 direction)
	{
		_rb.isKinematic = false;
		_rb.AddForce(direction * 0f, ForceMode.Impulse);
	}
}