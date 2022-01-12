using UnityEngine;

public class HealthCanvasController : MonoBehaviour
{
	[SerializeField] private Transform heartParent;

	private Canvas _canvas;
	private int _healthReduced;

	private void Start()
	{
		_canvas = GetComponent<Canvas>();
	}

	public void VisibilityToggle(bool status)
	{
		_canvas.enabled = status;
	}
	
	public void ReduceHealth()
	{
		heartParent.GetChild(heartParent.childCount - ++_healthReduced).gameObject.SetActive(false);
	}
}