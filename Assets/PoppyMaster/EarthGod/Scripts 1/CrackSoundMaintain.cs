using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackSoundMaintain : MonoBehaviour
{
    public List<GameObject> Breakers;
    public bool AudioPlaying;
    public static CrackSoundMaintain instance;
   void Awake()
    {
        if(!instance)
        {
            instance = this;
        }
    }

    public void addBreaker(GameObject A)
    {
        Breakers.Add(A);
        if (Breakers.Count != 0 && !AudioPlaying)
        {
            //if (AudioManager.instance)
            //{
            //    AudioPlaying = true;
            //    AudioManager.instance.Play("GroundCrack");
            //    StartCoroutine(virbationForCrack());
            //}
        }
    }
    public void RemoveBreaker(GameObject A)
    {
        Breakers.Remove(A);
        if (Breakers.Count == 0 && AudioPlaying)
        {
            //if (AudioManager.instance)
            //{
            //    AudioPlaying = false;
            //    AudioManager.instance.Pause("GroundCrack");
            //    StopAllCoroutines();
            //}
        }
    }

    IEnumerator virbationForCrack()
    {
        yield return new WaitForSeconds(0.25f);
    //    Debug.Log("tack");
        Vibration.Vibrate(15);
        StartCoroutine(virbationForCrack());
    }
}
