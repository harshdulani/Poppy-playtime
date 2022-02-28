using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using DG.Tweening;
public class DinoDroper : MonoBehaviour
{
    public GameObject MyBreaker;
    public float FirstSize;
    public GameObject BotTarget;
    public bool notfall, Walk;
    // Start is called before the first frame update
    void Start()
    {
        FirstSize = transform.localScale.x;
        notfall = Walk = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.localPosition, BotTarget.transform.localPosition) < 0.1f&&Walk)
        {
            StartCoroutine(failCondition());
        }
    }
    IEnumerator failCondition()
    {
        yield return new WaitForSeconds(0.5f);
        Attacked();
      
    }
    public void Attacked()
    {
        Ui.instance.LossPanel.SetActive(true);
        SendWave.instance.notover = false;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Breaker"))
        {
            fall();
            if (other.gameObject.GetComponent<EarthCrack>().scale)
            {
                MyBreaker = other.gameObject;
                GetComponent<Collider>().enabled = false;
                other.gameObject.GetComponent<EarthCrack>().scale = false;
                if (GetComponent<ReleaseHelicopter>() != null)
                {
                    GetComponent<ReleaseHelicopter>().relese();
                }
                gameObject.AddComponent<Rigidbody>();
                //other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                //other.gameObject.transform.GetChild(1).GetComponent<Spiner>().ComeBack = true;
                //other.gameObject.transform.GetChild(1).gameObject.transform.parent =
                //     other.gameObject.transform.GetChild(1).GetComponent<Spiner>().backuppoint.transform.parent.transform.parent;
                //other.gameObject.transform.GetChild(1).transform.parent = null;
                if (GetComponent<SplineFollower>() != null)
                {
                    GetComponent<SplineFollower>().enabled = false;
                }
                other.gameObject.transform.DOScale(new Vector3(20, 20, 20), 1).OnComplete(() =>
                {
                    Destorybreaker(MyBreaker);
                    if (SendWave.instance.switchCam)
                    {
                        SendWave.instance.camback();
                    }
                });
                GetComponent<Animator>().SetBool("isDead",true);
                GetComponent<Animator>().SetBool("isWalking", false);
            }
            else
            {
                other.gameObject.transform.DOScale(new Vector3(5, 5, 5), 1).OnComplete(() =>
                {
                    Destorybreaker(MyBreaker);
                });
            }
            if (SendWave.instance.switchCam)
            {
                other.gameObject.transform.GetChild(1).transform.parent = null;
            }
        }
    }
    public void Destorybreaker(GameObject A)
    {
        CrackSoundMaintain.instance.RemoveBreaker(A);
        if (A.GetComponent<SuperCracker>() != null)
        {
        }
        else
        {
            Destroy(A);
        }
    }
    public void fall()
    {
        if (notfall)
        {
            notfall = false;
            Walk = false;
            SendWave.instance.KillCounter();
            //if (AudioManager.instance)
            //{
            //    AudioManager.instance.Play("scream");
            //}
        }
    }
        public void pop()
    {
        float GrowScale = FirstSize + 0.35f;
        float BackScale = FirstSize;
        float time = 0.1f;
        Vector3 GrowScaleVec = new Vector3(GrowScale, GrowScale, GrowScale);
        Vector3 BackScaleVec = new Vector3(BackScale, BackScale, BackScale);
        gameObject.transform.DOScale(GrowScaleVec, time).OnComplete(() =>
        {
            gameObject.transform.DOScale(BackScaleVec, time);
        });
        Vibration.Vibrate(15);

    }
}

