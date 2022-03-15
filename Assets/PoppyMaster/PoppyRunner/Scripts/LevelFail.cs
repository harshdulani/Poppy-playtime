using UnityEngine;

public class LevelFail : MonoBehaviour
{
    GameEssentials gameEssentials { get { return GameEssentials.instance; } }
    void Start()
    {
        if(GAScript.Instance)
            GAScript.Instance.LevelFail(gameEssentials.sd.GetLevelNumber().ToString());
    }

   
    public void Retry()
    {
        if (gameEssentials) 
        {
            gameEssentials.shm.StopPlaying();
            gameEssentials.sl.LoadSameScene();
        }
    }

    public void DeactiveGO(bool v)
    {
        gameObject.SetActive(v);
    }
}
