using DG.Tweening;
using UnityEngine;

public class HandController : MonoBehaviour
{
	public bool isLeftHand;
	public Transform palm;
	[SerializeField] private float moveSpeed, returnSpeed, punchForce;

	private Animator _anim;
	private static RopeController _rope;
	private static Animation _armAnimation;

	private static bool _isCarryingRagdoll;
	private static bool _isCarryingBody;
	
	private Transform _palmParentInit;
	private Quaternion _ropeEndInitRot, _lastNormal;
	private Vector3 _ropeEndInitPos, _lastHitPoint;
	private bool _isHandMoving;
	private static readonly int IsPunching = Animator.StringToHash("isPunching");
	
	private static Vector3 _targetInitPos;
	private static bool _initPosSet;

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
		if(!_rope)
			if(TryGetComponent(out RopeController rope))
				_rope = rope;
		
		_armAnimation = transform.parent.GetComponent<Animation>();
		
		_palmParentInit = palm.parent;
		_ropeEndInitPos = palm.position;
		_ropeEndInitRot = palm.rotation;
	}

	public void MoveRopeEndTowards(RaycastHit hit, bool goHome = false)
	{
		if (goHome)
		{
			if (_isCarryingBody)
			{
				palm.root.position = 
					Vector3.MoveTowards(palm.root.position,
						transform.position + (_isCarryingRagdoll ? Vector3.down * 1.5f : Vector3.zero),
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
			if (!_initPosSet)
			{
				_targetInitPos = hit.transform.position;
				_initPosSet = true;
			}
			
			_lastHitPoint = hit.point;
			_lastNormal = Quaternion.LookRotation(-hit.normal);
			
			palm.position =
				Vector3.MoveTowards(palm.position,
				_lastHitPoint,
				 moveSpeed * Time.deltaTime);

			palm.rotation = 
				Quaternion.Lerp(palm.rotation, _lastNormal, moveSpeed * 1.5f * Time.deltaTime);
		}
	}
	
	public void UpdateRope() => _rope.UpdateRope();

	public void HandReachTarget(Transform other)
	{
		if (_isCarryingBody) return;
		
		if(isLeftHand)
		{
			if (!InputHandler.Only.CanSwitchToTargetState()) return;
			InputHandler.AssignNewState(new OnTargetState(other.transform, _lastHitPoint));
			palm.DOLocalMove(Vector3.forward * .5f, 0.5f);
		}
		else
		{
			InputHandler.AssignNewState(new InTransitState(true, InputStateBase.EmptyHit, false));
			if(_isCarryingRagdoll)
				other.GetComponent<RagdollLimbController>().GetPunched((other.position - transform.position).normalized, punchForce);
			else
			{
				_targetInitPos.y = other.position.y;
				other.root.GetComponent<BarrelController>()
					.GetPunched((_targetInitPos - other.root.position).normalized, punchForce);
			}
		}
	}

	public void HandReachHome()
	{
		InputHandler.AssignNewState(InputHandler.IdleState);
	}

	public void StartCarryingBody(Transform target)
	{
		_isCarryingBody = true;
		
		if(target.TryGetComponent(out RagdollLimbController raghu))
		{
			palm.parent = raghu.AskParentForHook().transform;
			_isCarryingRagdoll = true;
		}
		else
		{
			palm.parent = target.transform;
			_isCarryingRagdoll = false;
		}
	}

	public void StopCarryingBody()
	{
		ResetPalmParent();
	}

	public void WaitForPunch(Transform other, float zPos)
	{
		if (!other) return;
		
		var root = other.root;
		root.DOMove(new Vector3(
			0f,
			_isCarryingRagdoll ? 1f : 3f,
			zPos + 0.5f * (_isCarryingRagdoll ? 1f : -1f)), .2f);
		_anim.SetBool(IsPunching, true);
		_rope.ReturnHome();
		
		InputHandler.Only.StopCarryingBody();
	}

	public void GivePunch()
	{
		_armAnimation.Play();
	}

	private static void ClearInitTargetPos()
	{
		_initPosSet = false;
	}
	
	public void StopPunching()
	{
		_anim.SetBool(IsPunching, false);
		ClearInitTargetPos();
	}

	private void ResetPalmParent()
	{
		if(!isLeftHand) return;
		
		palm.parent = _palmParentInit;
		palm.DOMove(_ropeEndInitPos, 0.2f);
		palm.DORotateQuaternion(_ropeEndInitRot, 0.2f);
		_isCarryingBody = false;
	}

	private void OnEnterHitBox(Transform target)
	{
		if(!isLeftHand) return;

		ResetPalmParent();
	}

	public AimController GetAimController()
	{
		return transform.root.GetComponent<AimController>();
	}
}