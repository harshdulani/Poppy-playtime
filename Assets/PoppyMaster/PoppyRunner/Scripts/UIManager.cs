using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

	public LevelFail lf;
	public InGamePanel ig;
	public LevelComplete lc;

	public SafeAreaController safeArea;

	public GameObject aimHelperText;
	private bool _hasTappedToPlay;

    private void Awake()
    {
        instance = this;
        GetComponent<CanvasScaler>().referenceResolution = new Vector2(safeArea.GetSafeArea().width,safeArea.GetSafeArea().height);
        // safeArea = GetComponentInChildren<SafeAreaController>();
    }

	private void Update()
	{
		if(_hasTappedToPlay) return;
		if (!InputExtensions.GetFingerDown()) return;
		if(!EventSystem.current.IsPointerOverGameObject(InputExtensions.IsUsingTouch ? Input.GetTouch(0).fingerId : -1))
			TapToPlay();
	}

	private void TapToPlay()
	{
		aimHelperText.SetActive(false);
		_hasTappedToPlay = true;
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
    
    public void PressGDPRButton()
    {
		if(YcHelper.InstanceExists)
    		YcHelper.ShowGDPR();
    }
}
