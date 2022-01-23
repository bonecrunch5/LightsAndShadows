using UnityEngine;

public class URLLink : MonoBehaviour
{
    public string url = "";

    public void EnterURL()
    {
        Application.OpenURL(url);
    }
}
