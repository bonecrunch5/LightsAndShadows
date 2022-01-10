using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void Pause()
    {

    }

    public void Back()
    {
        SceneManager.LoadScene(0);
    }
}
