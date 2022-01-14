using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("LevelSelector");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DownloadMarkers()
    {
        Application.OpenURL("https://drive.google.com/u/0/uc?id=15m1sxEOkyHWmFKft9vi6fAcXZ7Gg7u9E&export=download");
    }
}
