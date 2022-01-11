using UnityEngine;

public class InputHandler : MonoBehaviour
{
	public static InputHandler Only;

	public bool testingUsingTouch;

	private HandController _leftHand, _rightHand;

	//derived states
	public static readonly IdleState IdleState = new IdleState();
	private static readonly DisabledState DisabledState = new DisabledState();
	private static AimingState _aimingState;

	//current state holder	
	private static InputStateBase _leftHandState;

	private bool _tappedToPlay, _inDisabledState, _isTemporarilyDisabled;

	private void OnEnable()
	{
		GameEvents.only.tapToPlay += OnTapToPlay;
		GameEvents.only.enterHitBox += OnEnterHitBox;
		GameEvents.only.punchHit += OnPunchHit;
		GameEvents.only.gameEnd += OnGameOver;
		GameEvents.only.enemyReachPlayer += OnGameOver;
	}

	private void OnDisable()
	{
		GameEvents.only.tapToPlay -= OnTapToPlay;
		GameEvents.only.enterHitBox -= OnEnterHitBox;
		GameEvents.only.punchHit -= OnPunchHit;
		GameEvents.only.gameEnd -= OnGameOver;
		GameEvents.only.enemyReachPlayer -= OnGameOver;
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
		_ = new InputStateBase(_leftHand, cam);
		_aimingState = new AimingState(_leftHand.GetAimController());
		_leftHandState = IdleState;
	}

	private void Update()
	{ 
		if(!_tappedToPlay) return;
		
		//print(_leftHandState);
		if (_inDisabledState)
		{
			if(!_isTemporarilyDisabled) return;
			if(!InputExtensions.GetFingerDown()) return;
			
			_rightHand.GivePunch();
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
			else if (_leftHandState is InTransitState)
				AssignNewState(new InTransitState(true, InputStateBase.EmptyHit));
		}

		_leftHandState?.Execute();
	}

	private void FixedUpdate()
	{
		_leftHandState?.FixedExecute();
	}

	private InputStateBase HandleInput()
	{
		if (!InputExtensions.GetFingerHeld()) return _leftHandState;

		return _aimingState;
	}

	public static void AssignNewState(InputStateBase newState, bool callOnExit = true)
	{
		if(callOnExit)
			_leftHandState?.OnExit();
		_leftHandState = newState;
		_leftHandState?.OnEnter();
	}

	private static void ChangeStateToDisabled()
	{
		AssignNewState(DisabledState);
	}

	public bool CanSwitchToTargetState()
	{
		if (_leftHandState is InTransitState state && !state.GoHome && !state.IsCarryingBody) return true;
		return false;
	}

	public bool IsInDisabledState() => _leftHandState is DisabledState;

	public void WaitForPunch(Transform other)
	{
		_rightHand.WaitForPunch(other.transform);
	}

	private void OnTapToPlay() => _tappedToPlay = true;

	private void OnGameOver()
	{
		_inDisabledState = true;
		_isTemporarilyDisabled = false;
		ChangeStateToDisabled();
	}

	private void OnEnterHitBox(Transform target)
	{
		AssignDisabledState();
	}

	private void OnPunchHit()
	{
		//might need to come back here to add flexibility for enemies that take multiple hits
		if(LevelFlowController.only.DidKillLastEnemyOfArea()) return;

			Invoke(nameof(AssignIdleState), .1f);
	}
	
	public void StopCarryingBody()
	{
		_leftHand.StopCarryingBody();
	}

	public void AssignIdleState()
	{
		if(_inDisabledState && !_isTemporarilyDisabled) return;
		AssignNewState(IdleState);

		_isTemporarilyDisabled = false;
		_inDisabledState = false;
	}

	public void AssignDisabledState()
	{
		AssignNewState(DisabledState);
		_isTemporarilyDisabled = true;
		_inDisabledState = true;
	}
}