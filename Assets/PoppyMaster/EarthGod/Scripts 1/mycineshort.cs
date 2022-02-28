using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Dreamteck.Splines;

public class mycineshort : MonoBehaviour
{
    public GameObject fcam, mcam,playercam;
    public GameObject gorilla;
    void Start()
    {
        playercam.SetActive(false);
        mcam.SetActive(false);
        fcam.SetActive(true);
        StartCoroutine(delayinshow());
        gorilla.GetComponent<SplineFollower>().follow = false;

    }
    void Update()
    {
        
    }

    public void StartShow()
    {
        fcam.transform.DORotate(mcam.transform.rotation.eulerAngles, 1.5f);
        fcam.transform.DOMove(mcam.transform.position, 1.5f).OnComplete(()=>
        {
            fcam.SetActive(false);
            mcam.SetActive(true);
            gorilla.GetComponent<SplineFollower>().follow = true;
            mcam.transform.DORotate(playercam.transform.rotation.eulerAngles, 1.5f);
            mcam.transform.DOMove(playercam.transform.position, 1.5f).OnComplete(()=>
            {
                mcam.SetActive(false);
                playercam.SetActive(true);
                SendWave.instance.spline.enabled = true;
            }
            );
        }
        );  
    }

   IEnumerator delayinshow()
    {
        yield return new WaitForSeconds(2f);
        StartShow();
    }
}
