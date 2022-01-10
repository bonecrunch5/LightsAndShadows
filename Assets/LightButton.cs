using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightButton : MonoBehaviour
{
    public GameObject spotLight;
    public GameObject aRCamera;

    public void MoveLight()
    {
        spotLight.transform.position = aRCamera.transform.position;
        spotLight.transform.rotation = aRCamera.transform.rotation;
    }
}
