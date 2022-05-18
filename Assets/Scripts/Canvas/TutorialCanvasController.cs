using System;
using System.Collections;
using UnityEngine;

public enum TutorialType
{
	Aiming,
	Punching,
	Barrel,
	Boss,
	DragToSmash
}
public class TutorialCanvasController : MonoBehaviour
{
	[HideInInspector] public bool knowsHowToPickUpCars;
	[SerializeField] private TutorialType myType;
	[SerializeField] private GameObject toDisable, secondHelp;

	[SerializeField] private AnimationClip aimingClip, tappingClip;

	private const float SecondsToDrag = 1;
	private float _draggedSeconds;
	private bool _hasLearned;
	
	private bool _bossTutorialAnimationOn;
	[SerializeField] private Animation anim;

	private void OnEnable()
	{
		switch (myType)
		{
			case TutorialType.Boss:
				GameEvents.Only.ReachNextArea += OnReachNextArea;
				GameEvents.Only.GiantLanding += OnGiantLand;
				break;
			case TutorialType.Aiming:
				GameEvents.Only.TapToPlay += OnTapToPlay;
				break;
			case TutorialType.Punching:
				GameEvents.Only.EnterHitBox += OnEnterHitBox;
				GameEvents.Only.PunchHit += OnPunchHit;
				break;
			case TutorialType.Barrel:
				GameEvents.Only.EnterHitBox += OnEnterHitBox;
				break;
			case TutorialType.DragToSmash:
				GameEvents.Only.EnterHitBox += OnEnterHitBox;
				GameEvents.Only.PunchHit += OnPunchHit;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void OnDisable()
	{
		switch (myType)
		{
			case TutorialType.Boss:
				GameEvents.Only.ReachNextArea -= OnReachNextArea;
				GameEvents.Only.GiantLanding -= OnGiantLand;
				break;
			case TutorialType.Aiming:
				GameEvents.Only.TapToPlay -= OnTapToPlay;
				break;
			case TutorialType.Punching:
				GameEvents.Only.EnterHitBox -= OnEnterHitBox;
				GameEvents.Only.PunchHit -= OnPunchHit;
				break;
			case TutorialType.Barrel:
				GameEvents.Only.EnterHitBox -= OnEnterHitBox;
				break;
			case TutorialType.DragToSmash:
				GameEvents.Only.EnterHitBox -= OnEnterHitBox;
				GameEvents.Only.PunchHit -= OnPunchHit;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void Start()
	{
		switch (myType)
		{
			case TutorialType.Aiming:
				anim.Play(InputHandler.Only.isUsingTapAndPunch ? tappingClip.name : aimingClip.name);
				break;
			case TutorialType.DragToSmash:
				toDisable.SetActive(false);
				secondHelp.SetActive(false);
				break;
			case TutorialType.Boss:
				toDisable.SetActive(false);
				break;
		}
	}

	private void Update()
	{
		switch (myType)
		{
			case TutorialType.DragToSmash:
			{
				if(_hasLearned) return;

				if (_draggedSeconds > SecondsToDrag)
				{
					_hasLearned = true;
					_draggedSeconds = 0f;
					toDisable.SetActive(false);
					secondHelp.SetActive(true);
				}
				else if(InputExtensions.GetFingerHeld())
						_draggedSeconds += Time.deltaTime;
				break;
			}
			case TutorialType.Boss:
				if (!_bossTutorialAnimationOn) return;
				if(knowsHowToPickUpCars) return;
		
				if(!InputExtensions.GetFingerDown()) return;
		
				TimeController.only.RevertTime();
				toDisable.SetActive(false);
				_bossTutorialAnimationOn = false;
				InputHandler.Only.AssignIdleState();
				break;
		}
	}

	private void OnTapToPlay()
	{
		switch (myType)
		{
			case TutorialType.Aiming:
				gameObject.SetActive(false);
				break;
		}
	}

	private void OnEnterHitBox(Transform target)
	{
		switch (myType)
		{
			case TutorialType.Barrel:
				gameObject.SetActive(false);
				break;
			case TutorialType.DragToSmash:
				toDisable.SetActive(true);
				secondHelp.SetActive(false);
				_hasLearned = false;
				break;
		}
		
	}

	public void PlayerKnowsHowToPickUpCars()
	{
		knowsHowToPickUpCars = true;
		toDisable.SetActive(false);
		_bossTutorialAnimationOn = false;
		TimeController.only.RevertTime();
	}

	private void OnPunchHit()
	{
		switch (myType)
		{
			case TutorialType.DragToSmash:
				toDisable.SetActive(false);
				secondHelp.SetActive(false);
				break;
		}
	}

	private void OnReachNextArea()
	{
		if(!LevelFlowController.only.IsInGiantFight()) return;
		
		InputHandler.AssignTemporaryDisabledState();
	}
	
	private void OnGiantLand(Transform giant)
	{
		if(knowsHowToPickUpCars) return;
		StartCoroutine(Wait());
	}

	private IEnumerator Wait()
	{
		yield return GameExtensions.GetWaiter(2f);
		if(knowsHowToPickUpCars) yield break;
		
		toDisable.SetActive(true);
		TimeController.only.SlowDownTime(.1f);

		_bossTutorialAnimationOn = true;
	}
}