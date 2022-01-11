using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public void PlayLevel(int level)
    {
        SceneManager.LoadScene("Level" + level);
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
