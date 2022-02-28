using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TurnOnTriggerForBroken : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Trigg()); 
    }
    IEnumerator Trigg()
    {
        yield return new WaitForSeconds(0.0f);
        GetComponent<MeshCollider>().isTrigger = true;
        transform.Translate(Vector3.down * 100 * Time.deltaTime);
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}
