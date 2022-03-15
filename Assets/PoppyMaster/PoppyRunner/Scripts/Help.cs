using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Help : MonoBehaviour
{
    public static Help instance;

    public Image img;
    public GameObject hand;
    public TextMeshProUGUI helpTxt;

    private void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        Invoke("Reactivate", 0.5f);
    }

    private void Update()
    {
        //Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, objectToFollow.position);
        //healthBar.anchoredPosition = screenPoint - canvasRectT.sizeDelta / 2f;
        //hand.GetComponent<Image>().rectTransform.anchorMin = Camera.main.WorldToViewportPoint(PoppyEnemies.instance.poppyEnemiesGroup[0].transform.position);
        //hand.GetComponent<Image>().rectTransform.anchorMax = Camera.main.WorldToViewportPoint(PoppyEnemies.instance.poppyEnemiesGroup[0].transform.position);
    }

    void Reactivate()
    {
        Time.timeScale = 0;
        img.enabled = true;
        hand.SetActive(true);
        helpTxt.gameObject.SetActive(true);
        hand.transform.DOScale(Vector3.one * 1.2f, 0.5f).SetEase(Ease.Flash).SetLoops(-1, LoopType.Yoyo);
    }

    public void DeactiveHelp()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
