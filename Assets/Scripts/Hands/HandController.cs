using DG.Tweening;
using UnityEngine;

public class HandController : MonoBehaviour
{
	public bool isLeftHand;
	public Transform palm;
	[SerializeField] private float moveSpeed, returnSpeed, punchForce;
	[SerializeField] private float ragdollWfpDistance, propWfpDistance;
	
	private Animator _anim;
	private static RopeController _rope;
	private static Animation _armAnimation;

	private static bool _isCarryingRagdoll;
	private static bool _isCarryingBody;
	
	private Transform _palmParentInit;
	private Quaternion _palmInitLocalRot, _lastNormal;
	private Vector3 _palmInitLocalPos, _lastHitPoint;
	private bool _isHandMoving;
	private static readonly int IsPunching = Animator.StringToHash("isPunching");
	
	private static Vector3 _targetInitPos;
	private static bool _initPosSet;

	private void OnEnable()
	{
		if(!isLeftHand)
			GameEvents.only.enterHitBox += OnEnterHitBox;
		else
			GameEvents.only.propDestroyed += OnPropDestroyed;
	}
	
	private void OnDisable()
	{
		if (!isLeftHand)
			GameEvents.only.enterHitBox -= OnEnterHitBox;
		else
			GameEvents.only.propDestroyed -= OnPropDestroyed;
	}
	
	private void Start()
	{
		_anim = GetComponent<Animator>();
		if(!_rope)
			if(TryGetComponent(out RopeController rope))
				_rope = rope;
		
		_armAnimation = transform.root.GetComponent<Animation>();
		
		_palmParentInit = palm.parent;
		_palmInitLocalPos = palm.localPosition;
		_palmInitLocalRot = palm.localRotation;
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
			
			palm.localPosition = 
				Vector3.MoveTowards(palm.localPosition,
				_palmInitLocalPos,
				returnSpeed * Time.deltaTime);

			palm.localRotation =
				Quaternion.Lerp(palm.localRotation, _palmInitLocalRot, returnSpeed * Time.deltaTime);
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
	
	public static void UpdateRope() => _rope.UpdateRope();

	public void HandReachTarget(Transform other)
	{
		if (_isCarryingBody) return;
		
		if(isLeftHand)
		{
			if (!InputHandler.Only.CanSwitchToTargetState()) return;
			
			StartCarryingBody(other.transform);
			if(other.transform.TryGetComponent(out RagdollLimbController raghu))
				raghu.TellParent();
			
			InputHandler.AssignNewState(new InTransitState(true, InputStateBase.EmptyHit, 
				true));
		}
		else
		{
			InputHandler.AssignNewState(new InTransitState(true, InputStateBase.EmptyHit, false));
			if(_isCarryingRagdoll)
				other.GetComponent<RagdollLimbController>().GetPunched((other.position - transform.position).normalized, punchForce);
			else
			{
				_targetInitPos.y = other.position.y;
				other.root.GetComponent<PropController>()
					.GetPunched((_targetInitPos - other.root.position).normalized, punchForce);
			}
		}
	}

	public void HandReachHome()
	{
		InputHandler.AssignNewState(InputHandler.IdleState);
	}

	private void StartCarryingBody(Transform target)
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

	public void WaitForPunch(Transform other)
	{
		if (!other) return;
		
		var root = other.root;

		var direction = (root.position - transform.position).normalized;
		
		var endValue = transform.position + direction * (_isCarryingRagdoll ? ragdollWfpDistance : propWfpDistance);
		endValue.y = transform.root.position.y + (_isCarryingRagdoll ? 1f : 3f);
		
		root.DOMove(endValue, 0.2f);
		root.DORotateQuaternion(Quaternion.LookRotation(-direction), 0.2f);
		
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
		palm.DOLocalMove(Vector3.zero, 0.2f).OnComplete(() => palm.localPosition = _palmInitLocalPos);
		palm.DOLocalRotateQuaternion(_palmInitLocalRot, 0.2f).OnComplete(() => palm.localRotation =_palmInitLocalRot);
		_isCarryingBody = false;
	}

	private void OnEnterHitBox(Transform target)
	{
		if(!isLeftHand) return;

		ResetPalmParent();
	}

	private void OnPropDestroyed(Transform target)
	{
		_isCarryingBody = false;
		ResetPalmParent();
	}

	public AimController GetAimController()
	{
		return transform.root.GetComponent<AimController>();
	}
}