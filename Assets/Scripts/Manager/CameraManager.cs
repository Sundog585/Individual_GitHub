using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public Image aim;

    Vignette vignette;
    Volume postProcessVolume;
    public Volume PostProcessVolume => postProcessVolume;

    public Camera mainCamera;
    public Camera zoomCamera;
    public static bool zoomMode = false;


    public static CameraManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        //aim = transform.Find("Aim").GetComponent<Image>();
        postProcessVolume = FindObjectOfType<Volume>();
        input = new();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    PlayerInputSystem input;

    private void Start()
    {
        PostProcessVolume.profile.TryGet<Vignette>(out vignette);
        aim.enabled = false;
        zoomMode = false;
        mainCamera.enabled = true;
        zoomCamera.enabled = false;
    }

    private void OnEnable()
    {
        input.Camera.Enable();
        input.Camera.ZoomInOut.performed += OnZoomInOut;
    }

    private void OnDisable()
    {
        input.Camera.ZoomInOut.canceled -= OnZoomInOut;
        input.Camera.Disable();
    }

    private void OnZoomInOut(InputAction.CallbackContext obj)
    {
        if (!zoomMode)
        {
            ZoomCameraOn();
            zoomMode = true;
            PlayerTank.siegeTankMode();
        }
        else
        {
            MainCameraOn();
            zoomMode = false;
            PlayerTank.normalTankMode();
        }
    }
    private void OnSceneLoaded(Scene sceneData, LoadSceneMode mode)
    {
        Initialize();
    }

    public void MainCameraOn()
    {
        mainCamera.enabled = true;
        zoomCamera.enabled = false;
        vignette.intensity.value = 0f;
        vignette.smoothness.value = 0f;
        aim.enabled = false;
    }

    public void ZoomCameraOn()
    {
        mainCamera.enabled = false;
        zoomCamera.enabled = true;
        vignette.intensity.value = 1.0f;
        vignette.smoothness.value = 1.0f;
        aim.enabled = true;
    }
    public void Initialize()
    {
    }
}
