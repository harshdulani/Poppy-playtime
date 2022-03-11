using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BonusLevelButton : MonoBehaviour
{
	[SerializeField] private float maxScale = 1.25f;

	private void Start()
	{
		transform.DOScale(transform.localScale * maxScale, 1f).SetLoops(-1, LoopType.Yoyo);
	}

	public void GoToBonusLevelOnButtonPress(int buildIndex)
	{
		SceneManager.LoadScene(buildIndex);
	}
}