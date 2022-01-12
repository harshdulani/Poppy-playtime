using UnityEngine;

public class HealthCanvasController : MonoBehaviour
{
	[SerializeField] private Transform heartParent;

	private int _healthReduced;
	
	public void ReduceHealth()
	{
		heartParent.GetChild(heartParent.childCount - ++_healthReduced).gameObject.SetActive(false);
	}
}