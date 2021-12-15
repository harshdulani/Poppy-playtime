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
	
	public Action punch;

	public void InvokePunch()
	{
		punch?.Invoke();
	}
}
