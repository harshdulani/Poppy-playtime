using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BuildingDroper : MonoBehaviour
{
    public Vector3 SizeToDropThisBuilding;
    public float SpeedOfDrop;
    public bool notfall;
    public GameObject myDropper;
    public float firstx;
    public void Start()
    {
        firstx = transform.localScale.x;
        notfall = true;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Breaker"))
        {
          //  other.gameObject.tag = "Untagged";
            if (notfall)
            {
                notfall = false;
                SendWave.instance.KillCounter();
            }
            gameObject.AddComponent<Rigidbody>();
            if (other.gameObject.GetComponent<EarthCrack>().scale)
            {
                myDropper = other.gameObject;
                other.gameObject.GetComponent<EarthCrack>().scale = false;
                other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                other.gameObject.transform.DOScale(SizeToDropThisBuilding, SpeedOfDrop).OnComplete(()=> {
                    Destroy(myDropper);
                });
                gameObject.transform.DOShakePosition(1f);
               
            }
        }
    }
    public void pop()
    {
        float GrowScale = firstx + 0.15f;
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
