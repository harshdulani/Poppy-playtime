using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformOner : MonoBehaviour
{
    public GameObject PreviewsPlatform;
    public GameObject NextPlatform;
    public List<GameObject> startbotwave;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PreviewsPlatform.SetActive(false);
            NextPlatform.transform.GetChild(0).gameObject.SetActive(false);
            NextPlatform.transform.GetChild(1).gameObject.SetActive(true);
            for (int i = 0; i < startbotwave.Count; i++)
            {
                startbotwave[i].GetComponent<Bots>().Startwalking();
            }
        }
    }
}
