using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class whichskin : MonoBehaviour
{
    public List<Material> skins;
    public SkinnedMeshRenderer skinnedMesh;

    void Start()
    {
        skinnedMesh = GetComponent<SkinnedMeshRenderer>();
        usethisskin();
    }
    public void usethisskin()
    {
        int temp = PlayerPrefs.GetInt("whichskin", 0);
        skinnedMesh.material = skins[temp];
    }
    public void setthisskin(int thisskin)
    {
        PlayerPrefs.SetInt("whichskin", thisskin);
        usethisskin();
    }
}
