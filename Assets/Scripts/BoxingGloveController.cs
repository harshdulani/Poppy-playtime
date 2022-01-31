using UnityEngine;

public class BoxingGloveController : MonoBehaviour
{
	[SerializeField] private Transform desiredPosition;

	private void Update()
	{
		if (desiredPosition)
			transform.position = desiredPosition.position;
	}
}