using System;
using UnityEngine;
using UnityEngine.Serialization;

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
	
	public Action propHitsEnemy;
	public Action<Transform> propDestroyed, giantLanding;
	public Action<Transform> giantPickupProp;
	
	public Action<Transform> rayfireShattered;
	
	public Action enemyKilled;

	public Action moveToNextArea, reachNextArea;

	public Action<Transform> enemyHitPlayer;
	public Action enemyKillPlayer;
	public Action gameEnd;

	public Action<int, ShopItemState> weaponSelect, skinSelect;

	public void InvokeTapToPlay() => tapToPlay?.Invoke();
	public void InvokeEnterHitBox(Transform target) => enterHitBox?.Invoke(target);

	public void InvokePunchHit() => punchHit?.Invoke();

	public void InvokePropHitsEnemy() => propHitsEnemy?.Invoke();
	
	public void InvokePropDestroy(Transform target) => propDestroyed?.Invoke(target);

	public void InvokeGiantLanding(Transform giant) => giantLanding?.Invoke(giant);
	
	public void InvokeGiantPickupProp(Transform car) => giantPickupProp?.Invoke(car);

	public void InvokeRayfireShattered(Transform shattered) => rayfireShattered?.Invoke(shattered);
	
	public void InvokeEnemyKill() => enemyKilled?.Invoke();
	
	public void InvokeEnemyHitPlayer(Transform hitter) => enemyHitPlayer?.Invoke(hitter);
	
	//Mass migration of kill player subscribers and invokers coming to hit player 
	public void InvokeEnemyKillPlayer() => enemyKillPlayer?.Invoke();
	
	public void InvokeMoveToNextArea() => moveToNextArea?.Invoke();
	public void InvokeReachNextArea() => reachNextArea?.Invoke();
	
	public void InvokeGameEnd() => gameEnd?.Invoke();

	public void InvokeWeaponSelect(int index, ShopItemState previousState) => weaponSelect?.Invoke(index, previousState);
	public void InvokeSkinSelect(int index, ShopItemState previousState) => skinSelect?.Invoke(index, previousState);
}