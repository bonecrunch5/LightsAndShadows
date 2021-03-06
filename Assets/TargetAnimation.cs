using System.Collections.Generic;
using UnityEngine;

public class TargetAnimation : MonoBehaviour
{
    private List<Vector3> finalPosition = new List<Vector3>();
    private List<Vector3> finalScale = new List<Vector3>();
    private List<bool> animationOver = new List<bool>();
    private float spottedTime;
    private bool spotted = false;

    void Start()
    {
        // Set final local positions and scales for children objects of target
        foreach (Transform t in transform)
        {
            finalPosition.Add(t.localPosition);
            finalScale.Add(t.localScale);
            animationOver.Add(t.gameObject.name == "Flashlight");
        }
    }

    void Update()
    {
        if (spotted)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (animationOver[i]) continue;
                if ((spottedTime + i * 0.5) > Time.time) return;

                Transform childTransform = transform.GetChild(i);
                // Snaps position and scale of objects to final values if the difference is too low, to avoid unnecessary processing
                if ((childTransform.localPosition - finalPosition[i]).magnitude < 0.01)
                {
                    childTransform.localPosition = finalPosition[i];
                    childTransform.localScale = finalScale[i];
                    animationOver[i] = true;
                    continue;
                }
                // Smooth translation and scaling of objects
                childTransform.localPosition = Vector3.Lerp(childTransform.localPosition, finalPosition[i], 0.07f);
                childTransform.localScale = Vector3.Lerp(childTransform.localScale, finalScale[i], 0.07f);
            }
        }
    }

    public void StartAnimation()
    {
        if (!spotted)
        {
            spotted = true;
            spottedTime = Time.time;
            // Set initial positions and scales of objects
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform childTransform = transform.GetChild(i);

                if (childTransform.gameObject.name == "Flashlight")
                    continue;

                childTransform.localPosition = Vector3.zero;
                childTransform.localScale = Vector3.zero;
            }
        }
    }
}
