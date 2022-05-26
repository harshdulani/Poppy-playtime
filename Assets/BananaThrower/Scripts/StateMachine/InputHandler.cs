using System;
using UnityEngine;
using Player;
using UnityEngine.EventSystems;

namespace StateMachine
{
	public enum InputState { Idle, Disabled, TapState }
	
	public class InputHandler : MonoBehaviour
	{
		//current state holder	
		private static InputStateBase _currentInputState;
		
		//all states
		private static readonly IdleState IdleState = new IdleState();
		private static readonly DisabledState DisabledState = new DisabledState();
		private static readonly TapState TapState =  new TapState();

		private bool _hasTappedToPlay;
		private Transform _ikTarget;
		private bool _tappedToPlay;

		private void OnEnable()
		{
			GameEvents.Only.TapToPlay += OnTapToPlay;
			GameEvents.Only.GameEnd += OnGameEnd;
			GameEvents.Only.EnemyKillPlayer += OnGameEnd;
		}

		private void OnDisable()
		{
			GameEvents.Only.TapToPlay += OnTapToPlay;
			GameEvents.Only.GameEnd += OnGameEnd;
			GameEvents.Only.EnemyKillPlayer += OnGameEnd;
		}

		private void Start()
		{
			InputExtensions.IsUsingTouch = Application.platform != RuntimePlatform.WindowsEditor &&
										   (Application.platform == RuntimePlatform.Android ||
											Application.platform == RuntimePlatform.IPhonePlayer);
			InputExtensions.TouchInputDivisor = GameExtensions.RemapClamped(1920, 2400, 30, 20, Screen.height);

			var player = GameObject.FindGameObjectWithTag("Player");
			
			_ = new InputStateBase(player.GetComponent<PlayerRefBank>());
			
			_currentInputState = IdleState;
		}

		private void Update()
		{
			if (!HandleTapToPlay()) return;
			
			if (_currentInputState is IdleState)
			{
				var oldState = _currentInputState;
				_currentInputState = HandleInput();
				
				if(_currentInputState != oldState)
					_currentInputState?.OnEnter();
			}

			//print($"{_currentInputState}");
			_currentInputState?.Execute();
		}

		private void FixedUpdate()
		{
			_currentInputState?.FixedExecute();
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

		private static bool HasTappedOverUi()
		{
			if (!InputExtensions.GetFingerDown()) return false;

			if (!EventSystem.current) { print("no event system"); return false; }

			if (EventSystem.current.IsPointerOverGameObject(InputExtensions.IsUsingTouch ? Input.GetTouch(0).fingerId : -1)) return false;

			return true;
		}

		private InputStateBase HandleInput()
		{
			//if (GameExtensions.GetObjectUnderPointer()) return _currentInputState;
			
			if (InputExtensions.GetFingerUp()) return TapState;
			
			return _currentInputState;
		}
	
		public static InputStateBase GetCurrentState() => _currentInputState;
		public void SetIkTarget(Transform rightHandTarget) => _ikTarget = rightHandTarget;

		public static void AssignNewState(InputState state)
		{
			_currentInputState?.OnExit();

			_currentInputState = state switch
			{
				InputState.Idle => IdleState,
				InputState.Disabled => DisabledState,
				InputState.TapState => TapState,
				_ => throw new ArgumentOutOfRangeException(nameof(state), state, "aisa kya pass kar diya vro tune yahaan")
			};

			_currentInputState?.OnEnter();
		}

		private static void AssignNewState(InputStateBase newState)
		{
			_currentInputState?.OnExit();
			_currentInputState = newState;
			_currentInputState?.OnEnter();
		}

		private void OnTapToPlay() => _hasTappedToPlay = true;

		private static void OnGameEnd() => AssignNewState(InputState.Disabled);
	}
}