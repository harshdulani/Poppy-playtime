using UnityEngine;
using UnityEngine.UI;

public class AimController : MonoBehaviour
{
	[SerializeField] private float aimSpeedVertical, aimSpeedHorizontal;
	[SerializeField] private Color findTargetColor, missingTargetColor;
	[SerializeField] private Image reticle;

	public void SetReticleStatus(bool isOn)
	{
		reticle.enabled = isOn;
	}

	public void FindTarget()
    {
	    reticle.color = findTargetColor;
    }

    public void LoseTarget()
    {
	    reticle.color = missingTargetColor;
    }

    public void Aim(Vector2 inputDelta)
    {
	    transform.Rotate(new Vector3(
		                     inputDelta.y * aimSpeedVertical,
		                     inputDelta.x * aimSpeedHorizontal, 
		                     0f) * Time.deltaTime);
    }
}
