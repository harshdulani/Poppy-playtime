using UnityEngine;

public class OnTargetState : InputStateBase
{
	private static Camera _camera;
	private static float _dragForce;
	
	private Rigidbody _rb;
	private Transform _target;
	
	private Vector3 _originalRbPos, _hitPoint;
	private float _dist;

	public OnTargetState(float dragForce, Camera camera)
	{
		_dragForce = dragForce;
		_camera = camera;
	}
	
	public OnTargetState(Transform target, Vector3 lastHitPoint)
	{
		_target = target;
		_hitPoint = lastHitPoint;
	}

	public override void OnEnter()
	{
		IsPersistent = false;

		_originalRbPos = _target.position;
		
		LeftHand.StartCarryingBody(_target);
		
		_dist = Vector3.Distance(_hitPoint, _camera.transform.position);
		
		if(_target.TryGetComponent(out RagdollLimbController raghu))
			raghu.TellParent();
	}

	public override void FixedExecute()
	{
		base.FixedExecute();
		if (!_rb) return;
		
		var mousePositionOffset = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _dist)) - _hitPoint;
		_rb.velocity = (_originalRbPos + mousePositionOffset - _rb.position) * (_dragForce * Time.deltaTime);
	}

	public override void OnExit()
	{
		base.OnExit();
		
		_target = null;

		_rb = null;
	}
}