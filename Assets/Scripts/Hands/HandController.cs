using DG.Tweening;
using UnityEngine;

public class HandController : MonoBehaviour
{
	[SerializeField] private bool isLeftHand;
	public Transform palm, wrist;
	[SerializeField] private float moveSpeed, returnSpeed, returnBodyDragForce, punchForce;

	private Animator _anim;
	private static Animation _parentAnimation;
	
	private Transform _palmParentInit;
	private Rigidbody _rb;
	private Quaternion _ropeEndInitRot, _lastNormal;
	private Vector3 _ropeEndInitPos, _lastHitPoint, _bodyDragDirection;
	private bool _isHandMoving, _isCarryingBody;
	private static readonly int IsPunching = Animator.StringToHash("isPunching");

	private void OnEnable()
	{
		GameEvents.only.punch += OnPunch;
	}
	
	private void OnDisable()
	{
		GameEvents.only.punch -= OnPunch;
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_parentAnimation = transform.parent.GetComponent<Animation>();
		
		_palmParentInit = palm.parent;
		_ropeEndInitPos = palm.position;
		_ropeEndInitRot = palm.rotation;
	}

	public void MoveRopeEndTowards(Vector3 hitPoint, Vector3 normal, bool goHome = false)
	{
		if (goHome)
		{
			if (_isCarryingBody)
			{
				palm.root.position = 
					Vector3.MoveTowards(palm.root.position,
						transform.position + Vector3.down * 1.5f,
						returnSpeed * Time.deltaTime);

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
		
		if(isLeftHand)
		{
			if(!InputHandler.Only.CanSwitchToTargetState()) return;
			
			InputHandler.AssignNewState(new OnTargetState(other.transform, _lastHitPoint));
			//palm.DOMove(other.GetComponent<RagdollLimbController>().AskParentForHook().position, 0.5f);
			palm.DOLocalMove(Vector3.forward * .5f, 0.5f);
		}
		else
		{
			InputHandler.AssignNewState(new InTransitState(true, InputStateBase.EmptyHit, false));
			other.GetComponent<RagdollLimbController>().GetPunched((other.position - transform.position).normalized, punchForce);
		}
	}

	public void HandReachHome()
	{
		InputHandler.AssignNewState(InputHandler.IdleState);
	}

	public void StartCarryingBody(Transform target)
	{
		_isCarryingBody = true;
		
		palm.parent = target.GetComponent<RagdollLimbController>().AskParentForHook().transform;
		_rb = target.GetComponent<Rigidbody>();
		
		_bodyDragDirection = (wrist.position - _rb.position + Vector3.up).normalized;
	}

	private void StopCarryingBody()
	{
		if (!_rb) return;
		
		_rb.velocity *= -4f;
		_rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);

		ResetPalmParent();
	}

	public void DeliverPunch(Transform other)
	{
		if (!other) return;

		_parentAnimation.Play();
		_anim.SetBool(IsPunching, true);
		//Debug.Break();
		//tell left hand to stop pulling
		
		InputHandler.Only._leftHand.StopCarryingBody();
		
		//transform.DOMove(other.position - Vector3.back, 1f);
	}

	public void StopPunching()
	{
		_anim.SetBool(IsPunching, false);
	}

	private void ResetPalmParent()
	{
		if(!isLeftHand) return;
		
		palm.parent = _palmParentInit;
		_rb = null;
		_isCarryingBody = false;
	}

	private void OnPunch()
	{
		if(!isLeftHand) return;

		ResetPalmParent();
	}
}