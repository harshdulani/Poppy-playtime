using UnityEngine;

public class TutorialCanvasController : MonoBehaviour
{
	public enum TutorialType
	{
		
	}
	
	private void OnEnable()
	{
		GameEvents.only.tapToPlay += OnTapToPlay;
	}

	private void OnDisable()
	{
		GameEvents.only.tapToPlay -= OnTapToPlay;
	}

	private void OnTapToPlay()
	{
		gameObject.SetActive(false);
	}
}
