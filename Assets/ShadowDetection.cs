using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowDetection : MonoBehaviour
{
    public Light spotlight;

    private MeshFilter meshFilter;

    public float numHorizSectors = 10;
    public float numVertSectors = 10;

    public Color debugColor = Color.green;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    void Update()
    {
        Bounds bounds = meshFilter.mesh.bounds;

        Vector3 planeCenter = bounds.center;
        Vector3 planeExtents = bounds.extents;

        Vector3 planeTopRight = new Vector3(planeCenter.x - planeExtents.x, planeCenter.y, planeCenter.z - planeExtents.z);  // Top right corner
        Vector3 planeTopLeft = new Vector3(planeCenter.x + planeExtents.x, planeCenter.y, planeCenter.z - planeExtents.z);  // Top left corner
        Vector3 planeBottomRight = new Vector3(planeCenter.x - planeExtents.x, planeCenter.y, planeCenter.z + planeExtents.z);  // Bottom right corner
        Vector3 planeBottomLeft = new Vector3(planeCenter.x + planeExtents.x, planeCenter.y, planeCenter.z + planeExtents.z);  // Bottom left corner

        planeTopRight = transform.TransformPoint(planeTopRight);
        planeTopLeft = transform.TransformPoint(planeTopLeft);
        planeBottomRight = transform.TransformPoint(planeBottomRight);
        planeBottomLeft = transform.TransformPoint(planeBottomLeft);

        for (int i = 1; i <= numVertSectors; i++)
        {
            Vector3 leftPoint = Vector3.Lerp(planeTopLeft, planeBottomLeft, (i - 0.5f) / numVertSectors);
            Vector3 rightPoint = Vector3.Lerp(planeTopRight, planeBottomRight, (i - 0.5f) / numVertSectors);

            for (int j = 1; j <= numHorizSectors; j++)
            {
                Vector3 sectorCenter = Vector3.Lerp(leftPoint, rightPoint, (j - 0.5f) / numHorizSectors);

                Vector3 direction = spotlight.transform.position - sectorCenter;

                Debug.DrawRay(sectorCenter, direction, Color.red);
            }
        }

        // DEBUG: draw plane bounding box
        for (int i = 0; i <= numVertSectors; i++)
        {
            Vector3 leftPoint = Vector3.Lerp(planeTopLeft, planeBottomLeft, i / numVertSectors);
            Vector3 rightPoint = Vector3.Lerp(planeTopRight, planeBottomRight, i / numVertSectors);

            Debug.DrawLine(leftPoint, rightPoint, debugColor);
        }

        for (int i = 0; i <= numHorizSectors; i++)
        {
            Vector3 topPoint = Vector3.Lerp(planeTopLeft, planeTopRight, i / numHorizSectors);
            Vector3 bottomPoint = Vector3.Lerp(planeBottomLeft, planeBottomRight, i / numHorizSectors);

            Debug.DrawLine(topPoint, bottomPoint, debugColor);
        }
    }
}
