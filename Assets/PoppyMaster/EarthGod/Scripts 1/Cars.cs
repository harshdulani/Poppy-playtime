using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Cars : MonoBehaviour
{
    public bool notfall;
    public GameObject cartarget;
    public bool walk;
    public float firstx;
    public void Start()
    {
        notfall = true;
        walk = true;
        firstx = transform.localScale.x;
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Breaker"))
        {
            gameObject.AddComponent<Rigidbody>();
            if (notfall)
            {
                notfall = false;
                SendWave.instance.KillCounter();
            }
            if (other.gameObject.GetComponent<EarthCrack>().scale)
            {
                other.gameObject.GetComponent<EarthCrack>().scale = false;
                other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                other.gameObject.transform.DOScale(new Vector3(15, 15, 15), 1).OnComplete(() =>
                {
                    Destorybreaker(other.gameObject);
                });

                StartCoroutine(drop());
                if (GetComponent<Carmovement>() != null)
                {
                    transform.DOPause();
                    GetComponent<Carmovement>().enabled = false;
                }

            }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Breaker"))
        {
            gameObject.AddComponent<Rigidbody>();
            gameObject.tag = "Untagged";
            if (notfall)
            {
                notfall = false;
                SendWave.instance.KillCounter();
            }
            if (collision.gameObject.GetComponent<EarthCrack>().scale)
            {
                gameObject.AddComponent<Rigidbody>();
                collision.gameObject.GetComponent<EarthCrack>().scale = false;
                collision.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                collision.gameObject.transform.DOScale(new Vector3(15, 15, 15), 1);

                StartCoroutine(drop());
                if (GetComponent<Carmovement>() != null)
                {
                    transform.DOPause();
                    GetComponent<Carmovement>().enabled = false;
                }

            }
        }
    }
    IEnumerator drop()
    {
        yield return new WaitForSeconds(0.08f);
        // gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }
    public void pop()
    {
        float GrowScale = firstx + 0.5f;
        float BackScale = firstx;
        float time = 0.1f;
        Vector3 GrowScaleVec = new Vector3(GrowScale, GrowScale, GrowScale);
        Vector3 BackScaleVec = new Vector3(BackScale, BackScale, BackScale);
        gameObject.transform.DOScale(GrowScaleVec, time).OnComplete(() =>
        {
            gameObject.transform.DOScale(BackScaleVec, time);
        });
    }
    public void Destorybreaker(GameObject A)
    {
        CrackSoundMaintain.instance.RemoveBreaker(A);
    }

}
