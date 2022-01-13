using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameUI : MonoBehaviour
{
    public UIBehaviour lighButton;

    void OnEnable()
    {
        int isLeftHanded = PlayerPrefs.GetInt("IsLeftHanded", 0);
        if (isLeftHanded == 1)
            lighButton.transform.localPosition = new Vector3(-600,-300,0);
        else lighButton.transform.localPosition = new Vector3(600,-300,0);
    }
}
