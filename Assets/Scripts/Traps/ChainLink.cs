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

		if(!other.collider.CompareTag("Target")) return;

		if(other.collider.TryGetComponent(out RagdollLimbController raghuvendra))
			if(!raghuvendra.IsRaghuRagdolling())
				return;

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