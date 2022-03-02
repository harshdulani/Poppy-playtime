using UnityEngine;

public class HoldContainerUpHelper : MonoBehaviour
{
	public Rigidbody myContainer;
	
	private void OnEnable()
	{
		GameEvents.only.dropContainer += OnDropContainer;
	}

	private void OnDisable()
	{
		GameEvents.only.dropContainer -= OnDropContainer;
	}

	private void OnDropContainer(Rigidbody container)
	{
		if(container != myContainer) return;

		myContainer.isKinematic = false;
		myContainer.GetComponent<Collider>().enabled = true;
	}
}