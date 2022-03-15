using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    public bool runOnce;
    public ItemsType currentItem;
    public List<GameObject> allItems;


    Transform handJoint { get { return transform.GetChild(1); } } 

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (runOnce)
            ActivateCurrentItem();

        runOnce = false;
    }


    void ActivateCurrentItem()
    {
        switch (currentItem)
        {
            case ItemsType.None:
                ForNone();
                DeactiveAllItems();
                break;
            case ItemsType.Hammer:
                ForRemaining();
                SetCurrentItemActivate(0);
                break;
            case ItemsType.Sandle:
                ForRemaining();
                SetCurrentItemActivate(1);
                break;
            case ItemsType.Shoes:
                ForRemaining();
                SetCurrentItemActivate(2);
                break;
            case ItemsType.Revolver:
                ForRemaining();
                SetCurrentItemActivate(3);
                break;
            case ItemsType.Cake:
                ForCake();
                SetCurrentItemActivate(4);
                break;
        }
    }

    void SetCurrentItemActivate(int val)
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            if (val == i)
                allItems[i].SetActive(true);
            else
                allItems[i].SetActive(false);
        }
    }

    void DeactiveAllItems()
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            allItems[i].SetActive(false);
        }
    }

    void ForCake()
    {
        GetComponent<Animator>().enabled = false;
        handJoint.localEulerAngles = new Vector3(90, -260f, -330f);
    }

    void ForRemaining()
    {
        GetComponent<Animator>().enabled = true;
        handJoint.localEulerAngles = new Vector3(0,-100f,-158f);
    }

    void ForNone()
    {
        GetComponent<Animator>().enabled = false;
        handJoint.localEulerAngles = new Vector3(-18f, -170f, -72f);
    }

}
public enum ItemsType { None, Hammer,Shoes, Sandle, Revolver, Cake }
