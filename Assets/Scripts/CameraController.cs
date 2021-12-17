using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private float actionFov, zoomDuration;
	private float _normalFov;
	private Camera _me;
	
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

	private void Start()
	{
		_me = GetComponent<Camera>();

		_normalFov = _me.fieldOfView;
	}

	private void OnEnterHitBox()
	{
		DOTween.To(() => _me.fieldOfView, value => _me.fieldOfView = value, actionFov, zoomDuration);
	}

	private void OnPunchHit()
	{
		DOTween.To(() => _me.fieldOfView, value => _me.fieldOfView = value, _normalFov, zoomDuration);
	}
}
