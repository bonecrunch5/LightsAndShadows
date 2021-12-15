using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowDetection : MonoBehaviour
{
    public Transform wallObj;

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, Vector3.forward * 10);

        Debug.DrawRay(transform.position, Vector3.forward * 10, Color.red, 10.0f);

        float enter = 0.0f;

        Plane wall = new Plane(new Vector3(-1, 0, 2), new Vector3(-1, 1, 2), new Vector3(1, 1, 2));

        if (wall.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Debug.Log("hit");
            Debug.Log(hitPoint.ToString());

            Vector3 hitPointRel = wallObj.InverseTransformPoint(hitPoint);
            Debug.Log(hitPointRel);
        }

    }
}
