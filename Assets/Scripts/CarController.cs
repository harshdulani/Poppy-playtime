using DG.Tweening;
using UnityEngine;

public class CarController : MonoBehaviour
{
	public bool shouldMove, isRightLane;
	
	[SerializeField] private float movementSpeed;
	[SerializeField] private Transform laneStartPoint;

	private GameObject _trail;
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
	
	public void DropVehicle()
	{
		_rb.isKinematic = false;
		_rb.useGravity = true;
		transform.parent = null;
		_trail.SetActive(false);
	}

	public void AddTrail(GameObject trailPrefab)
	{
		_trail = Instantiate(trailPrefab, transform.position, transform.rotation);
		_trail.transform.parent = transform;
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("CarLaneEnd")) return;

		var position = laneStartPoint.position - Vector3.right * _bounds.extents.x * (isRightLane ? 1f : -1f);

		position.y = 0f;
		transform.position = position;
	}

	private void OnCollisionEnter(Collision other)
	{
		if(!CompareTag("EnemyAttack")) return;
		
		if(!other.gameObject.CompareTag("HitBox") && !other.gameObject.CompareTag("Arm") && !other.gameObject.CompareTag("Player")) return;
		
		GameEvents.Only.InvokeEnemyHitPlayer(transform);
		
		//smoke vfx
		transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => gameObject.SetActive(false));
	}
}
