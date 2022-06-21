using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BonusLevelButton : MonoBehaviour
{
	[SerializeField] private int myBuildIndexOffset; 
	
	[SerializeField] private Transform star;
	[SerializeField] private float maxScale = 1.25f, maxStarRotation, starRotationLoopDuration;

	private void Start()
	{
		transform.DOScale(transform.localScale * maxScale, 1f).SetLoops(-1, LoopType.Yoyo);

		var seq = DOTween.Sequence();

		seq.Append(star.DORotate(Vector3.forward * maxStarRotation, starRotationLoopDuration, RotateMode.WorldAxisAdd));
		seq.Append(star.DORotate(Vector3.forward * -maxStarRotation, starRotationLoopDuration, RotateMode.WorldAxisAdd));
		
		seq.SetLoops(-1, LoopType.Yoyo);
	}

	public void GoToBonusLevelOnButtonPress()
	{
		SceneChangeManager.JustIncreaseCurrentLevelNo();
		SceneManager.LoadScene(GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<MainCanvasController>().lastRegularLevel + myBuildIndexOffset);
	}
}