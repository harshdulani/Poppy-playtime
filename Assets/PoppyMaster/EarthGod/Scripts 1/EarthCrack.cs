using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class EarthCrack : MonoBehaviour
{
    public GameObject BotTransformS;
    public float Speed;
    public bool Move;
    public GameObject BotThisHit;
    public TurnOnTriggerForBroken turnOnTriggerForBroken;
    public GameObject BigHoleEffect;
    public GameObject BlastEffect;
    public bool scale;
    public bool superspeed;
    public GameObject hammer;
    void Start()
    {
        if (SendWave.instance.SendHammer)
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(false);

        }
        turnOnTriggerForBroken = GetComponent<TurnOnTriggerForBroken>();
        scale = true;
        if (superspeed)
        {
            Speed = 70f;
        }
        else
        {
            Speed = 30f;
        }
        if(GameObject.FindObjectOfType<platformdetails>().superspeed)
        {
            Speed = 70f;
        }
    }
    void Update()
    {
        if (Move)
        {
            transform.position = Vector3.MoveTowards(transform.position, BotTransformS.transform.position, Speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, BotTransformS.transform.position) <= 0.125f)
            {
                Move = false;
                transform.GetChild(0).gameObject.SetActive(false);
                StartCoroutine(DestoryAfterSometime());
                CrackSoundMaintain.instance.RemoveBreaker(gameObject);
                if (SendWave.instance.SendHammer)
                {
                    GameObject temp = gameObject.transform.GetChild(1).gameObject;
                    temp.GetComponent<Spiner>().ComeBack = true;
                    temp.gameObject.transform.parent = temp.GetComponent<Spiner>().backuppoint.transform.parent.transform.parent;
                    temp.transform.parent = null;
                }
            }
            if (BotTransformS.GetComponent<Bots>() != null)
            {
                if (BotTransformS.GetComponent<Bots>().Gaint)
                {
                    usesuperspeed();
                }
            }

        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Land"))
        {
            GameObject Collisionwith = collision.gameObject;
            Collisionwith.tag = "Untagged";
            GetComponent<Collider>().isTrigger = false;
            DropIngCracks(Collisionwith);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Land"))
        {
            GameObject Collisionwith = other.gameObject;
            Collisionwith.tag = "Untagged";
            DropIngCracks(Collisionwith);
        }
    }
    public void DropIngCracks(GameObject Crack)
    {
        Crack.AddComponent<TurnOnTriggerForBroken>();
        Crack.AddComponent<Rigidbody>();
    }
    public void GiveCrack(GameObject BotTransform)
    {
        BotTransformS = BotTransform;
        if (BotTransform.GetComponent<Bots>() != null)
        {
            if (BotTransform.GetComponent<Bots>().Gaint)
            {
                usesuperspeed();
            }
        }
        Move = true;
        CrackSoundMaintain.instance.addBreaker(gameObject);
    }
    IEnumerator DestoryAfterSometime()
    {
        yield return new WaitForSeconds(2f);
        CrackSoundMaintain.instance.RemoveBreaker(gameObject);
    }
    public void usesuperspeed()
    {
        Speed = 70f;
    }
}
