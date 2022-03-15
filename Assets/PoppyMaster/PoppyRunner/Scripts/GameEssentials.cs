using UnityEngine;

public class GameEssentials : MonoBehaviour
{
    public static GameEssentials instance;

    public static string remoteConfigVal = "0";
    public SavedData sd { get { return GetComponentInChildren<SavedData>(); } }
    public SceneLoader sl { get { return GetComponentInChildren<SceneLoader>(); } }
    public SoundHapticManager shm { get { return GetComponentInChildren<SoundHapticManager>(); } }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }
        
    }

    void Start()
    {
        Vibration.Init();

       
        //GameObject.Find("EventSystem").SetActive(false);
    }

    public void PrintOut(string val)
    {
        print(val);
    }
}
