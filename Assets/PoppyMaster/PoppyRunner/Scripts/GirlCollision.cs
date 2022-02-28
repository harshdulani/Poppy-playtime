using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GirlCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            other.enabled = false;
            GetComponent<Animator>().SetTrigger("Hang");
            
            transform.parent.DOMoveY (transform.parent.position.y - 12f, 0.1f).SetEase(Ease.Flash).OnComplete(()=> 
            {
                transform.GetChild(3).gameObject.SetActive(true);
                other.transform.parent.gameObject.SetActive(false);
             //   other.transform.parent.SetParent(transform.parent); 
               // transform.parent.GetChild(3).position = new Vector3(0, 1f, transform.position.z); 
            });
            
        }

        if(other.CompareTag("EditorOnly"))
        {
            other.enabled = false;
            GetComponent<Animator>().SetTrigger("Walk");
            transform.GetChild(3).gameObject.SetActive(false);
            other.transform.parent.GetComponent<MeshRenderer>().enabled = true;
            // transform.parent.GetChild(3).SetParent(null);
        }
    }
}
