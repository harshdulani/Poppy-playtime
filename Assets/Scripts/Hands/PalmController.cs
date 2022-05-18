using DG.Tweening;
using UnityEngine;

public class PalmController : MonoBehaviour
{
	[SerializeField] private HandController myHand;

	[SerializeField] private float punchWaitTime = 1f;

	private static Transform _lastPickedTarget;
	private Vector3 _initLocalPos;
	private static bool _canAdopt = true;
	private static int _punchIndex;
	
	private void OnEnable()
	{
		if(myHand.isLeftHand)
		{
			GameEvents.Only.PropDestroyed += OnPropDestroyed;
			GameEvents.Only.GiantPickupProp += OnPropDestroyed;
			GameEvents.Only.DropArmor += OnDropArmor;
		}
		else
		{
			GameEvents.Only.PunchHit += OnPunchHit;
		}
	}

	private void OnDisable()
	{
		if(myHand.isLeftHand)
		{
			GameEvents.Only.PropDestroyed -= OnPropDestroyed;
			GameEvents.Only.GiantPickupProp -= OnPropDestroyed;
			GameEvents.Only.DropArmor -= OnDropArmor;
		}
		else
		{
			GameEvents.Only.PunchHit -= OnPunchHit;
		}
	}

	private void Start()
	{
		_lastPickedTarget = null;
		_canAdopt = true;

		_initLocalPos = transform.localPosition;
	}

	public Tween ReachPointInstantly(Vector3 point)
	{
		return transform.DOMove(point, 0.2f)
			.OnUpdate(HandController.ForceUpdateRope);
	}

	public Tween ReachHomeInstantly()
	{
		return transform.DOLocalMove(_initLocalPos, 0.2f)
			.OnUpdate(HandController.ForceUpdateRope);
	}

	private void EnablePunching() => myHand.StopPunching();

	private void ResetAdoptability() => _canAdopt = true;

	private void OnTriggerEnter(Collider other)
	{
		if(!_canAdopt) return;
		if (!myHand.isLeftHand) return;
		if(!other.CompareTag("Target") && !other.CompareTag("TrapButton") && !other.CompareTag("ChainLink")) return;

		if (other.TryGetComponent(out PropController prop))
		{
			prop.PlayerPicksUp();
			HandController.PropHeldToPunch = prop;

			prop.TryShowAds();
			if (prop.IsACompositeProp)
			{
				prop.MakeKinematic();
				prop.GetTouchedComposite(Vector3.up, false);
			}
		}
		else if (other.TryGetComponent(out WeaponPickup weapon))
		{
			weapon.PlayerInteractWithPickup();
			
			myHand.HandReachTarget(other.transform);
			
			DOVirtual.DelayedCall(punchWaitTime, EnablePunching);
			DOVirtual.DelayedCall(0.5f, ResetAdoptability);
			return;
		}
		
		if(other.CompareTag("TrapButton"))
		{
			myHand.HandReachTarget(other.transform);
			
			DOVirtual.DelayedCall(punchWaitTime, EnablePunching);
			DOVirtual.DelayedCall(0.5f, ResetAdoptability);
			return;
		}

		if (other.CompareTag("ChainLink"))
		{
			other.GetComponent<ChainLink>().TryBreakPlatformUsingPalm(transform.position);
			
			myHand.HandReachTarget(other.transform);
			
			DOVirtual.DelayedCall(punchWaitTime, EnablePunching);
			DOVirtual.DelayedCall(0.5f, ResetAdoptability);
			return;
		}
		
		if(HasTargetTransform())
		{
			InputHandler.AssignReturnTransitState();
			DOVirtual.DelayedCall(punchWaitTime, EnablePunching);
			DOVirtual.DelayedCall(0.5f, ResetAdoptability);
			return;
		}

		var otherTransform = other.transform;
		HandController.TargetHeldToPunch = otherTransform.root;
		SetCurrentTransform(otherTransform);
		myHand.HandReachTarget(otherTransform);
		_canAdopt = false;
	}

	public void EnableAdoptability()
	{
		DOVirtual.DelayedCall(punchWaitTime, EnablePunching);
		DOVirtual.DelayedCall(0.5f, ResetAdoptability);
	}

	private void OnDropArmor()
	{
		HandController.TargetHeldToPunch = null;
		SetCurrentTransform(null);
		DOVirtual.DelayedCall(0.5f, ResetAdoptability);
	}
	
	private void OnPunchHit()
	{
		if (!GetCurrentTransform())
		{
			InputHandler.Only.AssignIdleState();
			
			DOVirtual.DelayedCall(punchWaitTime, EnablePunching);
			DOVirtual.DelayedCall(0.5f, ResetAdoptability);
			return;
		}

		var trans = GetCurrentTransform();
		myHand.HandReachTarget(trans);
		
		Tween tween = null;
		tween = DOVirtual.DelayedCall(0.25f, () =>
		{
			//first is for old throw it back to where you picked it up from
			//second condition is for the newer throw in player forward direction smash mechanic

			//so it makes sure you're only waiting the 0.25 secs if you are waiting to punch
			if (InputHandler.Only.canDragAfterGrabbingToAim && HandController.TargetHeldToPunch) return;

			tween.Kill(true);
		}).OnComplete(() =>
		{
			HandController.TargetHeldToPunch = null;
			SetCurrentTransform(null);
	
			DOVirtual.DelayedCall(punchWaitTime, EnablePunching);
			DOVirtual.DelayedCall(0.5f, ResetAdoptability);
			HandController.Sounds.PlaySound(HandController.Sounds.punch[_punchIndex++ % HandController.Sounds.punch.Length], 1f);
	
			//this is for climber level
			ShatterableParent.TryAddToPossibleShatterers(trans.root);
		});
	}

	private void OnPropDestroyed(Transform target)
	{
		if(target != GetCurrentTransform()) return;
		
		HandController.TargetHeldToPunch = null;
		SetCurrentTransform(null);
		myHand.OnPropDestroyed();
		DOVirtual.DelayedCall(0.5f, ResetAdoptability);
	}

	private static bool HasTargetTransform() => _lastPickedTarget;
	private static Transform GetCurrentTransform() => _lastPickedTarget;
	private static void SetCurrentTransform(Transform newT) => _lastPickedTarget = newT;
}