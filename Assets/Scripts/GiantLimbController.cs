using DG.Tweening;
using UnityEngine;

public class GiantLimbController : MonoBehaviour
{
	[SerializeField] private GameObject explosion;
	private GiantController _parent;
	
	private void Start()
	{
		_parent = transform.root.GetComponent<GiantController>();
	}
	
	private void OnCollisionEnter(Collision other)
	{
		if(!_parent) return;
	
		if(_parent.isDead) return;
		
		if(other.transform.root == transform.root) return;
		
		if (!other.collider.CompareTag("Target")) return;

		var exploder = Instantiate(explosion, other.contacts[0].point,
			Quaternion.LookRotation(other.contacts[0].normal));

		exploder.transform.localScale *= _parent.explosionScale;
		Destroy(exploder, 3f);
		TakeCareOfThis(other.transform);
	}

	private void TakeCareOfThis(Transform victim)
	{
		//I AM THE VICTIM FFS
		//explanation why this code was put in this method:
		//i required this method to fire off independently and have a COPY of the victim to be disabled because
		//a dotween .oncomplete was checking the "other" variable at that instant
		//this method is called continuously
		//i just turn put all these flags on this so it doesn't get to DO anything
		//but every frame/physics step, Collision other ka value change hota hai
		
		victim.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
		{
			victim.gameObject.SetActive(false);
		});
		_parent.GetHit(victim);
	}
}