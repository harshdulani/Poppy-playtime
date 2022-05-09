using System.Collections.Generic;
using UnityEngine;

public class ShatterableParent : MonoBehaviour
{
	public bool isShattered, shouldUnparent, shouldOnlyBeShatteredByLastEnemy;
	[SerializeField] private Shatterable[] theShatterables;
	[SerializeField] private MeshRenderer overlapCube;
	[SerializeField] private bool shouldPlayBrickAudioOnShatter;

	private static List<Transform> _possibleShatterers = new List<Transform>();
	
	[SerializeField] private bool showOverlapBoxDebug;

	private void OnEnable()
	{
		GameEvents.Only.PunchHit += OnPunchHit;
	}
	
	private void OnDisable()
	{
		GameEvents.Only.PunchHit -= OnPunchHit;
	}

	private void OnDrawGizmos()
	{
		if(!showOverlapBoxDebug) return;
		
		Gizmos.color = new Color(0f, 0.75f, 1f, 0.5f);
		Gizmos.DrawCube(overlapCube.bounds.center, overlapCube.bounds.extents * 2);
	}
	
	public void ShatterTheShatterables()
	{
		if(shouldOnlyBeShatteredByLastEnemy)
			if(!LevelFlowController.only.DidKillLastEnemyOfArea()) return;
		
		GameEvents.Only.InvokeRayfireShattered(transform);
		
		if(shouldPlayBrickAudioOnShatter)
		{
			AudioManager.instance.Play("BrickBreakHigh");
			AudioManager.instance.Play("BrickFall" + Random.Range(1, 3));
		}

		foreach (var shatterable in theShatterables)
			shatterable.Shatter();

		if (!overlapCube) return;
		
		foreach (var collider in Physics.OverlapBox(overlapCube.bounds.center, overlapCube.bounds.extents, overlapCube.transform.rotation))
			if (collider.transform.root.TryGetComponent(out RagdollController raghu))
				raghu.GoRagdoll(Vector3.up);
	}

	private void OnPunchHit()
	{
		foreach (var shatterable in theShatterables)
			shatterable.MakeShatterable();
	}

	public static void TryAddToPossibleShatterers(Transform possibleShatterer)
	{
		if(IsThisAPossibleShatterer(possibleShatterer)) return;
		_possibleShatterers.Add(possibleShatterer);
	}

	private static bool IsThisAPossibleShatterer(Transform transformRoot)
	{
		return _possibleShatterers.Contains(transformRoot);
	}
}