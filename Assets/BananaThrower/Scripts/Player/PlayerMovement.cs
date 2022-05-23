/*using DG.Tweening;
using Dreamteck.Splines;
using StateMachine;
using UnityEngine;

namespace Player
{
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private bool followOnStart;
		private SplineFollower _spline;

		private bool _isSubscribed;
	
		private void OnEnable()
		{
			GameFlowEvents.TapToPlay += OnTapToPlay;
		
			GameFlowEvents.MoveToNextArea += OnMoveToNextArea;
			GameFlowEvents.ReachNextArea += OnReachNextArea;
			_isSubscribed = true;
		}

		private void OnDisable()
		{
			GameFlowEvents.TapToPlay -= OnTapToPlay;
			if(!_isSubscribed) return;
		
			GameFlowEvents.MoveToNextArea -= OnMoveToNextArea;
			GameFlowEvents.ReachNextArea -= OnReachNextArea;
			_isSubscribed = false;
		}

		private void Start()
		{
			_spline = GetComponent<SplineFollower>();
		
			if(_spline.spline) return;
			OnDisable();
		}

		private void OnTapToPlay()
		{
			if (!followOnStart) return;
		
			StartFollowing();
			LevelFlowController.only.currentArea--;
		}
	
		private void OnMoveToNextArea()
		{
			DOVirtual.DelayedCall(1.5f, StartFollowing);
		}

		private void StartFollowing()
		{
			_spline.follow = true;
		}

		private void OnReachNextArea()
		{
			//StopFollowing
			_spline.follow = false;
		}
	}
}*/