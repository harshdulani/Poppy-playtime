using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public LevelFail lf;
    public InGamePanel ig;
    public LevelComplete lc;

    public SafeAreaController safeArea;

    private void Awake()
    {
        instance = this;
        GetComponent<CanvasScaler>().referenceResolution = new Vector2(safeArea.GetSafeArea().width,safeArea.GetSafeArea().height);
        // safeArea = GetComponentInChildren<SafeAreaController>();
    }

    public void LevelCompleted()
    {
        ig.DeactiveGO(false);
        lc.DeactiveGO(true);
        lf.DeactiveGO(false);
        if (GameEssentials.instance)
            GameEssentials.instance.shm.PlayWinSound();
	}

    public void LevelFailed()
    {
        ig.DeactiveGO(false);
        lc.DeactiveGO(false);
        lf.DeactiveGO(true);
    }
}
