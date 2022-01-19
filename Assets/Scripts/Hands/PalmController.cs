using UnityEngine;

public class PalmController : MonoBehaviour
{
	[SerializeField] private HandController myHand;

	[SerializeField] private float punchWaitTime = 1f;

	private static Transform _lastPickedTarget;
	private static bool _canBeAdopted = true;
	private static int _punchIndex;

	public static string guitext; 
	
	private void OnEnable()
	{
		if(myHand.isLeftHand)
		{
			GameEvents.only.propDestroyed += OnPropDestroyed;
			GameEvents.only.giantPickupCar += OnPropDestroyed;
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
			GameEvents.only.giantPickupCar -= OnPropDestroyed;
		}
		else
		{
			GameEvents.only.punchHit -= OnPunchHit;
		}
	}

	private void Start()
	{
		_lastPickedTarget = null;
		_canBeAdopted = true;
	}

	private void OnGUI()
	{
		if (!myHand.isLeftHand) return;
		GUI.contentColor = Color.black;
		GUI.skin.label.fontSize = 36;
		GUI.Label(new Rect(20, 20, 500, 400), guitext);
	}

	private void OnTriggerEnter(Collider other)
	{
		if(!_canBeAdopted) return;
		if(!other.CompareTag("Target")) return;
		
		//AudioManager play sound
		if (!myHand.isLeftHand) return;
	
		if(HasTargetTransform()) return;
		
		SetCurrentTransform(other.transform);
		guitext = GetCurrentTransform().ToString();
		myHand.HandReachTarget(other.transform);
		_canBeAdopted = false;
	}

	private void EnablePunching()
	{
		myHand.StopPunching();
	}

	private void OnPunchHit()
	{
		myHand.HandReachTarget(GetCurrentTransform());
		
		//this is for climber level
		ShatterableParent.AddToPossibleShatterers(GetCurrentTransform().root);

		SetCurrentTransform(null);
		guitext = GetCurrentTransform() ? GetCurrentTransform().ToString() : "null";
		Invoke(nameof(EnablePunching), punchWaitTime);
		Invoke(nameof(ResetAdoptability), 0.5f);
		HandController.Sounds.PlaySound(HandController.Sounds.punch[_punchIndex ++ % HandController.Sounds.punch.Length], 1f);
	}

	private void OnPropDestroyed(Transform target)
	{
		if(target != GetCurrentTransform()) return;
		
		SetCurrentTransform(null);
		myHand.OnPropDestroyed();
		Invoke(nameof(ResetAdoptability), 0.5f);
	}

	private void ResetAdoptability()
	{
		_canBeAdopted = true;
	}

	private static bool HasTargetTransform() => _lastPickedTarget;
	private static Transform GetCurrentTransform() => _lastPickedTarget;
	private static void SetCurrentTransform(Transform newT) => _lastPickedTarget = newT;
}