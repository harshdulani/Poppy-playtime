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
	private static InputStateBase _leftHandState, _rightHandState;
	
	public HandController _leftHand, _rightHand;
	
	private Camera _cam;

	private bool _tappedToPlay, _inDisabledState = true;

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void Awake()
	{
		if (!Only) Only = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		if (testingUsingTouch) InputExtensions.IsUsingTouch = true; 
		else InputExtensions.IsUsingTouch = Application.platform != RuntimePlatform.WindowsEditor &&
											(Application.platform == RuntimePlatform.Android || 
											 Application.platform == RuntimePlatform.IPhonePlayer);
		
		InputExtensions.TouchInputDivisor = GameExtensions.RemapClamped(1920, 2400, 30, 20, Screen.height);
		
		_cam = Camera.main;

		_ = new InputStateBase(_leftHand, _rightHand);
		_ = new OnTargetState(targetDragForce, _cam);
		_leftHandState = IdleState;
		_rightHandState = IdleState;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		
		if(!_inDisabledState) return;
		
		if(_leftHandState is IdleState)
		{
			var oldState = _leftHandState;
			_leftHandState = HandleInput();
			
			if(oldState != _leftHandState)
			{
				oldState?.OnExit();
				_leftHandState?.OnEnter();
			}
		}
		else if (InputExtensions.GetFingerUp() && !InputStateBase.IsPersistent)
		{
			if(_leftHandState is InTransitState || _leftHandState is OnTargetState)
				AssignNewState(new InTransitState(true, InputStateBase.EmptyHit, true, _leftHandState is OnTargetState));
			//on finger up in transit could only be called if you were going there
		}
		_leftHandState?.Execute();

		//HandleRhs();
		HandleRightHand();
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

		return new InTransitState(false, hit);
	}

	private void HandleRightHand()
	{
		print(_rightHandState);
		if(_rightHandState is WaitingToPunchState)
		{
			var oldState = _rightHandState;
			_rightHandState = HandleRightHandInput();
			
			if(oldState != _rightHandState)
			{
				oldState?.OnExit();
				_rightHandState?.OnEnter();
			}
		}
		
		_rightHandState?.Execute();
	}

	private InputStateBase HandleRightHandInput()
	{
		if (!InputExtensions.GetFingerDown()) return _rightHandState;
		
		if (!(_leftHandState is InTransitState)) return _rightHandState;
		
		_rightHand.DeliverPunch(WaitingToPunchState.Target);
		return IdleState;
	}
	
	public static void AssignNewState(InputStateBase newState, bool shouldChangeRhs = false)
	{
		if(shouldChangeRhs)
		{
			_rightHandState?.OnExit();
			_rightHandState = newState;
			_rightHandState?.OnEnter();
			return;
		}
		
		_leftHandState?.OnExit();
		_leftHandState = newState;
		_leftHandState?.OnEnter();
	}
	private static void ChangeStateToDisabled()
	{
		AssignNewState(DisabledState);
	}

	private void OnGameStart() => _tappedToPlay = true;

	private void OnGameOver()
	{
		_inDisabledState = false;
		ChangeStateToDisabled();
	}

	public InputStateBase ReturnWaitingToPunch()
	{
		return new WaitingToPunchState(_leftHand.palm.parent);
	}
}