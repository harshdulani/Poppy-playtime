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
	private static InputStateBase _currentInputState;
	
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

		_ = new InputStateBase(_leftHand);
		_ = new OnTargetState(targetDragForce, _cam);
		_currentInputState = IdleState;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		
		if(!_inDisabledState) return;
		
		if(_currentInputState == IdleState || (_currentInputState is InTransitState state && state.GoHome))
		{
			var oldState = _currentInputState;
			_currentInputState = HandleInput();
			
			if(oldState != _currentInputState)
			{
				oldState?.OnExit();
				_currentInputState?.OnEnter();
			}
		}
		else if (InputExtensions.GetFingerUp() && !InputStateBase.IsPersistent)
		{
			if(_currentInputState is InTransitState || _currentInputState is OnTargetState) 
				AssignNewState(new InTransitState(true, InputStateBase.EmptyHit));
			//on finger up in transit could only be called if you were going there
		}

		print($"current input state {_currentInputState}");
		_currentInputState?.Execute();
	}

	private void FixedUpdate()
	{
		_currentInputState?.FixedExecute();
	}

	private InputStateBase HandleInput()
	{
		if (!InputExtensions.GetFingerHeld()) return _currentInputState;

		var ray = _cam.ScreenPointToRay(InputExtensions.GetInputPosition());
		
		if (!Physics.Raycast(ray, out var hit)) return _currentInputState; //if raycast didn't hit anything

		if (!hit.collider.CompareTag("Target")) return _currentInputState; //return fake dest state

		/*
		if (_currentInputState is OnTargetState)
		{
			hand = _rightHand;
			StartCoroutine(BlockInputTemporarily());
			return IdleState;
		}*/

		return new InTransitState(false, hit);
	}

	public static void AssignNewState(InputStateBase newState)
	{
		_currentInputState?.OnExit();
		_currentInputState = newState;
		_currentInputState?.OnEnter();
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
}