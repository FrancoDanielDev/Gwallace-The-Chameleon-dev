using UnityEngine;

public class SmoothCameraZoom : MonoBehaviour
{
    [SerializeField] private float _zoomAmount = 1f;
    [SerializeField] private float _smoothness = 1f;
    [Space]
    [SerializeField] private Camera _camera;

    private float _initialZ;

    private void Start()
    {
        _initialZ = _camera.transform.position.z;
    }

    private void Update()
    {
        float targetZoom = Mathf.PingPong(Time.time * _smoothness, _zoomAmount) - _zoomAmount / 2f;

        Vector3 newCameraPosition = _camera.transform.position;
        newCameraPosition.z = _initialZ + targetZoom;
        _camera.transform.position = newCameraPosition;
    }
}
