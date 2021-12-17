using DG.Tweening;
using UnityEngine;

public class HandController : MonoBehaviour
{
	[SerializeField] private bool isLeftHand;
	public Transform palm, wrist;
	[SerializeField] private float moveSpeed, returnSpeed, punchForce;

	private Animator _anim;
	private static Animation _armAnimation;
	
	private Transform _palmParentInit;
	private Quaternion _ropeEndInitRot, _lastNormal;
	private Vector3 _ropeEndInitPos, _lastHitPoint;
	private bool _isHandMoving, _isCarryingBody;
	private static readonly int IsPunching = Animator.StringToHash("isPunching");

	private void OnEnable()
	{
		if(!isLeftHand)
		{
			GameEvents.only.enterHitBox += OnEnterHitBox;
		}
	}
	
	private void OnDisable()
	{
		if (!isLeftHand)
		{
			GameEvents.only.enterHitBox -= OnEnterHitBox;
		}
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_armAnimation = transform.parent.GetComponent<Animation>();
		
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
	}

	private void StopCarryingBody()
	{
		ResetPalmParent();
	}

	public void WaitForPunch(Transform other, float zPos)
	{
		if (!other) return;
		
		var root = other.root;
		root.DOMove(new Vector3(0f, 1f, zPos), .2f);
		_anim.SetBool(IsPunching, true);
		
		InputHandler.Only._leftHand.StopCarryingBody();
	}

	public void GivePunch()
	{
		_armAnimation.Play();
	}
	
	public void StopPunching()
	{
		_anim.SetBool(IsPunching, false);
	}

	private void ResetPalmParent()
	{
		if(!isLeftHand) return;
		
		palm.parent = _palmParentInit;
		palm.DOMove(_ropeEndInitPos, 0.2f);
		palm.DORotateQuaternion(_ropeEndInitRot, 0.2f);
		_isCarryingBody = false;
	}

	private void OnEnterHitBox()
	{
		if(!isLeftHand) return;

		ResetPalmParent();
	}
}