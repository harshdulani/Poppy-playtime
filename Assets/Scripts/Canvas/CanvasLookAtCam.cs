using UnityEngine;

public class CanvasLookAtCam : MonoBehaviour
{
    private Canvas _canvas;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.worldCamera = Camera.main;
    }

    private void Update()
    {
        var direction = transform.root .position - _canvas.worldCamera.transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
