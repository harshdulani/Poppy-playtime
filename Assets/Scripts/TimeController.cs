using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class TimeController : MonoBehaviour
{
	public static TimeController only;

	[SerializeField] private float slowedTimeScale, timeRampDownDuration = 0.5f, timeRampUpDuration = 0.5f;
	[SerializeField] private AnimationCurve easing;
	private float _defaultTimeScale = 1;
	private const float DefaultFixedDeltaTime = 0.02f;
	private float _slowedDeltaTime;
	private bool _isTimeSlowedDown;

	private TweenerCore<float, float, FloatOptions> _timeDeltaTween, _fixedTimeDeltaTween;
	
	private void OnEnable()
	{
		GameEvents.Only.EnterHitBox += OnEnterHitBox;
	}

	private void OnDisable()
	{
		GameEvents.Only.EnterHitBox -= OnEnterHitBox;

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
		
		_slowedDeltaTime = DefaultFixedDeltaTime * slowedTimeScale;
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.T)) print("timeScale = " + Time.timeScale);
	}

	public void SlowDownTime(float multiplier = 1f)
	{
		if(_isTimeSlowedDown) return;
		
		_isTimeSlowedDown = true;
		_timeDeltaTween.Kill();
		_fixedTimeDeltaTween.Kill();
		
		_timeDeltaTween = DOTween.To(() => Time.timeScale, value => Time.timeScale = value, slowedTimeScale * multiplier, timeRampDownDuration).SetUpdate(true);
		_fixedTimeDeltaTween = DOTween.To(() => Time.fixedDeltaTime, value => Time.fixedDeltaTime = value, _slowedDeltaTime * multiplier, timeRampDownDuration).SetUpdate(true);
	}

	public void RevertTime(bool lastEnemy = false)
	{
		if(!_isTimeSlowedDown) return;
		
		_isTimeSlowedDown = false;
		_timeDeltaTween.Kill();
		_fixedTimeDeltaTween.Kill();

		_timeDeltaTween = DOTween.To(() => Time.timeScale, value => Time.timeScale = value, _defaultTimeScale, timeRampUpDuration).SetUpdate(true);
		_fixedTimeDeltaTween = DOTween.To(() => Time.fixedDeltaTime, value => Time.fixedDeltaTime = value, DefaultFixedDeltaTime, timeRampUpDuration).SetUpdate(true);
		
		if (!lastEnemy) return;
		
		_timeDeltaTween.SetEase(easing);
		_fixedTimeDeltaTween.SetEase(easing);
	}

	private void OnEnterHitBox(Transform target)
	{
		SlowDownTime();
	}
}
