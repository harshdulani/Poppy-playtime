using UnityEngine;

public class LevelFlowController : MonoBehaviour
{
	public static LevelFlowController only;

	[SerializeField] private float defaultTimeScale = 1, slowedTimeScale;

	private void Awake()
	{
		if (only) Destroy(gameObject);
		else only = this;
	}

	private void Start()
	{
		Time.timeScale = defaultTimeScale;
		slowedTimeScale *= defaultTimeScale;
	}

	public void SlowDownTime()
	{
		Time.timeScale = slowedTimeScale;
	}

	public void RevertTime()
	{
		Time.timeScale = defaultTimeScale;
	}
}
