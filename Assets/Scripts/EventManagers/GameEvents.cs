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
	
	public Action propHitsEnemy;
	public Action<Transform> propDestroyed, giantLanding;
	public Action<Transform> giantPickupProp;
	
	public Action<Transform> rayfireShattered;
	
	public Action<Rigidbody> dropContainer;
	public Action dropArmor, enemyKilled;

	public Action moveToNextArea, reachNextArea;

	public Action<Transform> enemyHitPlayer;
	public Action enemyKillPlayer;
	public Action gameEnd;

	public Action<int, bool> weaponSelect, skinSelect;

	public void InvokeTapToPlay() => tapToPlay?.Invoke();
	public void InvokeEnterHitBox(Transform target) => enterHitBox?.Invoke(target);

	public void InvokePunchHit() => punchHit?.Invoke();

	public void InvokePropHitsEnemy() => propHitsEnemy?.Invoke();
	
	public void InvokePropDestroy(Transform target) => propDestroyed?.Invoke(target);

	public void InvokeGiantLanding(Transform giant) => giantLanding?.Invoke(giant);
	
	public void InvokeGiantPickupProp(Transform car) => giantPickupProp?.Invoke(car);

	public void InvokeRayfireShattered(Transform shattered) => rayfireShattered?.Invoke(shattered);

	public void InvokeDropContainer(Rigidbody container) => dropContainer?.Invoke(container);
	
	public void InvokeDropArmor() => dropArmor?.Invoke();
	public void InvokeEnemyKill() => enemyKilled?.Invoke();
	
	public void InvokeEnemyHitPlayer(Transform hitter) => enemyHitPlayer?.Invoke(hitter);
	
	//Mass migration of kill player subscribers and invokers coming to hit player 
	public void InvokeEnemyKillPlayer() => enemyKillPlayer?.Invoke();
	
	public void InvokeMoveToNextArea() => moveToNextArea?.Invoke();
	public void InvokeReachNextArea() => reachNextArea?.Invoke();
	
	public void InvokeGameEnd() => gameEnd?.Invoke();

	public void InvokeWeaponSelect(int index, bool shouldDeductCoins) => weaponSelect?.Invoke(index, shouldDeductCoins);
	public void InvokeSkinSelect(int index, bool shouldDeductCoins) => skinSelect?.Invoke(index, shouldDeductCoins);
}