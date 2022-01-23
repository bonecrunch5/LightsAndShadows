using UnityEngine;
using UnityEngine.EventSystems;

public class GameUI : MonoBehaviour
{
    public UIBehaviour lightButton;

    void OnEnable()
    {
        int isLeftHanded = PlayerPrefs.GetInt("IsLeftHanded", 0);
        if (isLeftHanded == 1)
            lightButton.transform.localPosition = new Vector3(-600,-300,0);
        else lightButton.transform.localPosition = new Vector3(600,-300,0);
    }
}
