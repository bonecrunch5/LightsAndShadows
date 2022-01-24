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
        Application.OpenURL("https://github.com/bonecrunch5/LightsAndShadows/raw/main/markersPrint.pdf");
    }
}
