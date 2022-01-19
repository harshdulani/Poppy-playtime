using System;
using DG.Tweening;
//using TMPro;
using UnityEngine;

public enum CarriedObjectType
{
	Ragdoll, Prop, Car
}

public class HandController : MonoBehaviour
{
	public bool isLeftHand;
	public Transform palm;
	[SerializeField] private float moveSpeed, returnSpeed, punchForce, carPunchForce;
	[SerializeField] private float ragdollWfpDistance, propWfpDistance, carWfpDistance, enemyWfpHeight = -0.5f, carWfpHeight, propWfpHeight;

	[SerializeField] private ParticleSystem windLines;
	
	public static CarriedObjectType CurrentObjectCarriedType;
	[SerializeField] private WeaponType currentAttackType;
	[SerializeField] private GameObject hammer, gun, boot, heel, sneaker,  shield;
	[SerializeField] private ParticleSystem fireExplosion;
	
	public static PlayerSoundController Sounds;
	
	private Animator _myAnimator;
	private static Animator _rootAnimator;
	private static RopeController _rope;
	private Transform _lastTarget;
	private float _appliedMoveSpeed, _appliedReturnSpeed;
	private static bool _isCarryingBody;

	private AudioSource _gunshot;
	private Quaternion _palmInitLocalRot, _lastNormal;
	private Vector3 _palmInitLocalPos, _lastOffset;
	private bool _isHandMoving, _canGivePunch;

	private static Vector3 _targetInitPos;
	private static bool _initPosSet;

	//private TextMeshProUGUI _text;
	//private string _testString;
	
	private static readonly int IsPunching = Animator.StringToHash("isPunching");
	private static readonly int IsHoldingHammerHash = Animator.StringToHash("isHoldingHammer");
	private static readonly int IsUsingHandsHash = Animator.StringToHash("isUsingHands");
	private static readonly int IsHoldingGunHash = Animator.StringToHash("isHoldingGun");
	private static readonly int IsHoldingFootWearHash = Animator.StringToHash("isHoldingFootwear");
	private static readonly int IsHoldingShieldHash = Animator.StringToHash("isHoldingShield");
	private static readonly int Punch = Animator.StringToHash("Punch");
	private static readonly int ChangeWeapon = Animator.StringToHash("changeWeapon");

	private void OnEnable()
	{
		if(!isLeftHand) //only for right/punching hand
			GameEvents.only.enterHitBox += OnEnterHitBox;
		
		GameEvents.only.giantPickupCar += OnGiantPickupCar;
		GameEvents.only.punchHit += OnPunchHit;
	}
	
	private void OnDisable()
	{
		if (!isLeftHand) //only for right/punching hand
			GameEvents.only.enterHitBox -= OnEnterHitBox;
		
		
		GameEvents.only.giantPickupCar -= OnGiantPickupCar;
		GameEvents.only.punchHit -= OnPunchHit;
	}
	
	private void Start()
	{
        _myAnimator = GetComponent<Animator>();

		if (isLeftHand)
				_rope = GetComponent<RopeController>();

		if(!_rootAnimator)
			_rootAnimator = transform.root.GetComponent<Animator>();
		
		Sounds = _rootAnimator.GetComponent<PlayerSoundController>();

		_initPosSet = false;
		_palmInitLocalPos = palm.localPosition;
		_palmInitLocalRot = palm.localRotation;

		if(isLeftHand)
		{
			_appliedMoveSpeed = moveSpeed * (1f + PlayerPrefs.GetInt("currentSpeedLevel", 0) / 10f);
			_appliedReturnSpeed = returnSpeed * (1f + PlayerPrefs.GetInt("currentSpeedLevel", 0) / 10f);
		}
		
		if (isLeftHand) return;

		_gunshot = fireExplosion.GetComponent<AudioSource>();

		UpdateEquippedSkin();

		//_text = GameObject.FindGameObjectWithTag("AimCanvas").transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
	}

	public void MoveRopeEndTowards(RaycastHit hit, bool goHome = false)
	{
		if (goHome)
		{
			if (_isCarryingBody)
			{
				palm.localPosition =
					Vector3.MoveTowards(palm.localPosition, Vector3.zero, _appliedReturnSpeed * Time.deltaTime);

				return;
			}
			
			palm.localPosition = 
				Vector3.MoveTowards(palm.localPosition,
				Vector3.zero, 
				returnSpeed * Time.deltaTime);

			palm.localRotation =
				Quaternion.Lerp(palm.localRotation, _palmInitLocalRot, _appliedReturnSpeed * Time.deltaTime);
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
				 _appliedMoveSpeed * Time.deltaTime);

			palm.rotation = 
				Quaternion.Lerp(palm.rotation, _lastNormal, _appliedMoveSpeed * 1.5f * Time.deltaTime);
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
			}
			if (other.TryGetComponent(out PropController prop))
				prop.hasBeenInteractedWith = true;
			
