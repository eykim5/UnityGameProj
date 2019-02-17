using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour {

    public GameObject pauseMenu;
    public GameObject settings;

    public GameObject resumeButton;
    EventSystem eventSystem;
    private bool isPaused = false;

    public bool isMenu;
    private GameObject currentButton;

    void Start()
    {
        eventSystem = EventSystem.current;
        Time.timeScale = 1f;
    }

    void Update() {
        if (Input.GetButtonDown("Select"))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

    }

    void Pause()
    {
        if (isMenu)
        {
            currentButton = eventSystem.currentSelectedGameObject;
        }
        pauseMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(resumeButton);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        if (isMenu)
        {
            eventSystem.SetSelectedGameObject(currentButton);
        }

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Settings()
    {
        pauseMenu.SetActive(false);
        settings.SetActive(true);
    }

    public void MainMenu()
    {
        Debug.Log("Loading main menu... ");
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Debug.Log("Quitting game... ");
        Application.Quit();
    }

}
