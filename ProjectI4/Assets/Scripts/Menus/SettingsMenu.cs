using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public GameObject thisMenu;
    public GameObject settings;

    public Dropdown resDropdown;
    Resolution[] resolutions;

    public AudioMixer audioMixer;
    public Slider volSlider;

    int currScreenResIndex;
    bool isFullscreen;

    public Toggle fSToggle;

    void Start()
    {
        currScreenResIndex = PlayerPrefs.GetInt("screen res index");
        isFullscreen = (PlayerPrefs.GetInt("fullscreen") == 1) ? true : false;

        resolutions = Screen.resolutions;

        resDropdown.ClearOptions();


        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; ++i)
        {
            string option = resolutions[i].width + " * " + resolutions[i].height;
            options.Add(option);
        }

        fSToggle.isOn = isFullscreen;

        resDropdown.AddOptions(options);
        resDropdown.value = currScreenResIndex;
        resDropdown.RefreshShownValue();
    }

    public void SetResolution(int val)
    {
        currScreenResIndex = val;

        Resolution resolution = resolutions[currScreenResIndex];
        Screen.SetResolution(resolution.width, resolution.height, isFullscreen);

        PlayerPrefs.SetInt("screen res index", currScreenResIndex);
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float val)
    {
        audioMixer.SetFloat("MasterVolume", val);
    }

    public void SetFullscreen(bool val)
    {
        Screen.fullScreen = val;

        isFullscreen = val;

        PlayerPrefs.SetInt("fullscreen", ((isFullscreen) ? 1 : 0));
        PlayerPrefs.Save();
    }

    public void Back()
    {
        thisMenu.SetActive(true);
        settings.SetActive(false);
    }
}
