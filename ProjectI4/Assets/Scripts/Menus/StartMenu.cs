using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject settings;

    public void NewGame()
    {
        Debug.Log("Loading new game...");
        SceneManager.LoadScene(1);
        Debug.Log("Loading level select...");
    }

    public void LoadGame()
    {
        Debug.Log("Loading saved game...");
    }

    public void Settings()
    {
        Debug.Log("Loading settings...");
        mainMenu.SetActive(false);
        settings.SetActive(true);
    }

    public void MainMenu()
    {
        Debug.Log("Loading Main Menu...");
        mainMenu.SetActive(true);
        settings.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
