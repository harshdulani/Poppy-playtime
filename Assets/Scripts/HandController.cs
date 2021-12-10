using UnityEngine;

public class HandController : MonoBehaviour
{
	[SerializeField] private Transform ropeBegin, ropeEnd;
	
	[SerializeField] private float moveSpeed;
	[SerializeField] private bool isLeftHand;
	[SerializeField] private float handTravelTime;

	private Vector3 _ropeEndInitPos;
	private bool _isHandMoving;
	
	//hold mechanic enum here

	private void Start()
	{
		_ropeEndInitPos = ropeEnd.position;
	}

	public void MoveRopeEndTowards(RaycastHit hit, bool goHome = false)
	{
		ropeEnd.position = Vector3.Lerp(ropeEnd.position, goHome ? _ropeEndInitPos : hit.point, moveSpeed * Time.deltaTime);
		//also care for rotation
	}

	private void OnCollisionEnter(Collision other)
	{
		if(!other.collider.CompareTag("Hittable")) return;
		//optionally add a comparer for if this is the target but that would be unnatural, and unnecesarry because the guy on the front would be what we clicked on regardless
		
		//hittable.makeragdoll(transform)
		InputHandler.AssignNewState(new OnTargetState(other.transform));
	}

	private void OnTriggerEnter(Collider other)
	{
		if(!other.CompareTag("Arm")) return;
		
		InputHandler.AssignNewState(InputHandler.IdleState);
	}
}

//hold mechanics as classes here, refer Command Pattn repo