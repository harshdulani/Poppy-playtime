using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Carmovement : MonoBehaviour
{
    public GameObject target;
    public Vector3 TargetPostion;
    public float speed;
    public GameObject cartarget;
    public bool walk;
    public void Update()
    {
        if (walk)
        {
            transform.position = Vector3.MoveTowards(transform.position, cartarget.transform.position, 25f * Time.deltaTime);
            if (Vector3.Distance(transform.position, cartarget.transform.position) <= 0.01f)
            {
                StartCoroutine(Attacked());
                walk = false;
            }

        }
    }
    IEnumerator Attacked()
    {
        yield return new WaitForSeconds(1.25f);
        Ui.instance.LossPanel.SetActive(true);
        SendWave.instance.notover = false;
       
    }
}
