using DG.Tweening;
using UnityEngine;

public class LevelFlowController : MonoBehaviour
{
	public static LevelFlowController only;

	[SerializeField] private float slowedTimeScale, timeRampDownDuration = 0.5f, timeRampUpDuration = 0.5f;
	private float _defaultTimeScale = 1;

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
		_defaultTimeScale = Time.timeScale;
		slowedTimeScale *= _defaultTimeScale;
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.T)) print("timeScale = " + Time.timeScale);
	}

	private void SlowDownTime()
	{
		DOTween.To(() => Time.timeScale, value => Time.timeScale = value, slowedTimeScale, timeRampDownDuration);
	}

	private void RevertTime()
	{
		DOTween.To(() => Time.timeScale, value => Time.timeScale = value, _defaultTimeScale, timeRampUpDuration);
	}

	private void OnEnterHitBox(Transform target)
	{
		SlowDownTime();
	}

	private void OnPunchHit()
	{
		RevertTime();
	}

}
