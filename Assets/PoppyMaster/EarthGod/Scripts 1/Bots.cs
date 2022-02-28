using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using Dreamteck.Splines;
public enum AnimationChoices { Happy, JabCross, RumbaDancing, Taunt, SillyDancing };
public enum DoWhat { Stand, Walk, WalkOnRoad, Zombie, helpless, gaint, gunman, gaintpunch, helo, helpme,climb,poppy };
public class Bots : MonoBehaviour
{
    public AnimationChoices animationChoices;
    public DoWhat DoWhat;
    private Animator animator;
    private Vector3 PointIAmGoing;
    [Space(30)]
    public GameObject BigHoleEffect;
    public GameObject BotTarget;
    private GameObject MyBreaker;
    private GameObject BotThisHit;
    [Space(30)]
    public float Walkspeed;
    public float FirstSize;
    [Space(30)]
    public bool Walk;
    public bool WalkOnRoad;
    public bool Gaint;
    private bool notfall;
    private bool helpme;
    void Start()
    {
        FirstSize = transform.localScale.x;
        animator = GetComponent<Animator>();
        if (BotTarget != null)
        {
            BotTarget.GetComponent<MeshRenderer>().enabled = false;
        }
        Walkspeed = 5;
        notfall = true;
        dowhatnow();
        if (DoWhat == DoWhat.Zombie)
        {
            animator.SetTrigger("Zombie walk");
            Walkspeed = 1.5F;
        }
    }
    public void Update()
    {
        if (WalkOnRoad)
        {
            transform.position = Vector3.MoveTowards(transform.position, PointIAmGoing, 15 * Time.deltaTime);
            transform.LookAt(PointIAmGoing);
            if (Vector3.Distance(transform.position, PointIAmGoing) <= 0.1f)
            {
                PointIAmGoing = RoadLocationPoint.instance.giveapoint();
            }
        }
        if (Walk)
        {
            transform.position = Vector3.MoveTowards(transform.position, BotTarget.transform.position, Walkspeed * Time.deltaTime);
            if (Vector3.Distance(transform.localPosition, BotTarget.transform.localPosition) < 0.1f)
            {
                animator.SetTrigger("Attack");
                Walk = false;
                StartCoroutine(failCondition());
            }
        }
    }

    public void dowhatnow()
    {
        switch (DoWhat)
        {
            case DoWhat.Stand:
                int ran = 0;
                ran = Random.Range(1, 6);
                switch (ran)
                {
                    case 1:
                        animator.SetTrigger("Happy");
                        break;
                    case 2:
                        animator.SetTrigger("Rumba Dancing");

                        break;
                    case 3:
                        animator.SetTrigger("Taunt");

                        break;
                    case 4:
                        animator.SetTrigger("Silly Dancing");

                        break;
                    case 5:
                        animator.SetTrigger("Jab Cross");

                        break;
                    case 6:
                        animator.SetTrigger("Mutant Swiping");

                        break;
                    case 7:
                        animator.SetTrigger("Combo Punch");

                        break;
                    case 8:
                        animator.SetTrigger("Rope Swinging");

                        break;
                    case 9:
                        animator.SetTrigger("Hanging");

                        break;

                }
                break;
            case DoWhat.Walk:
                animator.SetTrigger("walking");
                Startwalking();
                break;
            case DoWhat.WalkOnRoad:
                PointIAmGoing = RoadLocationPoint.instance.giveapoint();
                animator.SetTrigger("walking");
                break;
            case DoWhat.Zombie:
                // Startwalking();
                animator.SetTrigger("Zombie walk");
                break;
            case DoWhat.helpless:
                animator.SetTrigger("Waving");
                break;
            case DoWhat.gaint:
                Gaint = true;
                animator.SetTrigger("Mutant Walking");
                break;
            case DoWhat.gunman:
                // animator.SetTrigger(" ");
                break;
            case DoWhat.gaintpunch:
                animator.SetTrigger("Mutant Swiping");
                break;
            case DoWhat.helo:
                animator.SetTrigger("Rope Swinging");
                break;
            case DoWhat.helpme:
                helpme = true;
                animator.SetTrigger("Waving");
                break;
            case DoWhat.climb:
                animator.SetTrigger("crawl");
                break;
            case DoWhat.poppy:
                animator.SetTrigger("Mutant Walking");
                break;
        }

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
                MyBreaker.GetComponent<EarthCrack>().scale = false;
                MyBreaker.transform.GetChild(0).gameObject.SetActive(false);
                if (GetComponent<ReleaseHelicopter>() != null)
                {
                    GetComponent<ReleaseHelicopter>().relese();
                }
                if (GetComponent<SplineFollower>() != null)
                {
                    GetComponent<SplineFollower>().enabled = false;
                }
                if (Gaint)
                {
                    other.gameObject.transform.DOScale(new Vector3(20, 20, 20), 1).OnComplete(() =>
                    {
                        Destorybreaker(MyBreaker);
                        if (SendWave.instance.switchCam)
                        {
                            SendWave.instance.camback();
                        }
                    });
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
            if (helpme)
            {
                StartCoroutine(failCondition());

            }
            else
            {
            }
        }
    }
    public void fall()
    {

        if (notfall)
        {
            notfall = false;
            Walk = false;
            if (!helpme)
            {
                SendWave.instance.KillCounter();
                Debug.Log("count went");

            }
            //if (AudioManager.instance)
            //{
            //    AudioManager.instance.Play("scream");
            //}
        }
        gameObject.GetComponent<Collider>().enabled = false;
        BotThisHit = gameObject;
        StartCoroutine(OnAnimators());
        Instantiate(BigHoleEffect, transform.position, Quaternion.Euler(-90, 0, 0)).SetActive(true);

    }
    public void Startwalking()
    {
        if (DoWhat == DoWhat.Zombie)
        {
            animator.SetTrigger("Zombie walk");
            Walkspeed = 1.5F;
        }
        else
        {

            animator.SetTrigger("walking");
        }
        Walk = true;
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
    public void Attacked()
    {
        Ui.instance.LossPanel.SetActive(true);
        SendWave.instance.notover = false;
    }
    public void Destorybreaker(GameObject A)
    {
        CrackSoundMaintain.instance.RemoveBreaker(A);
        if (A.GetComponent<SuperCracker>() == null)
        {
            Destroy(A);
        }
    }
    IEnumerator OnAnimators()
    {
        yield return new WaitForSeconds(0.4f);
        BotThisHit.GetComponent<Animator>().enabled = false;
    }
    IEnumerator failCondition()
    {
        yield return new WaitForSeconds(0.5f);
        Attacked();
       
    }
    public void shake()
    {
        CameraShake.instance.shakeDuration = 0.25f;
    }
}
