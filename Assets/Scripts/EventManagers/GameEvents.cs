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

	public Action<Transform> enterHitBox;
	public Action punchHit;

	public void InvokeEnterHitBox(Transform target) => enterHitBox?.Invoke(target);

	public void InvokePunchHit() => punchHit?.Invoke();
}
