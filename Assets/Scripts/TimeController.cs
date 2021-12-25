using DG.Tweening;
using UnityEngine;

public class TimeController : MonoBehaviour
{
	public static TimeController only;

	[SerializeField] private float slowedTimeScale, timeRampDownDuration = 0.5f, timeRampUpDuration = 0.5f;
	[SerializeField] private AnimationCurve easing;
	private float _defaultTimeScale = 1;
	private float _defaultDeltaTime = 0.02f, _slowedDeltaTime;

	private void OnEnable()
	{
		GameEvents.only.enterHitBox += OnEnterHitBox;
	}

	private void OnDisable()
	{
		GameEvents.only.enterHitBox -= OnEnterHitBox;

		Time.timeScale = _defaultTimeScale;
	}

	private void Awake()
	{
		if (only) Destroy(gameObject);
		else only = this;
	}

	private void Start()
	{
		_defaultTimeScale = Time.timeScale;
		
		slowedTimeScale *= _defaultTimeScale;
		
		_slowedDeltaTime = _defaultDeltaTime * slowedTimeScale;
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.T)) print("timeScale = " + Time.timeScale);
	}

	private void SlowDownTime()
	{
		DOTween.To(() => Time.timeScale, value => Time.timeScale = value, slowedTimeScale, timeRampDownDuration);
		DOTween.To(() => Time.fixedDeltaTime, value => Time.fixedDeltaTime = value, _slowedDeltaTime, timeRampDownDuration);
	}

	public void RevertTime(bool lastEnemy = false)
	{
		var tween = DOTween.To(() => Time.timeScale, value => Time.timeScale = value, _defaultTimeScale, timeRampUpDuration);

		if (!lastEnemy) return;
		
		tween.SetEase(easing);
		DOTween.To(() => Time.fixedDeltaTime, value => Time.fixedDeltaTime = value, _defaultDeltaTime,
			timeRampUpDuration).SetEase(easing);
	}

	private void OnEnterHitBox(Transform target)
	{
		SlowDownTime();
	}
}
