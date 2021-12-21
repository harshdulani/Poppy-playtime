using UnityEngine;
using UnityEngine.UI;

public class AimController : MonoBehaviour
{
	[SerializeField] private float aimSpeedVertical, aimSpeedHorizontal, clampAngleVertical, clampAngleHorizontal;
	[SerializeField] private Color findTargetColor, missingTargetColor;
	[SerializeField] private Image reticle;

	private float _rotX, _rotY;
	
	private void Start ()
	{
		Vector3 rot = transform.eulerAngles;
		_rotY = rot.y;
		_rotX = rot.x;
	}
	
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
		_rotY += inputDelta.x * aimSpeedHorizontal * Time.deltaTime;
		_rotX -= inputDelta.y * aimSpeedVertical * Time.deltaTime;
 
		_rotY = Mathf.Clamp(_rotY, -clampAngleHorizontal, clampAngleHorizontal);
		_rotX = Mathf.Clamp(_rotX, -clampAngleVertical, clampAngleVertical);
 
		var newRot = Quaternion.Euler(_rotX, _rotY, 0.0f);
		transform.rotation = newRot;
	}
}
