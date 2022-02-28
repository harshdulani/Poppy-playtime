using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlastEffect : MonoBehaviour
{
    public GameObject Blast;
    public GameObject cans;
    public Vector3 sizeofhole;
    public float speed;
    void Start()
    {
        Blast.SetActive(false);
    }
    public void ShowBlast()
    {
        cans.SetActive(false);
        Blast.SetActive(true);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Breaker"))
        {
            ShowBlast();
            if (other.gameObject.GetComponent<EarthCrack>().scale)
            {
                other.gameObject.GetComponent<EarthCrack>().scale = false;
                other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                other.gameObject.transform.DOScale(sizeofhole, speed);
                other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                other.gameObject.transform.GetChild(1).GetComponent<Spiner>().ComeBack = true;
                other.gameObject.transform.GetChild(1).gameObject.transform.parent =
                     other.gameObject.transform.GetChild(1).GetComponent<Spiner>().backuppoint.transform.parent.transform.parent;
                other.gameObject.transform.GetChild(1).transform.parent = null;
                if (SendWave.instance.switchCam)
                {
                    other.gameObject.transform.GetChild(1).transform.parent = null;
                }
            }
        }
    }
}
