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

	public static bool _isCarryingRagdoll;
	private static bool IsCarryingBody;
	
	private Transform _palmParentInit,_lastTarget;
	private Quaternion _palmInitLocalRot, _lastNormal;
	private Vector3 _palmInitLocalPos, _lastOffset;
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

		_initPosSet = false;
		_palmParentInit = palm.parent;
		_palmInitLocalPos = palm.localPosition;
		_palmInitLocalRot = palm.localRotation;
	}

	public void MoveRopeEndTowards(RaycastHit hit, bool goHome = false)
	{
		if (goHome)
		{
			if (IsCarryingBody)
			{
				var direction = (_palmParentInit.position - palm.position).normalized;
				//palm.root.position += direction * (returnSpeed * Time.deltaTime);
				palm.localPosition =
					Vector3.MoveTowards(palm.localPosition, Vector3.zero, returnSpeed * Time.deltaTime);

				return;
			}
			
			palm.localPosition = 
				Vector3.MoveTowards(palm.localPosition,
				Vector3.zero, 
				returnSpeed * Time.deltaTime);

			palm.localRotation =
				Quaternion.Lerp(palm.localRotation, _palmInitLocalRot, returnSpeed * Time.deltaTime);
		}
		else
		{
			if (!_initPosSet)
			{
				_targetInitPos = hit.transform.position;
				_lastTarget = hit.transform;
				_lastOffset = hit.point - hit.transform.position;
				_lastNormal = Quaternion.LookRotation(-hit.normal);

				_initPosSet = true;
			}
			
			palm.position =
				Vector3.MoveTowards(palm.position,
				_lastTarget.position + _lastOffset,
				 moveSpeed * Time.deltaTime);

			palm.rotation = 
				Quaternion.Lerp(palm.rotation, _lastNormal, moveSpeed * 1.5f * Time.deltaTime);
		}
	}
	
	public static void UpdateRope() => _rope.UpdateRope();

	public void HandReachTarget(Transform other)
	{
		if (IsCarryingBody) return;
		
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
				other.GetComponent<RagdollLimbController>().GetPunched((_targetInitPos - transform.position).normalized, punchForce);
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
		IsCarryingBody = true;
		
		if(target.TryGetComponent(out RagdollLimbController raghu))
		{
			raghu.AskParentForHook().transform.root.parent = palm;
			_isCarryingRagdoll = true;
		}
		else
		{
			target.transform.parent = palm;
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
		
		InputHandler.Only.StopCarryingBody();
		
		var root = other.root;

		var direction = (root.position - transform.position).normalized;
		
		var endValue = transform.position + direction * (_isCarryingRagdoll ? ragdollWfpDistance : propWfpDistance) + transform.up.normalized * (_isCarryingRagdoll ? -.5f : 1f);

		root.DOMove(endValue, 0.2f);
		root.DORotateQuaternion(Quaternion.LookRotation(-direction), 0.2f);
		
		_anim.SetBool(IsPunching, true);
		_rope.ReturnHome();
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
		
		if(palm.childCount > 2)
			palm.GetChild(2).parent = null;

		//palm.parent = _palmParentInit;
		palm.DOLocalMove(Vector3.zero, 0.2f).OnComplete(() => palm.localPosition = _palmInitLocalPos);
		palm.DOLocalRotateQuaternion(_palmInitLocalRot, 0.2f).OnComplete(() => palm.localRotation =_palmInitLocalRot);
		IsCarryingBody = false;
	}

	private void OnEnterHitBox(Transform target)
	{
		if(!isLeftHand) return;

		ResetPalmParent();
	}

	private void OnPropDestroyed(Transform target)
	{
		IsCarryingBody = false;
		ResetPalmParent();
		ClearInitTargetPos();
	}

	public AimController GetAimController()
	{
		return transform.root.GetComponent<AimController>();
	}
}