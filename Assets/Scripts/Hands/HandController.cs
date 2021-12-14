using UnityEngine;

public class HandController : MonoBehaviour
{
	[SerializeField] private bool isLeftHand;
	
	public Transform palm, wrist;
	[SerializeField] private float moveSpeed, returnSpeed, returnBodyDragForce;

	private Rigidbody _rb;
	private Quaternion _ropeEndInitRot, _lastNormal;
	private Vector3 _ropeEndInitPos, _lastHitPoint, _bodyDragDirection;
	private bool _isHandMoving, _isCarryingBody;
	
	private void Start()
	{
		
		_ropeEndInitPos = palm.position;
		_ropeEndInitRot = palm.rotation;
	}

	public void MoveRopeEndTowards(Vector3 hitPoint, Vector3 normal, bool goHome = false)
	{
		if (goHome)
		{
			if (_isCarryingBody)
			{
				_rb.velocity = (wrist.position - _rb.position + Vector3.up * 3f).normalized * (returnBodyDragForce * Time.deltaTime);
				print(_rb.velocity);
				return;
			}

			palm.position = 
				Vector3.MoveTowards(palm.position,
				_ropeEndInitPos,
				returnSpeed * Time.deltaTime);

			palm.rotation =
				Quaternion.Lerp(palm.rotation, _ropeEndInitRot, returnSpeed * Time.deltaTime);
		}
		else
		{
			_lastHitPoint = hitPoint;
			_lastNormal = Quaternion.LookRotation(-normal);
			
			palm.position =
				Vector3.MoveTowards(palm.position,
				_lastHitPoint,
				 moveSpeed * Time.deltaTime);

			palm.rotation = 
				Quaternion.Lerp(palm.rotation, _lastNormal, moveSpeed * 1.5f * Time.deltaTime);
		}
	}

	public void HandReachTarget(Transform other)
	{
		if (_isCarryingBody) return;
		InputHandler.AssignNewState(new OnTargetState(other.transform, _lastHitPoint));
	}

	public void HandReachHome()
	{
		InputHandler.AssignNewState(InputHandler.IdleState);
	}

	public void StartCarryingBody(Transform target)
	{
		_isCarryingBody = true;
		
		palm.parent = target;
		_rb = target.GetComponent<Rigidbody>();
		
		_bodyDragDirection = (wrist.position - _rb.position).normalized;
	}
}