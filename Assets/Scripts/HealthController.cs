using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
	public int hitsRequiredToKill, hitsReceived;

	[SerializeField] private HealthCanvasController healthCanvas;
	
	private readonly List<Transform> _hitters = new List<Transform>();
	
	public bool IsDead()
	{
		return hitsReceived >= hitsRequiredToKill;
	}

	public bool AddHit(Transform hitter)
	{
		//might need to add a bool for if this is not a car and non giant enemy, you can allow them to hit you twice
		if(_hitters.Contains(hitter)) return false;
		
		_hitters.Add(hitter);
		hitsReceived++;
		
		if(healthCanvas) healthCanvas.ReduceHealth();
		return true;
	}

	public void AddGrabbedCar(Transform car)
	{
		_hitters.Add(car);
	}

	public void VisibilityToggle(bool status)
	{
		healthCanvas.VisibilityToggle(status);
	}
}