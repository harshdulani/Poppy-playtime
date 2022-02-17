using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MyPlayerController : MonoBehaviour
{
	[SerializeField] private Transform glassBreakPanel;
	[SerializeField] private float glassBreakPanelVisibleTime = 1.5f;

	private readonly List<Image> _bulletHole = new List<Image>();
	private HealthController _health;
	
	private void OnEnable()
	{
		GameEvents.only.enemyHitPlayer += OnEnemyHitPlayer;
		GameEvents.only.reachNextArea += OnReachNextArea;
	}

	private void OnDisable()
	{
		GameEvents.only.enemyHitPlayer -= OnEnemyHitPlayer;
		GameEvents.only.reachNextArea -= OnReachNextArea;
	}

	private void Start()
	{
		_health = GetComponent<HealthController>();
		_health.VisibilityToggle(false);

		for (var i = 0; i < glassBreakPanel.childCount; i++)
		{
			_bulletHole.Add(glassBreakPanel.GetChild(i).GetComponent<Image>());
			_bulletHole[i].enabled = false;
		}
	}

	private void OnEnemyHitPlayer(Transform hitter)
	{
		if(!_health.AddHit()) return;

		CameraController.only.ScreenShake(3f);
		
		glassBreakPanel.gameObject.SetActive(true);
		DOVirtual.DelayedCall(glassBreakPanelVisibleTime, () => glassBreakPanel.gameObject.SetActive(false));

		_bulletHole[_health.hitsReceived - 1].enabled = true;
		
		Vibration.Vibrate(20);

		if (!_health.IsDead()) return;
		
		GameEvents.only.InvokeEnemyKillPlayer();
		foreach (var bulletHole in _bulletHole)
			bulletHole.enabled = true;
	}

	private void OnReachNextArea()
	{
		if(!LevelFlowController.only.IsInGiantFight()) return;
		_health.VisibilityToggle(true);
		_health.hitsRequiredToKill = 3;
	}
}