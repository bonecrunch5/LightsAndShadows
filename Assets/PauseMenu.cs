using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void Back()
    {
        SceneManager.LoadScene("LevelSelector");
    }
}
