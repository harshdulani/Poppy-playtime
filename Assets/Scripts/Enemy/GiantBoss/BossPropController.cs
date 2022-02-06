using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class BossPropController : MonoBehaviour
{
	[FormerlySerializedAs("barrelSpawnWaitTime")] [SerializeField] private float propSpawnWaitTime;
	[SerializeField] private int myArea, spawnAtATime;
	private Coroutine _cycle;

	private bool _isSpawning;

	private void OnEnable()
	{
		GameEvents.only.tapToPlay += OnTapToPlay;
		GameEvents.only.reachNextArea += OnReachNextArea;
		GameEvents.only.gameEnd += StopSpawning;
		GameEvents.only.enemyKillPlayer += StopSpawning;
	}

	private void OnDisable()
	{
		GameEvents.only.tapToPlay -= OnTapToPlay;
		GameEvents.only.reachNextArea -= OnReachNextArea;
		GameEvents.only.gameEnd -= StopSpawning;
		GameEvents.only.enemyKillPlayer -= StopSpawning;
	}
	
	private void StopSpawning()
	{
		if(!_isSpawning) return;
		
		StopCoroutine(_cycle);
		_isSpawning = false;
	}

	private IEnumerator ThrowingCycle()
	{			
		yield return GameExtensions.GetWaiter(1f);

		while (transform.childCount > 0)
		{
			for(var i = 0; i < spawnAtATime; i++)
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
	
	private void OnTapToPlay()
	{
		OnReachNextArea();
	}

	private void OnReachNextArea()
	{
		if(myArea != LevelFlowController.only.currentArea) return;

		_cycle = StartCoroutine(ThrowingCycle());
		_isSpawning = true;
	}
}