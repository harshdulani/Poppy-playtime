using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
	private void OnCollisionEnter(Collision other)
	{
		if(!other.collider.CompareTag("Hostage")) return;
		
		if(!other.collider.TryGetComponent(out RagdollLimbController raghu)) return;

		raghu.GetPunched((other.transform.root.position - transform.position).normalized, 10f);
		AudioManager.instance.Play("Bonk");
	}
}
