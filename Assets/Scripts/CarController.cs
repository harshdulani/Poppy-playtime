using UnityEngine;

public class CarController : MonoBehaviour
{
	public bool shouldMove, isRightLane;
	
	[SerializeField] private float movementSpeed;
	[SerializeField] private Transform laneStartPoint;
	private Bounds _bounds;
	
	private void Start()
	{
		_bounds = GetComponent<Renderer>().bounds;
	}

	private void Update()
	{
		if(shouldMove)
			transform.position += transform.forward * (movementSpeed * Time.deltaTime);
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
