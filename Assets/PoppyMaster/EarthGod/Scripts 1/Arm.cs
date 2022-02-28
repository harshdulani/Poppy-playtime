using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm : MonoBehaviour
{
    public GameObject point;
    public GameObject ThisBot;
    public bool Free;
    public GameObject GroundSlamEffect;
    public GameObject Hammer;
    public void Start()
    {
        Free = true;
    }
    public void sendCrackWave()
    {
        SendWave.instance.SendWaveInBotDirection(point, ThisBot, Hammer);
        if (SendWave.instance.SendHammer)
        {
            Hammer.SetActive(false);
            //if (AudioManager.instance)
            //{
            //    AudioManager.instance.Play("Hammermove");
            //}
            Free = false;
        }
        GameObject A = Instantiate(GroundSlamEffect, point.transform.position, Quaternion.Euler(-90, 0, 0));
        A.SetActive(true);

    }
    public void FreeMe()
    {
        if (!SendWave.instance.SendHammer)
        {
            Free = true;
        }

    }
    public void Hammerisback()
    {
        Free = true;
    }
}
