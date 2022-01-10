using System;
using DG.Tweening;
using UnityEngine;

public enum CarriedObjectType
{
	Ragdoll, Prop, Car
}

public class HandController : MonoBehaviour
{
	public bool isLeftHand;
	public Transform palm;
	[SerializeField] private float moveSpeed, returnSpeed, punchForce;
	[SerializeField] private float ragdollWfpDistance, propWfpDistance, carWfpDistance, enemyWfpHeight = -0.5f, carWfpHeight;

	[SerializeField] private ParticleSystem windLines;
	
	private Animator _anim;
	private static Animation _armAnimation;
	private static RopeController _rope;
	public static PlayerSoundController Sounds;

	public static CarriedObjectType CurrentObjectCarriedType;
	private static bool _isCarryingBody;
	
	private Transform _lastTarget;
	private Quaternion _palmInitLocalRot, _lastNormal;
	private Vector3 _palmInitLocalPos, _lastOffset;
	private bool _isHandMoving, _canGivePunch;
	
	private static Vector3 _targetInitPos;
	private static bool _initPosSet;
	
	private static readonly int IsPunching = Animator.StringToHash("isPunching");

	private void OnEnable()
	{
		if(!isLeftHand)
			GameEvents.only.enterHitBox += OnEnterHitBox;

		GameEvents.only.punchHit += OnPunchHit;
	}
	
	private void OnDisable()
	{
		if (!isLeftHand)
			GameEvents.only.enterHitBox -= OnEnterHitBox;
		
		GameEvents.only.punchHit -= OnPunchHit;
	}
	
	private void Start()
	{
		_anim = GetComponent<Animator>();
		if(!_rope) //if static variables aren't initialised
		{
			if (TryGetComponent(out RopeController rope))
				_rope = rope;

			_armAnimation = transform.root.GetComponent<Animation>();
			Sounds = _armAnimation.GetComponent<PlayerSoundController>();
		}

		_initPosSet = false;
		_palmInitLocalPos = palm.localPosition;
		_palmInitLocalRot = palm.localRotation;
	}

	public void MoveRopeEndTowards(RaycastHit hit, bool goHome = false)
	{
		if (goHome)
		{
			if (_isCarryingBody)
			{
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
				
				Sounds.PlaySound(Sounds.ziplineLeave, 1f);
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
		if (_isCarryingBody) return;
		
		if(isLeftHand)
		{
			if (!InputHandler.Only.CanSwitchToTargetState()) return;
			
			StartCarryingBody(other.transform);
			if(other.transform.TryGetComponent(out RagdollLimbController raghu))
				raghu.TellParent();
			else if (CurrentObjectCarriedType == CarriedObjectType.Car)
			{
				other.GetComponent<CarController>().StopMoving();
				print("here");
			}
			else if (other.TryGetComponent(out PropController prop))
				prop.hasBeenInteractedWith = true;
			
			InputHandler.AssignNewState(new InTransitState(true, InputStateBase.EmptyHit, 
				true));
		}
		else
		{
			InputHandler.AssignNewState(new InTransitState(true, InputStateBase.EmptyHit, false));
			if(CurrentObjectCarriedType == CarriedObjectType.Ragdoll)
				other.GetComponent<RagdollLimbController>().GetPunched((_targetInitPos - transform.position).normalized, punchForce);
			else if(CurrentObjectCarriedType == CarriedObjectType.Prop)
			{
				_targetInitPos.y = other.position.y;
				other.root.GetComponent<PropController>()
					.GetPunched((_targetInitPos - other.root.position).normalized, punchForce * (CurrentObjectCarriedType == CarriedObjectType.Car ? 2f : 1f));
			}
			else
			{ 
               other.root.GetComponent<PropController>()
                	.GetPunched((GameObject.FindGameObjectWithTag("Giant").GetComponentInChildren<Renderer>().bounds.center - other.root.position).normalized, punchForce * (CurrentObjectCarriedType == CarriedObjectType.Car ? 2f : 1f));
			}
		}

		Vibration.Vibrate(15);
	}

	public void HandReachHome()
	{
		if(!InputHandler.Only.IsInDisabledState())
			InputHandler.AssignNewState(InputHandler.IdleState);
	}

	private void StartCarryingBody(Transform target)
	{
		_isCarryingBody = true;
		
		if(target.TryGetComponent(out RagdollLimbController raghu))
		{
			raghu.AskParentForHook().transform.root.parent = palm;
			CurrentObjectCarriedType = CarriedObjectType.Ragdoll;
		}
		else
		{
			target.transform.parent = palm;
			CurrentObjectCarriedType = target.TryGetComponent(out CarController _) ? CarriedObjectType.Car : CarriedObjectType.Prop;
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

		float distance, height;

		switch (CurrentObjectCarriedType)
		{
			case CarriedObjectType.Ragdoll:
				distance = ragdollWfpDistance;
				height = enemyWfpHeight;
				break;
			case CarriedObjectType.Prop:
				distance = propWfpDistance;
				height = 1f;
				break;
			case CarriedObjectType.Car:
				distance = carWfpDistance;
				height = carWfpHeight;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		
		var endValue = transform.position + direction * distance + transform.up * height;

		root.DOMove(endValue, 0.2f);
		if(CurrentObjectCarriedType == CarriedObjectType.Ragdoll)
			root.DORotateQuaternion(Quaternion.LookRotation(-direction), 0.2f);
		
		_anim.SetBool(IsPunching, true);
		_canGivePunch = true;
		_rope.ReturnHome();
		windLines.Play();
	}

	public void GivePunch()
	{
		if (!_canGivePunch) return;
		
		_canGivePunch = false;
		_armAnimation.Play();
		Sounds.PlaySound(Sounds.clickForPunch, 1);
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
		_isCarryingBody = false;
	}

	
	public AimController GetAimController() => transform.root.GetComponent<AimController>();
	
	private void OnEnterHitBox(Transform target)
	{
		if(!isLeftHand) return;

		ResetPalmParent();
	}

	public void OnPropDestroyed()
	{
		_isCarryingBody = false;
		ResetPalmParent();
		ClearInitTargetPos();
	}
	
	private void OnPunchHit()
	{
		windLines.Stop();
	}
}