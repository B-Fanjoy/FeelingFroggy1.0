using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    public new GameObject camera;
    public CinemachineFreeLook cinemachineFreeLook;

    // Thank you :) https://forum.unity.com/threads/free-look-camera-and-mouse-responsiveness.642886/
    [Tooltip("This depends on your Free Look rigs setup, use to correct Y sensitivity,"
             + " about 1.5 - 2 results in good Y-X square responsiveness")]
    public float yCorrection = 2f;

    public float lookSensitivityX = 20;
    public float lookSensitivityY = 20;

    private Vector2 _lookInput;

    private float _xAxisValue = 0;
    private float _yAxisValue = 0.5f;

    private void Update()
    {
        var mouseX = _lookInput.x * lookSensitivityX * Time.deltaTime;
        var mouseY = _lookInput.y * lookSensitivityY * Time.deltaTime;

        // Correction for Y
        mouseY /= 360f;
        mouseY *= yCorrection;

        _xAxisValue += mouseX;
        _yAxisValue = Mathf.Clamp01(_yAxisValue - mouseY);

        cinemachineFreeLook.m_XAxis.Value = _xAxisValue;
        cinemachineFreeLook.m_YAxis.Value = _yAxisValue;
    }

    private void OnLook(InputValue lookValue)
    {
        _lookInput = lookValue.Get<Vector2>();
    }
}