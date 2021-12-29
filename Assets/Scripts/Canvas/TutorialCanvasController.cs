using UnityEngine;

public class TutorialCanvasController : MonoBehaviour
{
	[SerializeField] private TutorialType myType;
	[SerializeField] private GameObject toDisable;
	
	private void OnEnable()
	{
		if (myType != TutorialType.Punching)
			GameEvents.only.tapToPlay += OnTapToPlay;
		else
		{
			GameEvents.only.enterHitBox += OnEnterHitBox;
			GameEvents.only.punchHit += OnPunchHit;
		}
	}

	private void OnDisable()
	{
		if(myType != TutorialType.Punching)
			GameEvents.only.tapToPlay -= OnTapToPlay;
		else
		{
			GameEvents.only.enterHitBox -= OnEnterHitBox;
			GameEvents.only.punchHit -= OnPunchHit;
		}
	}

	private void OnTapToPlay()
	{
		gameObject.SetActive(false);
	}

	private void OnEnterHitBox(Transform target)
	{
		toDisable.SetActive(true);
	}

	private void OnPunchHit()
	{
		toDisable.SetActive(false);
	}
}
public enum TutorialType
{
	Aiming,
	Punching,
	Barrel
}