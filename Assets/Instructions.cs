using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Instructions : MonoBehaviour
{
    public bool marker1Used = false;
    public bool marker2Used = false;
    public bool marker3Used = false;
    public bool marker4Used = false;

    public Image marker1;
    public Image marker2;
    public Image marker3;
    public Image marker4;

    private Color activeColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private Color disabledColor = new Color(0.25f, 0.25f, 0.25f, 1.0f);

    void Start()
    {
        marker1.color = marker1Used ? activeColor : disabledColor;
        marker2.color = marker2Used ? activeColor : disabledColor;
        marker3.color = marker3Used ? activeColor : disabledColor;
        marker4.color = marker4Used ? activeColor : disabledColor;
    }
}
