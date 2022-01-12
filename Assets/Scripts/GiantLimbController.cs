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

		Destroy(Instantiate(explosion, other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal)), 3f);
		other.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => other.gameObject.SetActive(false));
		_parent.GetHit(other.transform);
	}
}