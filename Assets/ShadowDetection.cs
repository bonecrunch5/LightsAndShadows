using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowDetection : MonoBehaviour
{
    public Texture2D shadowMap;

    public Light spotlight;

    private MeshFilter meshFilter;

    public int numHorizSectors = 10;
    public int numVertSectors = 10;

    public bool debugSectors = false;
    public bool debugShadows = false;
    public bool debugTextureShadows = false;
    public bool debugTextureWhatever = false;
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

        int widthJump = Mathf.FloorToInt(shadowMap.width / numHorizSectors);
        int heightJump = Mathf.FloorToInt(shadowMap.height / numVertSectors);

        int numCorrectSectors = 0;
        int numWrongSectors = 0;

        for (int i = 1; i <= numVertSectors; i++)
        {
            Vector3 leftPoint = Vector3.Lerp(planeTopLeft, planeBottomLeft, (i - 0.5f) / numVertSectors);
            Vector3 rightPoint = Vector3.Lerp(planeTopRight, planeBottomRight, (i - 0.5f) / numVertSectors);

            for (int j = 1; j <= numHorizSectors; j++)
            {
                Vector3 sectorCenter = Vector3.Lerp(leftPoint, rightPoint, (j - 0.5f) / numHorizSectors);

                Vector3 direction = spotlight.transform.position - sectorCenter;

                float angle = Vector3.Angle(spotlight.transform.forward, -direction);

                bool isShadow = true;

                if (angle <= spotlight.spotAngle / 2)
                {
                    bool hit = Physics.Raycast(sectorCenter, direction, direction.magnitude, LayerMask.GetMask("Default"));

                    if (hit)
                    {
                        if (debugShadows)
                            Debug.DrawRay(sectorCenter, direction, Color.red);
                    }
                    else
                        isShadow = false;
                }
                else if (debugShadows)
                    Debug.DrawRay(sectorCenter, direction, Color.red);

                Color[] shadowMapPixels = shadowMap.GetPixels(j * widthJump, (numVertSectors - i) * heightJump, widthJump, heightJump, 0);

                int numShadowPixels = 0;
                int numLightPixels = 0;
                int numWhateverPixels = 0;

                foreach (Color pixel in shadowMapPixels)
                {
                    if (pixel.r == 1f && pixel.g == 1f && pixel.b == 1f)
                        numLightPixels++;
                    else if (pixel.r == 0f && pixel.g == 0f && pixel.b == 0f)
                        numShadowPixels++;
                    else
                        numWhateverPixels++;
                }

                // If numWhateverPixels is higher, then it doesn't matter if is shadow or light
                if (numWhateverPixels > numLightPixels && numWhateverPixels > numShadowPixels)
                {
                    if(debugTextureWhatever)
                        Debug.DrawRay(sectorCenter, direction, Color.yellow);
                }
                else
                {
                    bool shouldBeShadow = numLightPixels <= numShadowPixels;

                    if (shouldBeShadow && debugTextureShadows)
                        Debug.DrawRay(sectorCenter, direction, Color.blue);

                    if (shouldBeShadow == isShadow)
                        numCorrectSectors++;
                    else
                        numWrongSectors++;
                }
            }
        }

        float correctPercentage = ((float)numCorrectSectors / (float)(numCorrectSectors + numWrongSectors)) * 100;

        Debug.Log(correctPercentage + "%");

        // DEBUG: draw plane bounding box
        for (int i = 0; i <= numVertSectors; i++)
        {
            Vector3 leftPoint = Vector3.Lerp(planeTopLeft, planeBottomLeft, i / (float)numVertSectors);
            Vector3 rightPoint = Vector3.Lerp(planeTopRight, planeBottomRight, i / (float)numVertSectors);

            if (debugSectors)
                Debug.DrawLine(leftPoint, rightPoint, debugColor);
        }

        for (int i = 0; i <= numHorizSectors; i++)
        {
            Vector3 topPoint = Vector3.Lerp(planeTopLeft, planeTopRight, i / (float)numHorizSectors);
            Vector3 bottomPoint = Vector3.Lerp(planeBottomLeft, planeBottomRight, i / (float)numHorizSectors);

            if (debugSectors)
                Debug.DrawLine(topPoint, bottomPoint, debugColor);
        }
    }
}
