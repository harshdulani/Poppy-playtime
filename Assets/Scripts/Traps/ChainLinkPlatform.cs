using UnityEngine;

public class ChainLinkPlatform : MonoBehaviour
{
	public int myArea;
	[SerializeField] private bool detectCollisionsOnBreaking, showOverlapBoxDebug;
	[SerializeField] private BoxBounds overlapBoxBounds;
	[SerializeField] private Rigidbody basePlatform;
	
	private void OnDrawGizmos()
    {
    	if(!detectCollisionsOnBreaking) return;
    	if(!showOverlapBoxDebug) return;
    	
    	// cache previous Gizmos settings
    	var prevColor = Gizmos.color;
    	Matrix4x4 prevMatrix = Gizmos.matrix;

    	Gizmos.color = new Color(0f, 0.75f, 1f, 0.5f);
    	Gizmos.matrix = transform.localToWorldMatrix;

    	var boxPosition = transform.position + transform.up * overlapBoxBounds.distance;

    	// convert from world position to local position 
    	boxPosition = transform.InverseTransformPoint(boxPosition); 

    	var boxSize = new Vector3(overlapBoxBounds.x, overlapBoxBounds.y, overlapBoxBounds.z);
    	Gizmos.DrawCube(boxPosition, boxSize);

    	// restore previous Gizmos settings
    	Gizmos.matrix = prevMatrix;
    }
	
	public void BreakChainLinkUsingBarrel(GameObject caller, Vector3 otherPos)
	{
		HingeJoint hinge = null;
		var sqrDistFromMe = 99999f;
		foreach (var joint in caller.GetComponents<HingeJoint>())
		{
			var sqrMag = (joint.connectedBody.position - otherPos).sqrMagnitude;
			if(sqrMag > sqrDistFromMe) continue;
			
			hinge = joint;
			sqrDistFromMe = sqrMag;
		}
		
		if(!hinge) caller.TryGetComponent(out hinge);

		if(hinge)
		{
			hinge.connectedBody = null;
			hinge.breakForce = 0.01f;
		}
		caller.TryGetComponent(out Rigidbody rigid);
		rigid.isKinematic = false;
		basePlatform.isKinematic = false;

		var colliders = Physics.OverlapBox(transform.position + transform.up * overlapBoxBounds.distance, 
			new Vector3(overlapBoxBounds.x / 2, overlapBoxBounds.y / 2, overlapBoxBounds.z / 2),
			transform.rotation);
		

		foreach (var item in colliders)
		{
			if(!item.CompareTag("Target") && !item.CompareTag("Trap")) continue;

			if (item.TryGetComponent(out RagdollLimbController raghu))
				raghu.GetPunched(-(item.transform.position - transform.position).normalized, 1f);
			
			if(item.TryGetComponent(out TrapAttacker attacker))
				attacker.MakeNonKinematic(-(item.transform.position - transform.position).normalized);
		}
	}

	public void BreakChainUsingPalm(Vector3 palmPos)
	{
		HingeJoint hinge = null;
		var sqrDistFromMe = 99999f;
		foreach (var joint in basePlatform.GetComponents<HingeJoint>())
		{
			//yes we return here and not continue because then the trigger is being called multiple times in the same frame(s)
			//and trying to break the platform even after it is broken
			if(!joint.connectedBody) return;
			
			var sqrMag = (joint.connectedBody.position - palmPos).sqrMagnitude;
			if(sqrMag > sqrDistFromMe) continue;
			
			hinge = joint;
			sqrDistFromMe = sqrMag;
		}
		
		if(!hinge) basePlatform.TryGetComponent(out hinge);

		if(hinge)
		{
			hinge.connectedBody = null;
			hinge.useLimits = false;
			hinge.breakForce = 0.001f;
		}
		basePlatform.isKinematic = false;

		var colliders = Physics.OverlapBox(transform.position + transform.up * overlapBoxBounds.distance, 
			new Vector3(overlapBoxBounds.x / 2, overlapBoxBounds.y / 2, overlapBoxBounds.z / 2),
			transform.rotation);
		

		foreach (var item in colliders)
		{
			if(!item.CompareTag("Target") && !item.CompareTag("Trap")) continue;

			if (item.TryGetComponent(out RagdollLimbController raghu))
				raghu.GetPunched(-(item.transform.position - transform.position).normalized, 1f);
			
			if(item.TryGetComponent(out TrapAttacker attacker))
				attacker.MakeNonKinematic(-(item.transform.position - transform.position).normalized);
		}
	}
}