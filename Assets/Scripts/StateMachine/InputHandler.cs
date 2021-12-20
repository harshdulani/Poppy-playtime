using UnityEngine;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
	public static InputHandler Only;

	public bool testingUsingTouch;
	[SerializeField] private float targetDragForce;

	//derived states
	public static readonly IdleState IdleState = new IdleState();
	private static readonly DisabledState DisabledState = new DisabledState();

	//current state holder	
	private static InputStateBase _leftHandState;

	private HandController _leftHand, _rightHand;

	private Transform _lastPickedTarget;
	private Camera _cam;

	private bool _tappedToPlay, _inDisabledState, _isTemporarilyDisabled;

	private void OnEnable()
	{
		GameEvents.only.enterHitBox += OnEnterHitBox;
		GameEvents.only.punchHit += OnPunchHit;
	}

	private void OnDisable()
	{
		GameEvents.only.enterHitBox -= OnEnterHitBox;
		GameEvents.only.punchHit -= OnPunchHit;
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

		_cam = Camera.main;

		foreach (var hand in GameObject.FindGameObjectsWithTag("Hand"))
		{
			var ctrl = hand.GetComponent<HandController>();
			if (ctrl.isLeftHand) _leftHand = ctrl;
			else _rightHand = ctrl;
		}
		
		_ = new InputStateBase(_leftHand);
		_ = new OnTargetState(targetDragForce, _cam);
		_leftHandState = IdleState;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

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
			if (_leftHandState is InTransitState || _leftHandState is OnTargetState)
				AssignNewState(new InTransitState(true, InputStateBase.EmptyHit, 
					_leftHandState is OnTargetState));
			//on finger up in transit could only be called if you were going there
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

		var ray = _cam.ScreenPointToRay(InputExtensions.GetInputPosition());

		if (!Physics.Raycast(ray, out var hit)) return _leftHandState; //if raycast didn't hit anything

		if (!hit.collider.CompareTag("Target")) return _leftHandState; //return fake dest state

		_lastPickedTarget = hit.transform;
		return new InTransitState(false, hit);
	}

	public static void AssignNewState(InputStateBase newState)
	{
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

	public void WaitForPunch(Transform other, float zPos)
	{
		_rightHand.WaitForPunch(other.transform, zPos);
	}
	
	public Transform GetCurrentTransform() => _lastPickedTarget;

	private void OnGameStart() => _tappedToPlay = true;

	private void OnGameOver()
	{
		_inDisabledState = false;
		ChangeStateToDisabled();
	}

	private void OnEnterHitBox(Transform target)
	{
		//also level flow controller knows to slow down and fasten time up using this event
		AssignNewState(DisabledState);
		_isTemporarilyDisabled = true;
		_inDisabledState = true;
	}

	private void OnPunchHit()
	{
		AssignNewState(IdleState);
		
		_isTemporarilyDisabled = false;
		_inDisabledState = false;
		
	}

	public void StopCarryingBody()
	{
		_leftHand.StopCarryingBody();
	}
}