using DG.Tweening;
using UnityEngine;

public class LevelFlowController : MonoBehaviour
{
	public static LevelFlowController only;

	[SerializeField] private float defaultTimeScale = 1, slowedTimeScale, timeRampDownDuration = 0.5f, timeRampUpDuration = 0.5f;

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

	private void Awake()
	{
		if (only) Destroy(gameObject);
		else only = this;
	}

	private void Start()
	{
		defaultTimeScale = Time.timeScale;
		slowedTimeScale *= defaultTimeScale;
	}
	
	private void SlowDownTime()
	{
		DOTween.To(() => Time.timeScale, value => Time.timeScale = value, slowedTimeScale, timeRampDownDuration);
	}

	private void RevertTime()
	{
		DOTween.To(() => Time.timeScale, value => Time.timeScale = value, defaultTimeScale, timeRampUpDuration);
	}

	private void OnEnterHitBox()
	{
		SlowDownTime();
	}

	private void OnPunchHit()
	{
		RevertTime();
	}
}
