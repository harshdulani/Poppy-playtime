using UnityEngine;

public class PunchAnimationController : MonoBehaviour
{
	[SerializeField] private ParticleSystem punchFx;

	public void PunchSlowMoOnAnimation()
	{
		TimeController.only.RevertTime(HandController._isCarryingRagdoll && LevelFlowController.only.IsThisLastEnemy());
	}

	public void GivePunchOnAnimation()
	{
		GameEvents.only.InvokePunchHit();
		punchFx.Play();
	}
}
