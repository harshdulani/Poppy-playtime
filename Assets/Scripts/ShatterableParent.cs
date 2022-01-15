using System.Collections.Generic;
using UnityEngine;

public class ShatterableParent : MonoBehaviour
{
	public bool isShattered, shouldUnparent;
	[SerializeField] private Shatterable[] theShatterables;

	private static List<Transform> _possibleShatterers = new List<Transform>();
	
	[SerializeField] private float explosionRadius, explosionForce;

	private void OnEnable()
	{
		GameEvents.only.punchHit += OnPunchHit;
	}
	
	private void OnDisable()
	{
		GameEvents.only.punchHit -= OnPunchHit;
	}

	public void ShatterTheShatterables(Vector3 point)
	{
		GameEvents.only.InvokeRayfireShattered(transform);

		foreach (var shatterable in theShatterables)
			shatterable.Shatter(point, explosionForce, explosionRadius);
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