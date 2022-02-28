using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using DG.Tweening;
public class Racecar : MonoBehaviour
{
    public bool notfall;
    public GameObject mybreaker;
    void Start()
    {
        notfall = true;
    }

    public void Update()
    {
        if (GetComponent<SplineFollower>().result.percent == 1)
        {
            StartCoroutine(failCondition());
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Breaker"))
        {
            mybreaker = other.gameObject;
            GetComponent<SplineFollower>().enabled = false;
            GetComponent<Rigidbody>().useGravity = true;
            if (notfall)
            {
                notfall = false;
                SendWave.instance.KillCounter();
                if (SendWave.instance.switchCam)
                {
                    other.gameObject.transform.GetChild(1).transform.parent = null;

                }
            }
            if (other.gameObject.GetComponent<EarthCrack>().scale)
            {
                other.gameObject.GetComponent<EarthCrack>().scale = false;
                other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                other.gameObject.transform.DOScale(new Vector3(6, 6, 6), 1).OnComplete(() =>
                {
                    if (SendWave.instance.switchCam)
                    {
                        SendWave.instance.camback();
                    }
                    Destorybreaker(mybreaker);
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
