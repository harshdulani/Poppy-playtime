using UnityEngine;

public class PalmController : MonoBehaviour
{
	[SerializeField] private HandController myHand;

	[SerializeField] private float punchWaitTime = 1f;

	private static Transform _lastPickedTarget;
	private static bool _canAdopt = true;
	private static int _punchIndex;
	
	private void OnEnable()
	{
		if(myHand.isLeftHand)
		{
			GameEvents.only.propDestroyed += OnPropDestroyed;
			GameEvents.only.giantPickupProp += OnPropDestroyed;
			GameEvents.only.dropArmor += OnDropArmor;
		}
		else
		{
			GameEvents.only.punchHit += OnPunchHit;
		}
	}

	private void OnDisable()
	{
		if(myHand.isLeftHand)
		{
			GameEvents.only.propDestroyed -= OnPropDestroyed;
			GameEvents.only.giantPickupProp -= OnPropDestroyed;
			GameEvents.only.dropArmor -= OnDropArmor;
		}
		else
		{
			GameEvents.only.punchHit -= OnPunchHit;
		}
	}

	private void Start()
	{
		_lastPickedTarget = null;
		_canAdopt = true;
	}

	private void EnablePunching() => myHand.StopPunching();

	private void ResetAdoptability()
	{
		_canAdopt = true;
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if(!_canAdopt) return;
		if(!other.CompareTag("Target") && !other.CompareTag("TrapButton") && !other.CompareTag("ChainLink")) return;
		
		//AudioManager play sound
		if (!myHand.isLeftHand) return;
		
		if (other.TryGetComponent(out PropController prop))
		{
			prop.PlayerPicksUp();
			prop.TryShowAds();
			if (prop.IsACompositeProp)
			{
				prop.MakeKinematic();
				prop.GetTouchedComposite(Vector3.up, false);
			}
		}
		
		if(other.CompareTag("TrapButton"))
		{
			myHand.HandReachTarget(other.transform);
			
			Invoke(nameof(EnablePunching), punchWaitTime);
			Invoke(nameof(ResetAdoptability), 0.5f);
			return;
		}

		if (other.CompareTag("ChainLink"))
		{
			other.GetComponent<ChainLink>().TryBreakPlatformUsingPalm(transform.position);
			
			myHand.HandReachTarget(other.transform);
			
			Invoke(nameof(EnablePunching), punchWaitTime);
			Invoke(nameof(ResetAdoptability), 0.5f);
			return;
		}
		
		if(HasTargetTransform())
		{
			InputHandler.Only.AssignReturnTransitState();
			Invoke(nameof(EnablePunching), punchWaitTime);
			Invoke(nameof(ResetAdoptability), 0.5f);
			return;
		}
		
		SetCurrentTransform(other.transform);
		myHand.HandReachTarget(other.transform);
		_canAdopt = false;
	}

	private void OnDropArmor()
	{
		SetCurrentTransform(null);
		Invoke(nameof(ResetAdoptability), 0.5f);
	}
	
	private void OnPunchHit()
	{
		if (!GetCurrentTransform())
		{
			InputHandler.Only.AssignIdleState();
			
			Invoke(nameof(EnablePunching), punchWaitTime);
			Invoke(nameof(ResetAdoptability), 0.5f);
			return;
		}
		
		myHand.HandReachTarget(GetCurrentTransform());

		var trans = GetCurrentTransform();

		SetCurrentTransform(null);
		
		Invoke(nameof(EnablePunching), punchWaitTime);
		Invoke(nameof(ResetAdoptability), 0.5f);
		HandController.Sounds.PlaySound(HandController.Sounds.punch[_punchIndex++ % HandController.Sounds.punch.Length], 1f);
		
		//this is for climber level
		ShatterableParent.AddToPossibleShatterers(trans.root);
	}

	private void OnPropDestroyed(Transform target)
	{
		if(target != GetCurrentTransform()) return;
		
		SetCurrentTransform(null);
		myHand.OnPropDestroyed();
		Invoke(nameof(ResetAdoptability), 0.5f);
	}

	private static bool HasTargetTransform() => _lastPickedTarget;
	private static Transform GetCurrentTransform() => _lastPickedTarget;
	private static void SetCurrentTransform(Transform newT) => _lastPickedTarget = newT;
}