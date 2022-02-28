using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Shop : MonoBehaviour
{
    public GameObject weaponsPanel;
    public GameObject Dresspanel;
    public GameObject switchpanel;
    public GameObject storeOpenButton;
    public whichweapons[] GetWhichweapons;
    public whichskin Whichskins;
    public List<GameObject> unlockweapons;
    public List<GameObject> unlockskins;
    public Image lockicon;
    public void Start()
    {
        GetWhichweapons = GameObject.FindObjectsOfType<whichweapons>();
        Whichskins = GameObject.FindObjectOfType<whichskin>();
      //  weaponsPanel.SetActive(false);
        Dresspanel.SetActive(false);
        switchpanel.SetActive(false);
        for(int i=1;i< unlockweapons.Count;i++)
        {
            if(PlayerPrefs.GetString(("iteam" + i.ToString()),"lock")=="lock")
            {
                unlockweapons[i].GetComponent<Image>().sprite = lockicon.sprite;
                unlockweapons[i].GetComponent<Button>().enabled = false;
                GameObject lockthis = unlockweapons[i].transform.GetChild(0).gameObject;
                lockthis.GetComponent<Image>().enabled = false;
            }
           
        } 
        for(int i=1;i< unlockskins.Count;i++)
        {
            if(PlayerPrefs.GetString(("iteamskin" + i.ToString()),"lock")=="lock")
            {
                unlockskins[i].GetComponent<Image>().sprite = lockicon.sprite;
                unlockskins[i].GetComponent<Button>().enabled = false;
                GameObject lockthis = unlockskins[i].transform.GetChild(0).gameObject;
                lockthis.GetComponent<Image>().enabled = false;
            }
           
        }
    }
    public void OpenWeaponsPanel()
    {
        weaponsPanel.SetActive(true);
        Dresspanel.SetActive(false);
    }
    public void OpenDressPanel()
    {
        weaponsPanel.SetActive(false);
        Dresspanel.SetActive(true);
    }
    public void thisweapon()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex());
        for (int i = 0; i < GetWhichweapons.Length; i++)
        {
         //   GetWhichweapons[i].setthisweapon(EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex());
        }
    }
    public void thisskin()
    {
        Whichskins.setthisskin(EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex());
    }

    public void openstore()
    {
        Debug.Log("open");
        storeOpenButton.SetActive(false);
        weaponsPanel.SetActive(true);
        switchpanel.SetActive(true);
    }
    public void closestore()
    {
        weaponsPanel.SetActive(false);
        Dresspanel.SetActive(false);
        switchpanel.SetActive(false);
        storeOpenButton.SetActive(true);
    }

}
