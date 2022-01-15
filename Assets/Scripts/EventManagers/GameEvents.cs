using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
	#region Singleton
	
	public static GameEvents only;

	private void Awake()
	{
		if (only) Destroy(gameObject);
		else only = this;
	}

	#endregion

	public Action tapToPlay;
		
	public Action<Transform> enterHitBox;
	public Action punchHit;

	public Action<Transform> propDestroyed, giantLanding, giantPickupCar;
	
	public Action<Transform> rayfireShattered;
	
	public Action enemyKilled;

	public Action moveToNextArea, reachNextArea;

	public Action<Transform> enemyHitPlayer;
	public Action enemyKillPlayer;
	public Action gameEnd;


	public void InvokeTapToPlay() => tapToPlay?.Invoke();
	public void InvokeEnterHitBox(Transform target) => enterHitBox?.Invoke(target);

	public void InvokePunchHit() => punchHit?.Invoke();

	public void InvokePropDestroy(Transform target) => propDestroyed?.Invoke(target);

	public void InvokeGiantLanding(Transform giant) => giantLanding?.Invoke(giant);
	
	public void InvokeGiantPickupCar(Transform car) => giantPickupCar?.Invoke(car);

	public void InvokeRayfireShattered(Transform shattered) => rayfireShattered?.Invoke(shattered);
	
	public void InvokeEnemyKill() => enemyKilled?.Invoke();
	
	public void InvokeEnemyHitPlayer(Transform hitter) => enemyHitPlayer?.Invoke(hitter);
	
	//Mass migration of kill player subscribers and invokers coming to hit player 
	public void InvokeEnemyKillPlayer() => enemyKillPlayer?.Invoke();
	
	public void InvokeMoveToNextArea() => moveToNextArea?.Invoke();
	public void InvokeReachNextArea() => reachNextArea?.Invoke();
	
	public void InvokeGameEnd() => gameEnd?.Invoke();
}