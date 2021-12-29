using UnityEngine;

public class TutorialCanvasController : MonoBehaviour
{
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
