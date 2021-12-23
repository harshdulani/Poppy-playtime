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

	public Action enemyKilled;

	public Action moveToNextArea;
	public Action gameEnd;


	public void InvokeTapToPlay() => tapToPlay?.Invoke();
	public void InvokeEnterHitBox(Transform target) => enterHitBox?.Invoke(target);

	public void InvokePunchHit() => punchHit?.Invoke();

	public void InvokeEnemyKill() => enemyKilled?.Invoke();

	public void InvokeGameEnd() => gameEnd?.Invoke();

	public void InvokeNextArea() => moveToNextArea?.Invoke();
}