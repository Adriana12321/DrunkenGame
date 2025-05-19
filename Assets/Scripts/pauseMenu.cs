using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menuBtn : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool isPaused;

    public TMP_Dropdown micDropdown;
    public loudnessDetection micManager;

    
    void Start()
    {
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        var devices = micManager.GetAvailableMicDevices();
        micDropdown.ClearOptions();
        micDropdown.AddOptions(devices);
        micDropdown.onValueChanged.AddListener(index =>
        {
            micManager.SetMicDevice(devices[index]);
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (PlayerController.Instance != null)
            PlayerController.Instance.enabled = false;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (PlayerController.Instance != null)
            PlayerController.Instance.enabled = true;
    }
}