using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
	public int hitsRequiredToKill, hitsReceived;

	private readonly List<Transform> _hitters = new List<Transform>();
	
	public bool IsDead()
	{
		return hitsReceived >= hitsRequiredToKill;
	}

	public bool AddHit(Transform hitter)
	{
		//might need to add a bool for if this is not a car and venom-esque enemy, you can allow them to hit you twice
		if(_hitters.Contains(hitter)) return false;
		
		_hitters.Add(hitter);
		hitsReceived++;
		return true;
	}

	public void AddGrabbedCar(Transform car)
	{
		_hitters.Add(car);
	}
}