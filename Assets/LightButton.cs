using UnityEngine;

public class LightButton : MonoBehaviour
{
    public GameObject spotLight;
    public GameObject aRCamera;

    public void MoveLight()
    {
        if(!spotLight.activeSelf)
            spotLight.SetActive(true);

        spotLight.transform.position = aRCamera.transform.position;
        spotLight.transform.rotation = aRCamera.transform.rotation;
    }
}
