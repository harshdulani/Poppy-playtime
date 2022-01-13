using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class BossPropController : MonoBehaviour
{
	[FormerlySerializedAs("barrelSpawnWaitTime")] [SerializeField] private float propSpawnWaitTime;
	private Coroutine _cycle;

	private void OnEnable()
	{
		GameEvents.only.reachNextArea += OnReachNextArea;
		GameEvents.only.gameEnd += StopSpawning;
		GameEvents.only.enemyKillPlayer += StopSpawning;
	}

	private void OnDisable()
	{
		GameEvents.only.reachNextArea -= OnReachNextArea;
		GameEvents.only.gameEnd -= StopSpawning;
		GameEvents.only.enemyKillPlayer -= StopSpawning;
	}

	private void OnReachNextArea()
	{
		if (!LevelFlowController.only.isGiantLevel) return;

		_cycle = StartCoroutine(ThrowingCycle());
	}
	
	private void StopSpawning()
	{
		StopCoroutine(_cycle);
	}

	private IEnumerator ThrowingCycle()
	{
		while (transform.childCount > 0)
		{
			SpawnProp();
			SpawnProp();
			
			yield return GameExtensions.GetWaiter(propSpawnWaitTime);
		}
	}

	private void SpawnProp()
	{
		var prop = transform.GetChild(Random.Range(0, transform.childCount)).gameObject;
		var oldPosY = prop.transform.position.y;
		prop.transform.position += Vector3.up * 10f;
		prop.SetActive(true);

		prop.transform.DOMoveY(oldPosY, 0.3f);
			
		prop.transform.parent = null;
	}
}