			InputHandler.AssignNewState(new InTransitState(true, InputStateBase.EmptyHit, 
				true));
		}
		else
		{
			InputHandler.AssignNewState(new InTransitState(true, InputStateBase.EmptyHit, false));
			if(CurrentObjectCarriedType == CarriedObjectType.Ragdoll)
			{
				other.GetComponent<RagdollLimbController>().GetPunched((
					(LevelFlowController.only.IsInGiantFight()
						? LevelFlowController.only.GetGiant().GetBoundsCenter() : _targetInitPos) - transform.position).normalized, punchForce);
			}
			else
			{
				_targetInitPos.y = other.position.y;
				
				if(!other.root.TryGetComponent(out PropController prop))
					prop = other.GetComponent<PropController>();

				var direction = (LevelFlowController.only.IsInGiantFight() ? 
					LevelFlowController.only.GetGiant().GetBoundsCenter() : _targetInitPos) - other.root.position;

				prop.GetPunched(direction.normalized, 
						CurrentObjectCarriedType == CarriedObjectType.Car ? carPunchForce : punchForce);
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

	public void UpdatePullingSpeed(int level)
	{
		_appliedMoveSpeed = moveSpeed * (1f + level / 10f);
		_appliedReturnSpeed = returnSpeed * (1f + level / 10f);
	}

	public void UpdateEquippedSkin(bool initialising = true)
	{
		currentAttackType = SkinLoader.only.GetSkinName();
		for (var i = 1; i < hammer.transform.parent.childCount; i++)
			hammer.transform.parent.GetChild(i).gameObject.SetActive(false);

		if(!initialising)
			_rootAnimator.SetTrigger(ChangeWeapon);
		
		switch (currentAttackType)
		{
			case WeaponType.Punch:
				_myAnimator.SetBool(IsHoldingHammerHash, false);
				_rootAnimator.SetTrigger(IsUsingHandsHash);
				break;
			case WeaponType.Hammer:
				hammer.SetActive(true);
				_myAnimator.SetBool(IsHoldingHammerHash, true);
				_rootAnimator.SetTrigger(IsHoldingHammerHash);
				break;
			case WeaponType.Gun:
				gun.SetActive(true);
				_myAnimator.SetBool(IsHoldingHammerHash, true);
				_rootAnimator.SetTrigger(IsHoldingGunHash);
				break;
			case WeaponType.Boot:
				boot.SetActive(true);
				_myAnimator.SetBool(IsHoldingHammerHash, true);
				_rootAnimator.SetTrigger(IsHoldingFootWearHash);
				break;
			case WeaponType.Heel:
				heel.SetActive(true);
				_myAnimator.SetBool(IsHoldingHammerHash, true);
				_rootAnimator.SetTrigger(IsHoldingFootWearHash);
				break;
			case WeaponType.Sneaker:
				sneaker.SetActive(true);
				_myAnimator.SetBool(IsHoldingHammerHash, true);
				_rootAnimator.SetTrigger(IsHoldingFootWearHash);
				break;
			case WeaponType.Shield:
				shield.SetActive(true);
				_myAnimator.SetBool(IsHoldingHammerHash, true);
				_rootAnimator.SetTrigger(IsHoldingShieldHash);
				break;
		}
	}

	public void WaitForPunch(Transform other)
	{
		if (!other) return;
		
		InputHandler.Only.StopCarryingBody();
		
		var root = other.root;

		var direction = (root.position - transform.root.position).normalized;

		float distance, height;

		switch (CurrentObjectCarriedType)
		{
			case CarriedObjectType.Ragdoll:
				distance = ragdollWfpDistance;
				height = enemyWfpHeight;
				break;
			case CarriedObjectType.Prop:
				distance = propWfpDistance;
				height = propWfpHeight;
				break;
			case CarriedObjectType.Car:
				distance = carWfpDistance;
				height = carWfpHeight;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		
		var endValue = transform.root.position + direction * distance + transform.root.up * height;
		
		root.DOMove(endValue, 0.2f);
		if (CurrentObjectCarriedType == CarriedObjectType.Ragdoll)
			root.DORotateQuaternion(Quaternion.LookRotation(-direction), 0.2f);

		_myAnimator.SetBool(IsPunching, true);
		_canGivePunch = true;
		_rope.ReturnHome();
		windLines.Play();
	}

	public void GivePunch()
	{
		if (!_canGivePunch) return;
		
		_canGivePunch = false;
		_rootAnimator.SetTrigger(Punch);
		if(currentAttackType == WeaponType.Gun)
		{
			fireExplosion.Play();
			_gunshot.Play();
		}
		Sounds.PlaySound(Sounds.clickForPunch, 1);
	}

	private static void ClearInitTargetPos()
	{
		_initPosSet = false;
	}
	
	public void StopPunching()
	{
		_myAnimator.SetBool(IsPunching, false);
		ClearInitTargetPos();
	}

	private void ResetPalmParent()
	{
		if(!isLeftHand) return;
		
		if(palm.childCount > 2)
			palm.GetChild(2).parent = null;
		
		palm.DOLocalMove(Vector3.zero, 0.2f).OnComplete(() => palm.localPosition = _palmInitLocalPos);
		palm.DOLocalRotateQuaternion(_palmInitLocalRot, 0.2f).OnComplete(() => palm.localRotation =_palmInitLocalRot);
		_isCarryingBody = false;
	}
	
	public AimController GetAimController() => transform.root.GetComponent<AimController>();

	private void OnGiantPickupCar(Transform car)
	{
		if (_lastTarget != car) return;
		
		_isCarryingBody = false;
		ResetPalmParent();
		ClearInitTargetPos();
		InputHandler.Only.AssignReturnTransitState();
	}
	
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
		InputHandler.Only.AssignReturnTransitState();
	}
	
	private void OnPunchHit()
	{
		windLines.Stop();
	}
}