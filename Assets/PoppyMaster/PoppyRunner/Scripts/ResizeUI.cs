using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CGTespy.UI;
using System;

public class ResizeUI : MonoBehaviour
{

    public List<Image> swappableImgs;
    public Canvas myTestCanvas;
    Vector2 screenResolution;
    Resolution screenRes;

    private void Awake()
    {
        Screen.SetResolution(1080, 2400, true);
        SetCameraAspectRatio();

        screenRes = Screen.currentResolution;

        if (Application.isEditor)
            screenResolution = GetMainGameViewSize();
        else
            screenResolution = new Vector2(screenRes.width, screenRes.height);

        myTestCanvas.GetComponent<CanvasScaler>().referenceResolution = screenResolution;
        
    }
    void Start()
    {
        print(screenResolution + "aspect: " + AspectRatio((int)screenResolution.x, (int)screenResolution.y));

        ResizeTheUI();
      //  DOTween.To(() => angle, x => angle = x, 360, 1).SetLoops(2,LoopType.Yoyo);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            ResizeTheUI();
    }

    void ResizeTheUI()
    {
        
        for (int i = 0; i < swappableImgs.Count; i++)
        {
            swappableImgs[i].rectTransform.sizeDelta = new Vector2((screenResolution.x / 2), (screenResolution.y / 2));

            switch (i)
            {
                case 0:
                    swappableImgs[i].rectTransform.ApplyAnchorPreset(TextAnchor.UpperLeft, true, true);
                    break;
                case 1:
                    swappableImgs[i].rectTransform.ApplyAnchorPreset(TextAnchor.UpperRight, true, true);
                    break;
                case 2:
                    swappableImgs[i].rectTransform.ApplyAnchorPreset(TextAnchor.LowerLeft, true, true);
                    break;
                case 3:
                    swappableImgs[i].rectTransform.ApplyAnchorPreset(TextAnchor.LowerRight, true, true);
                    break;
            }

        }
        Canvas.ForceUpdateCanvases();
    }

    public static Vector2 GetMainGameViewSize()
    {
        Type T = Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
        return (Vector2)Res;
    }

    private string AspectRatio(int a, int b)
    {
        int r;
        int oa = a;
        int ob = b;
        while (b != 0)
        {
            r = a % b;
            a = b;
            b = r;
        }
        return (oa / a).ToString() + ":" + (ob / a).ToString();
    }

    void SetCameraAspectRatio()
    {
        // set the desired aspect ratio (the values in this example are
        // hard-coded for 16:9, but you could make them into public
        // variables instead so you can set them at design time)
        float targetaspect = 10.0f / 20.0f;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = Camera.main;

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
}
