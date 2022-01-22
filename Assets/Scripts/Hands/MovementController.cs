using Dreamteck.Splines;
using UnityEngine;

public class MovementController : MonoBehaviour
{
	private Collider _collider;
	private SplineFollower _spline;

	private bool _isSubscribed;
	
	private void OnEnable()
	{
		GameEvents.only.moveToNextArea += OnMoveToNextArea;
		_isSubscribed = true;
	}

	private void OnDisable()
	{
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
