using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
	#region Singleton
	
	public static GameEvents Only;

	private void Awake()
	{
		if (Only) Destroy(gameObject);
		else Only = this;
	}

	#endregion

	public event Action TapToPlay;
		
	public event Action<Transform> EnterHitBox;
	public event Action PunchHit;
	
	public event Action PropHitsEnemy;
	public event Action<Transform> PropDestroyed, GiantLanding;
	public event Action<Transform> GiantPickupProp;
	
	public event Action<Transform> RayfireShattered, EnemyDied;

	public event Action TrapButtonPressed;
	public event Action<Rigidbody> DropContainer;
	public event Action DropArmor, EnemyKilled;

	public event Action MoveToNextArea, ReachNextArea;

	public event Action<Transform> EnemyHitPlayer;
	public event Action EnemyKillPlayer;
	public event Action GameEnd;

	public event Action<int, bool> WeaponSelect, SkinSelect;

	public void InvokeTapToPlay() => TapToPlay?.Invoke();
	public void InvokeEnterHitBox(Transform target) => EnterHitBox?.Invoke(target);

	public void InvokePunchHit() => PunchHit?.Invoke();

	public void InvokePropHitsEnemy() => PropHitsEnemy?.Invoke();
	
	public void InvokePropDestroy(Transform target) => PropDestroyed?.Invoke(target);

	public void InvokeGiantLanding(Transform giant) => GiantLanding?.Invoke(giant);
	
	public void InvokeGiantPickupProp(Transform car) => GiantPickupProp?.Invoke(car);

	public void InvokeRayfireShattered(Transform shattered) => RayfireShattered?.Invoke(shattered);

	public void InvokeTrapButtonPressed() => TrapButtonPressed?.Invoke();
	public void InvokeDropContainer(Rigidbody container) => DropContainer?.Invoke(container);
	
	public void InvokeDropArmor() => DropArmor?.Invoke();
	public void InvokeEnemyKill() => EnemyKilled?.Invoke();
	
	public void InvokeEnemyDied(Transform enemy) => EnemyDied?.Invoke(enemy);
	
	public void InvokeEnemyHitPlayer(Transform hitter) => EnemyHitPlayer?.Invoke(hitter);
	
	//Mass migration of kill player subscribers and invokers coming to hit player 
	public void InvokeEnemyKillPlayer() => EnemyKillPlayer?.Invoke();
	
	public void InvokeMoveToNextArea() => MoveToNextArea?.Invoke();
	public void InvokeReachNextArea() => ReachNextArea?.Invoke();
	
	public void InvokeGameEnd() => GameEnd?.Invoke();

	public void InvokeWeaponSelect(int index, bool shouldDeductCoins) => WeaponSelect?.Invoke(index, shouldDeductCoins);
	public void InvokeSkinSelect(int index, bool shouldDeductCoins) => SkinSelect?.Invoke(index, shouldDeductCoins);
}