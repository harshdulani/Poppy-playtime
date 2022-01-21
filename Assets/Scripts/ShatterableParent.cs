using System.Collections.Generic;
using UnityEngine;

public class ShatterableParent : MonoBehaviour
{
	public bool isShattered, shouldUnparent;
	[SerializeField] private Shatterable[] theShatterables;
	[SerializeField] private MeshRenderer overlapCube;

	private static List<Transform> _possibleShatterers = new List<Transform>();
	
	[SerializeField] private float explosionRadius, explosionForce;
	[SerializeField] private bool showOverlapBoxDebug;

	private void OnEnable()
	{
		GameEvents.only.punchHit += OnPunchHit;
	}
	
	private void OnDisable()
	{
		GameEvents.only.punchHit -= OnPunchHit;
	}

	private void OnDrawGizmos()
	{
		if(!showOverlapBoxDebug) return;
		
		Gizmos.color = new Color(0f, 0.75f, 1f, 0.5f);
		Gizmos.DrawCube(overlapCube.bounds.center, overlapCube.bounds.extents * 2);
	}
	
	public void ShatterTheShatterables(Vector3 point)
	{
		GameEvents.only.InvokeRayfireShattered(transform);

		foreach (var shatterable in theShatterables)
			shatterable.Shatter(point, explosionForce, explosionRadius);
		
		foreach (var collider in Physics.OverlapBox(overlapCube.bounds.center, overlapCube.bounds.extents * 2, overlapCube.transform.rotation))
			if (collider.transform.root.TryGetComponent(out RagdollController raghu))
				raghu.GoRagdoll(collider.transform.position - point);
	}

	private void OnPunchHit()
	{
		foreach (var shatterable in theShatterables)
			shatterable.MakeShatterable();
	}

	public static void AddToPossibleShatterers(Transform possibleShatterer)
	{
		if(IsThisAPossibleShatterer(possibleShatterer)) return;
		_possibleShatterers.Add(possibleShatterer);
	}

	public static bool IsThisAPossibleShatterer(Transform transformRoot)
	{
		return _possibleShatterers.Contains(transformRoot);
	}
}