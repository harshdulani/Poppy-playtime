using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
	public static InputHandler Only;

	[SerializeField] private float raycastDistance = 50f;
	public bool testingUsingTouch;
	public bool isUsingTapAndPunch, canDragAfterGrabbingToAim;
	
	[SerializeField] private float tapCooldownWaitTime;

	private HandController _leftHand, _rightHand;

	//derived states
	public static readonly IdleState IdleState = new IdleState();
	public static readonly DragToSmashState DragToSmashState = new DragToSmashState();
	private static readonly DisabledState PermanentlyDisabledState = new DisabledState(false), TemporarilyDisabledState = new DisabledState(true);
	private static AimingState _aimingState;
	private static TapState _tapState;

	//current state holder	
	private static InputStateBase _leftHandState;

	private bool _tappedToPlay, _inTapCooldown, _inStateChangeCooldown;
	
	//only is being set by the in game ads, pickable barrel etc
	[HideInInspector] public bool userIsWatchingAnAdForPickup;

	private void OnEnable()
	{
		GameEvents.Only.EnterHitBox += OnEnterHitBox;
		GameEvents.Only.PunchHit += OnPunchHit;
		GameEvents.Only.GameEnd += OnGameOver;
		GameEvents.Only.EnemyKillPlayer += OnGameOver;
	}

	private void OnDisable()
	{
		GameEvents.Only.EnterHitBox -= OnEnterHitBox;
		GameEvents.Only.PunchHit -= OnPunchHit;
		GameEvents.Only.GameEnd -= OnGameOver;
		GameEvents.Only.EnemyKillPlayer -= OnGameOver;
	}

	private void Awake()
	{
		if (!Only) Only = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		if (testingUsingTouch) InputExtensions.IsUsingTouch = true;
		else
			InputExtensions.IsUsingTouch = Application.platform != RuntimePlatform.WindowsEditor &&
										   (Application.platform == RuntimePlatform.Android ||
											Application.platform == RuntimePlatform.IPhonePlayer);

		InputExtensions.TouchInputDivisor = GameExtensions.RemapClamped(1920, 2400, 30, 20, Screen.height);

		foreach (var hand in GameObject.FindGameObjectsWithTag("Hand"))
		{
			var ctrl = hand.GetComponent<HandController>();
			if (ctrl.isLeftHand) _leftHand = ctrl;
			else _rightHand = ctrl;
		}

		var cam = Camera.main;
		_ = new InputStateBase(_leftHand, _rightHand, cam, raycastDistance);
		_ = new DisabledState(_rightHand);
		_ = new DragToSmashState(_leftHand.GetAimController());
		
		_aimingState = new AimingState(_leftHand.GetAimController());
		_tapState = new TapState(_leftHand.GetAimController());
		_leftHandState = IdleState;
		
		//if "control mechanic" is zero, this variable is true
		//so default is tap and punch
		isUsingTapAndPunch = PlayerPrefs.GetInt("isUsingTapAndPunch", 0) == 0;
	}

	private void Update()
	{
		if (!HandleTapToPlay()) return;

		if (_inTapCooldown) { _leftHandState?.Execute(); return; }
		
		if(userIsWatchingAnAdForPickup) return;
		
		if (_leftHandState is IdleState)
		{
			var oldState = _leftHandState;
			_leftHandState = HandleInput();

			if (oldState != _leftHandState)
			{
				oldState.OnExit();
				_leftHandState?.OnEnter();
			}
		}
		else if (InputExtensions.GetFingerUp() && !InputStateBase.IsPersistent)
		{
			if (_leftHandState is AimingState)
				_leftHandState.OnExit();
			//here lies "cancel hand going to a enemy". go dig the git grave if you want it back
		}
		
		//print($"{_leftHandState}");
		_leftHandState?.Execute();
	}

	private void FixedUpdate()
	{
		_leftHandState?.FixedExecute();
	}

	private InputStateBase HandleInput()
	{
		if(IsInTemporaryDisabledState()) return _leftHandState;
		
		if (isUsingTapAndPunch)
		{
			if (!InputExtensions.GetFingerDown()) return _leftHandState;

			return _tapState;
		}

		if (!InputExtensions.GetFingerHeld()) return _leftHandState;

		return _aimingState;
	}

	private bool HandleTapToPlay()
	{
		if (_tappedToPlay) return true;

		if (!HasTappedOverUi()) return false;

		_tappedToPlay = true;
		GameEvents.Only.InvokeTapToPlay();
		
		//PutInTapCoolDown(0.25f);
		return true;
	}

	public static bool HasTappedOverUi()
	{
		if (!InputExtensions.GetFingerDown()) return false;

		if (!EventSystem.current) { print("no event system"); return false; }

		if (EventSystem.current.IsPointerOverGameObject(InputExtensions.IsUsingTouch ? Input.GetTouch(0).fingerId : -1)) return false;

		return true;
	}

	public void PutInTapCoolDown(float customCooldownTime = -1f)
	{
		if(IsInPermanentDisabledState()) return;

		_inTapCooldown = true;
		AssignTemporaryDisabledState();
		DOVirtual.DelayedCall(customCooldownTime > 0f ? customCooldownTime : tapCooldownWaitTime, TapCoolDown);
	}
	
	private void TapCoolDown()
	{
		AssignIdleState();
		_inTapCooldown = false;
	}

	public static void AssignNewState(InputStateBase newState, bool callOnExit = true)
	{
		if(_leftHandState == newState) return;
		
		if(callOnExit)
			_leftHandState?.OnExit();
		_leftHandState = newState;
		_leftHandState?.OnEnter();
	}

	public void ShouldUseTapAndPunch(bool status)
	{
		isUsingTapAndPunch = status;
		PlayerPrefs.SetInt("isUsingTapAndPunch", status ? 0 : 1);
	}

	public static bool CanSwitchToTargetState()
	{
		if (_leftHandState is InTransitState state && !state.GoHome && !state.IsCarryingBody) return true;
		return false;
	}

	public static bool IsInDisabledState() => _leftHandState is DisabledState;
	private static bool IsInPermanentDisabledState() => _leftHandState is DisabledState state && !state.IsTemporary;
	private static bool IsInTemporaryDisabledState() => _leftHandState is DisabledState state && state.IsTemporary;

	public static bool IsInIdleState() => _leftHandState is IdleState;

	public void WaitForPunch(Transform other) => _rightHand.WaitForPunch(other.transform);

	public void StopCarryingBody() => _leftHand.StopCarryingBody();

	public HandController GetLeftHand() => _leftHand;

	public HandController GetRightHand() => _rightHand;

	public void AssignIdleState()
	{
		if(IsInPermanentDisabledState()) return;
		
		AssignNewState(IdleState);
	}

	public static void AssignTemporaryDisabledState() => AssignNewState(TemporarilyDisabledState);

	private static void AssignPermanentDisabledState() => AssignNewState(PermanentlyDisabledState);

	public static void AssignReturnTransitState() => AssignNewState(new InTransitState(true, InputStateBase.EmptyHit));

	private void OnGameOver() => AssignPermanentDisabledState();

	private void OnEnterHitBox(Transform target)
	{
		if (canDragAfterGrabbingToAim)
			AssignNewState(DragToSmashState);
		else
			AssignTemporaryDisabledState();
	}

	private void OnPunchHit()
	{
		if(LevelFlowController.only.DidKillLastEnemyOfArea()) return;

		PutInTapCoolDown(0.75f);
		//DOVirtual.DelayedCall(0.1f, AssignIdleState);
	}
}