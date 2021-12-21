using Dreamteck.Splines;
using UnityEngine;

public class MovementController : MonoBehaviour
{
	private SplineFollower _spline;

	private void OnEnable()
	{
		GameEvents.only.moveToNextArea += OnMoveToNextArea;
	}

	private void OnDisable()
	{
		GameEvents.only.moveToNextArea -= OnMoveToNextArea;
	}

	private void Start()
	{
		_spline = GetComponent<SplineFollower>();
		_spline.spline = GameObject.FindGameObjectWithTag("SplinePath").GetComponent<SplineComputer>();
	}

	private void OnMoveToNextArea()
	{
		Invoke(nameof(StartFollowing), 1.5f);
	}

	private void StartFollowing()
	{
		_spline.follow = true;
	}

	public void StopFollowing()
	{
		_spline.follow = false;
	}
}
