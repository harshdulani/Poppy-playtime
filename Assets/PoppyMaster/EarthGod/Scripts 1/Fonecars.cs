using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using DG.Tweening;

public class Fonecars : MonoBehaviour
{
    RaycastHit hit;
    public bool notfall;
    public GameObject mybreaker;
    public bool notover;
    public bool close;
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.forward);
        notfall = true;
        notover = true;
        close = true;
    }
    void Update()
    {
        if (close)
        {
            if (GetComponent<SplineFollower>().result.percent == 1)
            {
                StartCoroutine(failCondition());
                close = false;
            }
        }
        if (notover)
        {
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 3.5f);
            if (hit.collider == null)
            {
                GetComponent<Rigidbody>().useGravity = true;
                Destroy(GetComponent<SplineFollower>());
                GetComponent<Rigidbody>().AddForce(Vector3.forward);
                GetComponent<Fonecars>().enabled = false;
                notfall = false;
                notover = false;
            }
            //if (hit.collider != null)
            //{
            //    Debug.Log(hit.collider.name);
            //}
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Breaker") && notfall)
        {
            mybreaker = other.gameObject;
            other.gameObject.tag = "Untagged";
            other.gameObject.GetComponent<EarthCrack>().Move = false;
            if (notfall)
            {
                notfall = false;
                SendWave.instance.KillCounter();
                if (SendWave.instance.switchCam)
                {
                    if (other.gameObject.transform.GetChild(1).transform.gameObject != null)
                    {
                        other.gameObject.transform.GetChild(1).transform.parent = null;
                    }
                }
            }
            if (other.gameObject.GetComponent<EarthCrack>().scale && notover)
            {
                other.gameObject.GetComponent<EarthCrack>().scale = false;
                other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                other.gameObject.transform.DOScale(new Vector3(4.5f, 4.5f, 4.5f), 1).OnComplete(() =>
                {

                    Destorybreaker(mybreaker);
                    if (SendWave.instance.switchCam)
                    {
                        SendWave.instance.camback();
                    }
                });
                if (GetComponent<Carmovement>() != null)
                {
                    transform.DOPause();
                    GetComponent<Carmovement>().enabled = false;
                }

            }
        }
    }
    public void Destorybreaker(GameObject A)
    {
        CrackSoundMaintain.instance.RemoveBreaker(A);
        Destroy(A);
    }
    IEnumerator failCondition()
    {
        yield return new WaitForSeconds(0.5f);
        Attacked();
        if (SendWave.instance.notover)
        {
           
        }
    }

    public void Attacked()
    {
        Ui.instance.LossPanel.SetActive(true);
        SendWave.instance.notover = false;
    }
}
