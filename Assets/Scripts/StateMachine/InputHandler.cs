using DG.Tweening;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
	public static InputHandler Only;

	[SerializeField] private float raycastDistance = 50f;
	public bool testingUsingTouch;
	public bool isUsingTapAndPunch;
	
	[SerializeField] private float tapCooldownWaitTime;

	private HandController _leftHand, _rightHand;

	//derived states
	public static readonly IdleState IdleState = new IdleState();
	private static readonly DisabledState DisabledState = new DisabledState();
	private static AimingState _aimingState;
	private static TapState _tapState;

	//current state holder	
	private static InputStateBase _leftHandState;

	private bool _tappedToPlay, _inDisabledState, _isTemporarilyDisabled, _inTapCooldown;
	
	//only is being set by the in game ads, pickable barrel etc
	[HideInInspector] public bool userIsWatchingAnAdForPickup;

	private void OnEnable()
	{
		GameEvents.Only.TapToPlay += OnTapToPlay;
		GameEvents.Only.EnterHitBox += OnEnterHitBox;
		GameEvents.Only.PunchHit += OnPunchHit;
		GameEvents.Only.GameEnd += OnGameOver;
		GameEvents.Only.EnemyKillPlayer += OnGameOver;
	}

	private void OnDisable()
	{
		GameEvents.Only.TapToPlay -= OnTapToPlay;
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
		_ = new InputStateBase(_leftHand, cam, raycastDistance);
		_aimingState = new AimingState(_leftHand.GetAimController());
		_tapState = new TapState(_leftHand.GetAimController());
		_leftHandState = IdleState;
		
		//if "control mechanic" is zero, this variable is true
		//so default is tap and punch
		isUsingTapAndPunch = PlayerPrefs.GetInt("controlMechanic", 0) != 0;
	}

	private void Update()
	{
		if(!_tappedToPlay) return;
		
		if(userIsWatchingAnAdForPickup) return;
		
		if (_inTapCooldown) return;

		//print(_leftHandState);
		if (_inDisabledState)
		{
			if(!_isTemporarilyDisabled) return;
			if(!InputExtensions.GetFingerDown()) return;
			if(!_rightHand.TryGivePunch()) return;
			
			PutInTapCoolDown();
			return;
		}

		if (_leftHandState is IdleState)
		{
			var oldState = _leftHandState;
			_leftHandState = HandleInput();

			if (oldState != _leftHandState)
			{
				oldState?.OnExit();
				_leftHandState?.OnEnter();
			}
		}
		else if (InputExtensions.GetFingerUp() && !InputStateBase.IsPersistent)
		{
			if (_leftHandState is AimingState)
				_leftHandState.OnExit();
			//here lies "cancel hand going to a enemy". go dig the git grave if you want it back
		}

		_leftHandState?.Execute();
	}

	private void FixedUpdate()
	{
		_leftHandState?.FixedExecute();
	}

	private InputStateBase HandleInput()
	{
		if (isUsingTapAndPunch)
		{
			if (!InputExtensions.GetFingerDown()) return _leftHandState;
			
			return _tapState;
		}

		if (!InputExtensions.GetFingerHeld()) return _leftHandState;

		return _aimingState;
	}

	private void PutInTapCoolDown(float customCooldownTime = -1f)
	{
		if(IsInDisabledState()) return;
		
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
		if(callOnExit)
			_leftHandState?.OnExit();
		_leftHandState = newState;
		_leftHandState?.OnEnter();
	}

	public void ShouldUseTapAndPunch(bool status)
	{
		isUsingTapAndPunch = status;
		PlayerPrefs.SetInt("controlMechanic", status ? 0 : 1);
	}
	
	private void ChangeStateToDisabled()
	{
		_isTemporarilyDisabled = false;
		AssignNewState(DisabledState);
	}

	public bool CanSwitchToTargetState()
	{
		if (_leftHandState is InTransitState state && !state.GoHome && !state.IsCarryingBody) return true;
		return false;
	}

	public static bool IsInDisabledState() => _leftHandState is DisabledState;
	public static bool IsInIdleState() => _leftHandState is IdleState;

	public void WaitForPunch(Transform other) => _rightHand.WaitForPunch(other.transform);

	public void StopCarryingBody() => _leftHand.StopCarryingBody();

	public HandController GetLeftHand() => _leftHand;
	public HandController GetRightHand() => _rightHand;

	public void AssignIdleState()
	{
		if(_inDisabledState && !_isTemporarilyDisabled) return;
		
		AssignNewState(IdleState);

		_isTemporarilyDisabled = false;
		_inDisabledState = false;
	}

	public void AssignTemporaryDisabledState()
	{
		AssignNewState(DisabledState);
		_isTemporarilyDisabled = true;
		_inDisabledState = true;
	}

	public static void AssignReturnTransitState()
	{
		AssignNewState(new InTransitState(true, InputStateBase.EmptyHit));
	}

	private void OnTapToPlay()
	{
		_tappedToPlay = true;
		PutInTapCoolDown(0.75f);
	}

	private void OnGameOver()
	{
		_inDisabledState = true;
		_isTemporarilyDisabled = false;
		ChangeStateToDisabled();
	}

	private void OnEnterHitBox(Transform target)
	{
		AssignTemporaryDisabledState();
	}

	private void OnPunchHit()
	{
		if(LevelFlowController.only.DidKillLastEnemyOfArea()) return;

		Invoke(nameof(AssignIdleState), .1f);
	}
}