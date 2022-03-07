using UnityEngine;

public class ChainLink : MonoBehaviour
{
	private ChainLinkPlatform _parent;
	private bool _broken;

	private void Start()
	{
		_parent = transform.parent.GetComponent<ChainLinkPlatform>();
	}

	private void OnCollisionEnter(Collision other)
	{
		if (_broken) return;
		
		//after enemies become ragdolls, their limbs lose their Target tag, but props still have the target tag till they are destroyed, which will ideally always be after it collides with a chain link 
		if(!other.collider.CompareTag("Target") && !other.collider.CompareTag("Untagged")) return;
		
		//only people that are allowed interactions with this are a thrown prop or a thrown ragdoll
		//so if it collides with a ragdoll, and it is not ragdolling, return
		//otherwise
		//if it collides with an object that is not a ragdoll, check if it is a prop
		//if it is neither, return
		if(other.collider.TryGetComponent(out RagdollLimbController raghuvendra))
		{
			if (!raghuvendra.IsRaghuRagdolling()) return;
		}
		else if(!other.collider.TryGetComponent(out PropController _)) return;

		if(_parent)
			_parent.BreakChainLinkUsingBarrel(gameObject, other.transform.position);
		_broken = true;
	}

	public bool IsInCurrentArea() => LevelFlowController.only.currentArea == _parent.myArea;

	public void TryBreakPlatformUsingPalm(Vector3 palmPos)
	{
		if (_broken) return;

		_broken = true;
		_parent.BreakChainUsingPalm(palmPos);
		
		GetComponent<HingeJoint>().connectedBody = null;
	}
}