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
		
		GameEvents.only.enemyReachPlayer += OnEnemyReachPlayer;
	}

	private void OnDisable()
	{
		GameEvents.only.enterHitBox -= OnEnterHitBox;
		GameEvents.only.punchHit -= OnPunchHit;
		
		GameEvents.only.enemyReachPlayer -= OnEnemyReachPlayer;
	}

	private void Start()
	{
		_me = GetComponent<Camera>();

		_normalFov = _me.fieldOfView;
	}
	
	private void ZoomNormal()
	{
		DOTween.To(() => _me.fieldOfView, value => _me.fieldOfView = value, _normalFov, zoomDuration);
	}

	private void ZoomAction()
	{
		DOTween.To(() => _me.fieldOfView, value => _me.fieldOfView = value, actionFov, zoomDuration);
	}

	private void OnEnterHitBox(Transform target)
	{
		ZoomAction();
	}

	private void OnPunchHit()
	{
		ZoomNormal();
	}

	private void OnEnemyReachPlayer()
	{
		ZoomAction();
	}
}
