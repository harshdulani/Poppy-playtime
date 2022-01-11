using UnityEngine;

public class CarController : MonoBehaviour
{
	public bool shouldMove, isRightLane;
	
	[SerializeField] private float movementSpeed;
	[SerializeField] private Transform laneStartPoint;
	private Bounds _bounds;
	private Rigidbody _rb;
	
	private void Start()
	{
		_bounds = GetComponent<Renderer>().bounds;
		_rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if(shouldMove)
			_rb.MovePosition(transform.position + transform.forward.normalized * (movementSpeed * Time.fixedDeltaTime));
	}

	public void StopMoving()
	{
		shouldMove = false;
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("CarLaneEnd")) return;

		transform.position = laneStartPoint.position - Vector3.right * _bounds.extents.x * (isRightLane ? 1f : -1f);
	}
}
