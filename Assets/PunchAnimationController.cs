using UnityEngine;

public class PunchAnimationController : MonoBehaviour
{
	public void GivePunchOnAnimation()
	{
		GameEvents.only.InvokePunchHit();
	}
}
