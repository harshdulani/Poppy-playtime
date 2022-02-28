using DG.Tweening;
using UnityEngine;

public class HealthController : MonoBehaviour
{
	public int hitsRequiredToKill, hitsReceived;

	[SerializeField] private HealthCanvasController healthCanvas;

	private const float HitCooldown = 1f;
	private bool _isInHitCooldown;
	
	public bool IsDead()
	{
		print(hitsReceived + ", " + hitsRequiredToKill);
		var result = hitsReceived >= hitsRequiredToKill;
		if (result)
			healthCanvas.enabled = false;
		
		return result;
	}

	/// <summary>
	/// Add a hit to the health and if exists, the health canvas
	/// </summary>
	/// <param name="careAboutCooldown"> should delivering this hit care about the hit cooldown, i.e., avoiding giving hits too quickly</param>
	/// <returns>Return whether player was allowed to deliver a hit</returns>
	public bool AddHit(bool careAboutCooldown = true)
	{
		if(careAboutCooldown)
			if(_isInHitCooldown) return false;

		VisibilityToggle(true);

		_isInHitCooldown = true;
		DOVirtual.DelayedCall(HitCooldown, () => _isInHitCooldown = false);
		hitsReceived++;
		
		if(healthCanvas) healthCanvas.ReduceHealth();
		return true;
	}

	public void VisibilityToggle(bool status)
	{
		healthCanvas.VisibilityToggle(status);
	}
}