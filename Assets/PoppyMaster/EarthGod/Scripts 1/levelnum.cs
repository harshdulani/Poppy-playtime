using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class levelnum : MonoBehaviour
{
    public Text levelnum1;
    void Start()
    {
        levelnum1 = GetComponent<Text>();
        levelnum1.text = "Level " + PlayerPrefs.GetInt("level",1).ToString();
       // Debug.Log(PlayerPrefs.GetInt("level"));
    }
}
