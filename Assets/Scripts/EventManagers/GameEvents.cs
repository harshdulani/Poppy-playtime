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

	public Action enterHitBox, punchHit;

	public void InvokeEnterHitBox() => enterHitBox?.Invoke();

	public void InvokePunchHit() => punchHit?.Invoke();
}
