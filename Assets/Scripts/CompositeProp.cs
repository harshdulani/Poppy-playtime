using UnityEngine;

public class CompositeProp : MonoBehaviour
{
	private bool _isDestroyed;
	[SerializeField] private PropController[] props;
	
	[SerializeField] private BoxBounds overlapBoxBounds;
	[SerializeField] private bool showOverlapBoxDebug;
	
	private void OnDrawGizmos()
	{
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
	
	public void StopBeingKinematic(Vector3 direction, Transform caller)
	{
		//this direction will also be applied to any others reacting to it
		if(_isDestroyed) return;

		_isDestroyed = true;
		foreach (var prop in props) 
			if(prop.transform != caller) 
				prop.Collapse(direction.normalized);
		
		var colliders = Physics.OverlapBox(transform.position + transform.up * overlapBoxBounds.distance, 
			new Vector3(overlapBoxBounds.x / 2, overlapBoxBounds.y / 2, overlapBoxBounds.z / 2),
			transform.rotation);

		foreach (var item in colliders)
		{
			if(!item.CompareTag("Target")) continue;

			if (item.TryGetComponent(out RagdollLimbController raghu))
				raghu.GetPunched((item.transform.position - transform.position).normalized, 1f);
			if (item.TryGetComponent(out PropController leg) && leg.IsACompositeProp)
				leg.GetTouchedComposite(item.transform.position - transform.position, true);
		}
	}
}