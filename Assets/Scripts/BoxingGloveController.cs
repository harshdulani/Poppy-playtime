using UnityEngine;

public class BoxingGloveController : MonoBehaviour
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
		
		var dir = other.transform.position - transform.position;
		other.gameObject.GetComponent<RagdollLimbController>().GetPunched(dir, dir.magnitude);
	}
}