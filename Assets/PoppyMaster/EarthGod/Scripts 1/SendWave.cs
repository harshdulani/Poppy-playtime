using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using DG.Tweening;
using UnityEngine.EventSystems;
public class SendWave : MonoBehaviour
{
    public static SendWave instance;
    RaycastHit hit;
    public SplineFollower spline;
    public GameObject Cracker;
    public GameObject seletedeffect;
    public List<GameObject> SelectedBotsList;
    public  GameObject BotTo;
    [Space(30)]
    public List<int> NumberOfPlatfromAndBotOnThem;
    public  int OnPlatfrom, BotsKillOnThisPlatform;
    [Space(30)]
    public GameObject LeftArm;
    public GameObject RightArm;
    public GameObject LeftArmPoint, RightArmPoint;
    public Animator LeftArmAnimator, RightArmAnimator;
    public Arm LeftArmScript,RightArmScript;
    [Space(30)]
    public GameObject effect;
    private GameObject f;
    private GameObject A, b;
    [Space(30)]
    public bool notover;
    public bool switchCam;
    public bool SendHammer;

    private void Awake()
    {
        GetComponentInParent<SplineFollower>().spline = GameObject.FindGameObjectWithTag("mainspline").GetComponent<SplineComputer>();
        if (!instance)
        {
            instance = this;
        }
    }
    void Start()
    {
        OnPlatfrom = 0;
        BotsKillOnThisPlatform = 0;
        //effect.SetActive(false);
        notover = true;
        spline.enabled = false;
        switchCam = false;
        spline.followSpeed = 50;
        StartCoroutine(splineon());
        NumberOfPlatfromAndBotOnThem.Clear();
        NumberOfPlatfromAndBotOnThem = GameObject.FindObjectOfType<platformdetails>().NumberOfPlatfromAndBotOnThem;
        chech();
    }
    public void Update()
    {
        //if (Input.GetMouseButtonDown(0) && notover&&!EventSystem.current.currentSelectedGameObject)
        //{
        //    Debug.Log("lololo");
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        if (hit.collider != null)
        //        {
        //            Debug.Log(hit.collider.gameObject.name);
        //            if (hit.collider.tag == "Bots")
        //            {
        //                dropthis(hit.collider.gameObject);
        //                BotTo.GetComponent<Bots>().pop();
        //            }
        //            string tags = hit.collider.tag;
        //            if (tags == "Drums" || tags == "cars" || tags == "Building" || tags == "Dino"||tags=="linehole")
        //            {
        //                dropthis(hit.collider.gameObject);
        //            }
        //        }
        //    }
        //}
        //if (SelectedBotsList.Count > 0)
        //{
        //    if (Vector3.Distance(LeftArmPoint.transform.position, SelectedBotsList[0].transform.position) <=
        //        Vector3.Distance(RightArmPoint.transform.position, SelectedBotsList[0].transform.position))
        //    {
        //        if (LeftArm.GetComponent<Arm>().Free)
        //        {
        //            LeftHandHit();
        //        }
        //        else if (RightArm.GetComponent<Arm>().Free)
        //        {
        //            RightHandHit();
        //        }
        //    }
        //    else
        //    {
        //        if (RightArm.GetComponent<Arm>().Free)
        //        {
        //            RightHandHit();
        //        }
        //        else if (LeftArm.GetComponent<Arm>().Free)
        //        {

        //            LeftHandHit();
        //        }
        //    }
        //}
    }

    public void chech()
    {
        // Debug.Log((((8) * 0.04) * 31536000));
    }
    public void dropthis(GameObject drop)
    {
        BotTo = drop;
        SelectedBotsList.Add(drop);
        seletedeffect.transform.position = hit.point;
        seletedeffect.GetComponent<ParticleSystem>().Play();
        TapSound();
    }
    public void LeftHandHit()
    {
        LeftArmAnimator.SetTrigger("HammerHit");
        armreset(LeftArmScript);
    }
    public void RightHandHit()
    {
        RightArmAnimator.SetTrigger("hit");
        armreset(RightArmScript);
    }
    public void armreset(Arm arm)
    {
        arm.Free = false;
        arm.ThisBot = SelectedBotsList[0];
        SelectedBotsList.Remove(SelectedBotsList[0]);
    }
    public void SendWaveInBotDirection(GameObject point, GameObject ThisBot, GameObject poo)
    {
        GameObject CrackMaker = Instantiate(Cracker, point.transform.position, Quaternion.identity);
        CrackMaker.SetActive(true);
        CrackMaker.GetComponent<EarthCrack>().GiveCrack(ThisBot);
        CrackMaker.transform.GetChild(1).GetComponent<Spiner>().backuppoint = poo;
        if (switchCam)
        {
            f = CrackMaker.transform.GetChild(1).gameObject;
            f.SetActive(true);
            GetComponent<Camera>().enabled = false;
            gameObject.transform.parent.GetChild(3).GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = false;
            A.SetActive(false);
            b.SetActive(false);
            StartCoroutine(backagain());
        }
    }
    public void StopSplineFollow()
    {
        spline.follow = false;
        BotsKillOnThisPlatform = 0;
    }
    public void StartSplineFollow()
    {
        spline.enabled = true;
        spline.follow = true;
    }
    public void KillCounter()
    {
        BotsKillOnThisPlatform++;
        if (NumberOfPlatfromAndBotOnThem[OnPlatfrom] == BotsKillOnThisPlatform)
        {
           
            OnPlatfrom++;
            if (OnPlatfrom != NumberOfPlatfromAndBotOnThem.Count)
            {
                StartCoroutine(StartSplineFollowWithDelay());
                Debug.Log("count taken");
            }
            else
            {
                print("Won");
               // StartCoroutine(WonCondition());
            }
        }
    }
    public void TapSound()
    {
        //if (AudioManager.instance)
        //{
        //    AudioManager.instance.Play("Tap");
        //}
    }
    public void switchCamera()
    {
        if (switchCam)
        {
            GetComponent<Camera>().enabled = false;
        }
    }
    public void camback()
    {
        StartCoroutine(backagain());
    }
    IEnumerator StartSplineFollowWithDelay()
    {
        //  StartCoroutine(backagain());
        yield return new WaitForSeconds(1.5f);
        StartSplineFollow();
    }
    IEnumerator splineon()
    {
        yield return new WaitForSeconds(0.1f);
        spline.enabled = true;
    }
    IEnumerator WonCondition()
    {
        yield return new WaitForSeconds(2f);
      //  effect.SetActive(true);
        yield return new WaitForSeconds(2.2f);
        print("Won");
       // Ui.instance.winPanel.SetActive(true);
       
    }
    IEnumerator backagain()
    {
        yield return new WaitForSeconds(2.25f);
        Destroy(f);
        GetComponent<Camera>().enabled = true;
        switchCam = false;
        Cracker.GetComponent<EarthCrack>().Speed = 10;
        GameObject Temp = gameObject.transform.parent.GetChild(3).gameObject;
        Temp.transform.GetChild(0).transform.gameObject.SetActive(true);
        Temp.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = true;
        Temp.transform.gameObject.SetActive(true);
        A.SetActive(true);
        b.SetActive(true);
    }
}

