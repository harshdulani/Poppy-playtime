using System.Collections;
using UnityEngine;

public enum TutorialType
{
	Aiming,
	Punching,
	Barrel,
	Boss
}
public class TutorialCanvasController : MonoBehaviour
{
	[SerializeField] private TutorialType myType;
	[SerializeField] private GameObject toDisable;

	[SerializeField] private AnimationClip aimingClip, tappingClip; 

	private bool _bossTutorialAnimationOn;
	[SerializeField] private Animation anim;

	private void OnEnable()
	{
		if (myType == TutorialType.Boss)
		{
			GameEvents.Only.ReachNextArea += OnReachNextArea;
			GameEvents.Only.GiantLanding += OnGiantLand;
		}
		if (myType != TutorialType.Punching)
			GameEvents.Only.TapToPlay += OnTapToPlay;
		else
		{
			GameEvents.Only.EnterHitBox += OnEnterHitBox;
			GameEvents.Only.PunchHit += OnPunchHit;
		}
	}

	private void OnDisable()
	{
		if (myType == TutorialType.Boss)
		{
			GameEvents.Only.ReachNextArea -= OnReachNextArea;
			GameEvents.Only.GiantLanding -= OnGiantLand;
		}
		if(myType != TutorialType.Punching)
			GameEvents.Only.TapToPlay -= OnTapToPlay;
		else
		{
			GameEvents.Only.EnterHitBox -= OnEnterHitBox;
			GameEvents.Only.PunchHit -= OnPunchHit;
		}
	}

	private void Start()
	{
		if (myType == TutorialType.Aiming)
			anim.Play(InputHandler.Only.isUsingTapAndPunch ? tappingClip.name : aimingClip.name);
		
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
		InputHandler.Only.AssignIdleState();
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

	private void OnReachNextArea()
	{
		if(!LevelFlowController.only.IsInGiantFight()) return;
		
		InputHandler.AssignTemporaryDisabledState();
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