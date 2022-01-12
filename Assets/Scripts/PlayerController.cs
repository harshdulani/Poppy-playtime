using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private HealthController _health;
	
	private void OnEnable()
	{
		GameEvents.only.enemyHitPlayer += OnEnemyHitPlayer;
		GameEvents.only.reachNextArea += OnReachNextArea;
	}

	private void OnDisable()
	{
		GameEvents.only.enemyHitPlayer -= OnEnemyHitPlayer;
		GameEvents.only.reachNextArea -= OnReachNextArea;
	}

	private void Start()
	{
		_health = GetComponent<HealthController>();
		_health.VisibilityToggle(false);
	}

	private void OnEnemyHitPlayer(Transform hitter)
	{
		if(!_health.AddHit(hitter)) return;

		CameraController.only.ScreenShake(3f);
		//_anim.SetTrigger(Hit);
		//show cracked screen
		Vibration.Vibrate(20);
		
		if (_health.IsDead())
			GameEvents.only.InvokeEnemyKillPlayer();
	}

	private void OnReachNextArea()
	{
		_health.VisibilityToggle(true);
		_health.hitsRequiredToKill = 3;
	}
}