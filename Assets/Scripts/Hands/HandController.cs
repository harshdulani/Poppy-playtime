using UnityEngine;

public class HandController : MonoBehaviour
{
	[SerializeField] private bool isLeftHand;
	
	public Transform palm;
	[SerializeField] private float moveSpeed, returnSpeed;

	private Quaternion _ropeEndInitRot, _lastNormal;
	private Vector3 _ropeEndInitPos, _lastHitPoint;
	private bool _isHandMoving;
	
	private void Start()
	{
		_ropeEndInitPos = palm.position;
		_ropeEndInitRot = palm.rotation;
	}

	public void MoveRopeEndTowards(Vector3 hitPoint, Vector3 normal, bool goHome = false)
	{
		_lastHitPoint = hitPoint;
		_lastNormal = Quaternion.LookRotation(-normal);

		palm.position = Vector3.MoveTowards(palm.position, goHome ? _ropeEndInitPos : _lastHitPoint, (goHome ? returnSpeed : moveSpeed) * Time.deltaTime);

		palm.rotation = goHome ? 
			Quaternion.Lerp(palm.rotation, _ropeEndInitRot, returnSpeed * Time.deltaTime) : 
			Quaternion.Lerp(palm.rotation, _lastNormal, moveSpeed * 1.5f * Time.deltaTime);
	}

	public void HandReachTarget(Transform other)
	{
		InputHandler.AssignNewState(new OnTargetState(other.transform, _lastHitPoint));
	}

	public void HandReachHome()
	{
		InputHandler.AssignNewState(InputHandler.IdleState);
	}
}