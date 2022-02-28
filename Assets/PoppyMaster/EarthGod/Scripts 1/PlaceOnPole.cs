using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOnPole : MonoBehaviour
{

    public List<GameObject> botsforpole;
    public List<GameObject> postionforint;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < botsforpole.Count; i++)
        {
            botsforpole[i].SetActive(false);
        }
        StartCoroutine(sendABotDownThePole());
    }
    IEnumerator sendABotDownThePole()
    {
        yield return new WaitForSeconds(Random.Range(2.5f,4));
        int i = Random.Range(0, botsforpole.Count);
        int j = Random.Range(0, botsforpole.Count);
        GameObject A = Instantiate(botsforpole[j], botsforpole[j].transform.position, botsforpole[j].transform.rotation);
        A.SetActive(true);
        StartCoroutine(sendABotDownThePole());
    }
}
