using DG.Tweening;
using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
	public bool isDead;

	public void OnDeath()
	{
		GetComponent<Collider>().enabled = false;
		isDead = true;
	}

	private void OnCollisionEnter(Collision other)
	{
		if (isDead) return;
		
		if(CompareTag("EnemyAttack"))
		{
			if (!(other.gameObject.CompareTag("HitBox") || other.gameObject.CompareTag("Arm") ||
				  other.gameObject.CompareTag("Player"))) return;

			GameEvents.only.InvokeEnemyHitPlayer(transform);

			transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => gameObject.SetActive(false));
			return;
		}

		if(!other.collider.CompareTag("Hostage")) return;
		
		if(!other.collider.TryGetComponent(out RagdollLimbController raghu)) return;
		
		raghu.GetPunched((other.transform.root.position - transform.position).normalized, 10f);
		GameEvents.only.InvokeEnemyKillPlayer();
		AudioManager.instance.Play("Bonk");
	}
}
