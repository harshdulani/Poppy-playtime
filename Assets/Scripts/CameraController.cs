using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public static CameraController only; 
	
	[SerializeField] private float actionFov, zoomDuration;

	[Header("ScreenShake"), SerializeField]
	private float shakeDuration;
	[SerializeField] private float shakeStrength;

	private Vector3 _initialLocalPos;
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

	private void Awake()
	{
		if (!only) only = this;
		else Destroy(only);
	}

	private void Start()
	{
		_me = GetComponent<Camera>();

		_normalFov = _me.fieldOfView;
		_initialLocalPos = transform.localPosition;
	}
	
	private void ZoomNormal()
	{
		DOTween.To(() => _me.fieldOfView, value => _me.fieldOfView = value, _normalFov, zoomDuration);
	}

	private void ZoomAction()
	{
		DOTween.To(() => _me.fieldOfView, value => _me.fieldOfView = value, actionFov, zoomDuration);
	}

	public void ScreenShake(float intensity)
	{
		_me.DOShakePosition(shakeDuration * intensity / 2f, shakeStrength * intensity, 10, 45f).OnComplete(() => transform.DOLocalMove(_initialLocalPos, 0.15f));
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
