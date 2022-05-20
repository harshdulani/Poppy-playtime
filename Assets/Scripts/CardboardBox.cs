using System.Windows.Forms;
using UnityEngine;

public class CardboardBox : MonoBehaviour
{
	[SerializeField] private float damageThreshold;
	[SerializeField] private float currentDamage;

	private float _crumpledDamage, _destroyedDamage;

	private void Start()
	{
		_crumpledDamage = damageThreshold * 0.33f;
		_destroyedDamage = _crumpledDamage * 2f;
	}
	

	private void OnCollisionEnter(Collision other)
	{
		//if(currentDamage > damageThreshold) return;
		if(transform.parent && other.transform.parent == transform.parent) return;

		transform.parent = null;
		currentDamage += other.relativeVelocity.magnitude;

		if (currentDamage > _destroyedDamage)
			BoxStateDestroyed();
		else if(currentDamage > _crumpledDamage)
			BoxStateCrumpled();
	}

	private void BoxStatePerfect()
	{
		transform.GetChild(0).gameObject.SetActive(true);
		transform.GetChild(1).gameObject.SetActive(false);
		transform.GetChild(2).gameObject.SetActive(false);
	}

	private void BoxStateCrumpled()
	{
		transform.GetChild(0).gameObject.SetActive(false);
		transform.GetChild(1).gameObject.SetActive(true);
		transform.GetChild(2).gameObject.SetActive(false);
	}

	private void BoxStateDestroyed()
	{
		transform.GetChild(0).gameObject.SetActive(false);
		transform.GetChild(1).gameObject.SetActive(false);
		transform.GetChild(2).gameObject.SetActive(true);
	}
}