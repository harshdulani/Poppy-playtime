using UnityEngine;

public class HoldContainerUpHelper : MonoBehaviour
{
	public Rigidbody myContainer;
	
	private void OnEnable()
	{
		GameEvents.Only.DropContainer += OnDropContainer;
	}

	private void OnDisable()
	{
		GameEvents.Only.DropContainer -= OnDropContainer;
	}

	private void OnDropContainer(Rigidbody container)
	{
		if(container != myContainer) return;

		myContainer.isKinematic = false;
		myContainer.GetComponent<Collider>().enabled = true;
	}
}