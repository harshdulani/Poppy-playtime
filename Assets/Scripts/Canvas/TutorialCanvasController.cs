using System.Collections;
using UnityEngine;

public class TutorialCanvasController : MonoBehaviour
{
	[SerializeField] private TutorialType myType;
	[SerializeField] private GameObject toDisable;

	private bool _bossTutorialAnimationOn; 
	
	private void OnEnable()
	{
		if (myType == TutorialType.Boss)
			GameEvents.only.giantLanding += OnGiantLand;
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
		if (myType == TutorialType.Boss)
			GameEvents.only.giantLanding -= OnGiantLand;
		if(myType != TutorialType.Punching)
			GameEvents.only.tapToPlay -= OnTapToPlay;
		else
		{
			GameEvents.only.enterHitBox -= OnEnterHitBox;
			GameEvents.only.punchHit -= OnPunchHit;
		}
	}

	private void Start()
	{
		if (myType == TutorialType.Boss)
			toDisable.SetActive(false);
	}

	private void Update()
	{
		if (!_bossTutorialAnimationOn) return;
		
		if (myType != TutorialType.Boss) return;

		if(!InputExtensions.GetFingerDown()) return;
		
		TimeController.only.RevertTime();
		toDisable.SetActive(false);
		_bossTutorialAnimationOn = false;
	}

	private void OnTapToPlay()
	{
		if(myType != TutorialType.Boss)
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
	
	private void OnGiantLand(Transform giant)
	{
		StartCoroutine(Wait());
	}

	private IEnumerator Wait()
	{
		yield return GameExtensions.GetWaiter(2f);
		toDisable.SetActive(true);
		TimeController.only.SlowDownTime(.1f);

		_bossTutorialAnimationOn = true;
	}
}
public enum TutorialType
{
	Aiming,
	Punching,
	Barrel,
	Boss
}