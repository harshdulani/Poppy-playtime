using Dreamteck.Splines;
using UnityEngine;

public class MovementController : MonoBehaviour
{
	[SerializeField] private bool followOnStart;
	private Collider _collider;
	private SplineFollower _spline;

	private bool _isSubscribed;
	
	private void OnEnable()
	{
		GameEvents.only.tapToPlay += OnTapToPlay;
		
		GameEvents.only.moveToNextArea += OnMoveToNextArea;
		_isSubscribed = true;
	}

	private void OnDisable()
	{
		GameEvents.only.tapToPlay -= OnTapToPlay;
		if(!_isSubscribed) return;
		
		GameEvents.only.moveToNextArea -= OnMoveToNextArea;
		_isSubscribed = false;
	}

	private void Start()
	{
		_spline = GetComponent<SplineFollower>();
		_collider = GetComponent<Collider>();
		
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
		Invoke(nameof(StartFollowing), 1.5f);
	}

	private void StartFollowing()
	{
		_spline.follow = true;
		_collider.enabled = false;
		PlayerSoundController.only.PlayFootSteps();
	}

	public void StopFollowing()
	{
		_spline.follow = false;
		_collider.enabled = true;
		InputHandler.Only.AssignIdleState();
	}
}
