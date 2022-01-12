using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Toggle leftHandedToggle;

    void Awake()
    {
        int isLeftHanded = PlayerPrefs.GetInt("IsLeftHanded", 0);
        leftHandedToggle.isOn = (isLeftHanded == 1);
    }

    public void IsLeftHanded()
    {
        int isLeftHanded = leftHandedToggle.isOn ? 1 : 0;
        PlayerPrefs.SetInt("IsLeftHanded", isLeftHanded);
        PlayerPrefs.Save();
    }
}
