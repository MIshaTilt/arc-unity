using UnityEngine;

public class SliderLookAtCam : MonoBehaviour
{
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_mainCamera != null)
        {
            transform.LookAt(_mainCamera.transform);
        }
    }
}
