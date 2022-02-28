using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperCracker : MonoBehaviour
{
    public List<GameObject> botslist;
    public bool killbots;
    public GameObject presentTarget;

    void Start()
    {
        neartome();
    }
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            SendWave.instance.SendHammer = false;
            StartKilling();
        }
        if (killbots)
        {
            transform.position = Vector3.MoveTowards(transform.position, presentTarget.transform.position, 25 * Time.smoothDeltaTime);
            if(Vector3.Distance(transform.position, presentTarget.transform.position)<=0.01f)
            {
                botslist.Remove(presentTarget);
                if(botslist.Count==0)
                {
                    killbots = false;
                }
                else
                {
                neartome();
                }
            }

        }
    }
    public void StartKilling()
    {
        killbots = true;
    }
    public void neartome()
    {
        float low = 100000;
        float dis = 0;
        presentTarget = null;
        for (int i = 0; i < botslist.Count; i++)
        {
            dis = Vector3.Distance(transform.position, botslist[i].transform.position);
            if (low > dis)
            {
                low = dis;
                presentTarget = botslist[i];
            }
        }
    }
}
