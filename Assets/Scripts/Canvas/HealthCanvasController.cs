using UnityEngine;
using UnityEngine.UI;

public class HealthCanvasController : MonoBehaviour
{
	[SerializeField] private Transform heartParent;
	[SerializeField] private Sprite hollowHeart;
	
	private Canvas _canvas;
	private int _healthReduced;

	private void Awake()
	{
		_canvas = GetComponent<Canvas>();
	}

	public void VisibilityToggle(bool status)
	{
		_canvas.enabled = status;
	}
	
	public void ReduceHealth()
	{
		heartParent.GetChild(heartParent.childCount - ++_healthReduced).GetComponent<Image>().sprite = hollowHeart;
	}
}