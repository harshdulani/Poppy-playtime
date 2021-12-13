using UnityEngine;

public class HandController : MonoBehaviour
{
	[SerializeField] private bool isLeftHand;
	
	[SerializeField] private Transform ropeBegin, ropeEnd;
	[SerializeField] private float moveSpeed;

	private Quaternion _ropeEndInitRot;
	private Vector3 _ropeEndInitPos;
	private bool _isHandMoving;
	
	private void Start()
	{
		_ropeEndInitPos = ropeEnd.position;
		_ropeEndInitRot = ropeEnd.rotation;
	}

	public void MoveRopeEndTowards(Vector3 hitPoint, Vector3 normal, bool goHome = false)
	{
		//_currentDirection = ((goHome ? _ropeEndInitPos : hitPoint) - ropeEnd.position).normalized;
		
		//ropeEnd.position += _currentDirection * (moveSpeed * (goHome ? 2f : 1f) * Time.deltaTime);
		
		ropeEnd.position = Vector3.MoveTowards(ropeEnd.position, goHome ? _ropeEndInitPos : hitPoint, moveSpeed * (goHome ? 3f : 1f) * Time.deltaTime);
		
		ropeEnd.rotation = goHome ? 
			Quaternion.Lerp(ropeEnd.rotation, _ropeEndInitRot, moveSpeed * 3f * Time.deltaTime) : 
			Quaternion.Lerp(ropeEnd.rotation, Quaternion.LookRotation(-normal), moveSpeed * 1.5f * Time.deltaTime);
		//also care for rotation
	}

	public void HandReachTarget(Transform other)
	{
		
		
		InputHandler.AssignNewState(new OnTargetState(other.transform));
	}

	public void HandReachHome()
	{
		print("home");
		InputHandler.AssignNewState(InputHandler.IdleState);
	}
}