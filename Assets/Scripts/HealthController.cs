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
		var result = hitsReceived >= hitsRequiredToKill;
		if (result)
			healthCanvas.enabled = false;
		
		return result;
	}

	public bool AddHit(bool careAboutCooldown = true)
	{
		if(careAboutCooldown)
		//changing stuff here, if you have boss fight bugs, its probably because i broke them adding heli compatibility
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