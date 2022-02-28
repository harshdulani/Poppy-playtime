using UnityEngine;
using System.Collections;

public class Earthquake : MonoBehaviour
{

    public GameObject earthquakeFX;
    public GameObject earthquakeAnim;
    private Animation ani;

    void Start()
    {

        earthquakeFX.SetActive(false);
        ani = earthquakeAnim.GetComponent<Animation>();

    }


    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {

            StartCoroutine("startEarthquake");

        }

        // Reset earthquake

        if (Input.GetButtonDown("Fire2"))
        {

            ani["Quake"].time = 0.0f;
            ani["Quake"].speed = 0;
            earthquakeAnim.GetComponent<Animation>().Play();
      
            earthquakeFX.SetActive(false);

        }

    }

    



    IEnumerator startEarthquake()
    {

        ani["Quake"].speed = 1;
        earthquakeFX.SetActive(true);
        earthquakeAnim.GetComponent<Animation>().Play();

        yield return new WaitForSeconds(0.1f);

    }


}
