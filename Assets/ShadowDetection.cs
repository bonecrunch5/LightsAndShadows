using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowDetection : MonoBehaviour
{
    public Texture2D shadowMap;

    public Light spotlight;

    private MeshFilter meshFilter;

    private int numCorrectSectors = 0;
    private int numWrongSectors = 0;

    private int currentSector = 0;
    public int sectorsPerFrame = 10;

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
        int numSectorsInFrame = 0;

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

        if (currentSector == 0)
        {
            numCorrectSectors = 0;
            numWrongSectors = 0;
        }

        while (numSectorsInFrame < sectorsPerFrame && currentSector < numHorizSectors * numVertSectors)
        {
            int indexVert = currentSector / numHorizSectors;

            Vector3 leftPoint = Vector3.Lerp(planeTopLeft, planeBottomLeft, (indexVert + 0.5f) / numVertSectors);
            Vector3 rightPoint = Vector3.Lerp(planeTopRight, planeBottomRight, (indexVert + 0.5f) / numVertSectors);

            int indexHoriz = currentSector % numHorizSectors;

            Vector3 sectorCenter = Vector3.Lerp(leftPoint, rightPoint, (indexHoriz + 0.5f) / numHorizSectors);

            Vector3 direction = spotlight.transform.position - sectorCenter;

            float angle = Vector3.Angle(spotlight.transform.forward, -direction);

            bool isShadow = true;
            bool isInsideLightCone = true;

            if (angle <= spotlight.spotAngle / 2)
            {
                bool hit = Physics.Raycast(sectorCenter, direction, direction.magnitude, LayerMask.GetMask("Default"));

                if (hit)
                {
                    if (debugShadows)
                        Debug.DrawRay(sectorCenter, direction, Color.red, Time.deltaTime + 0.1f);
                }
                else
                    isShadow = false;
            }
            else
            {
                isInsideLightCone = false;
                if (debugShadows)
                    Debug.DrawRay(sectorCenter, direction, Color.red, Time.deltaTime + 0.1f);
            }

            Color[] shadowMapPixels = shadowMap.GetPixels(indexHoriz * widthJump, (numVertSectors - indexVert - 1) * heightJump, widthJump, heightJump, 0);

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
                if (debugTextureWhatever)
                    Debug.DrawRay(sectorCenter, direction, Color.yellow, Time.deltaTime + 0.1f);
            }
            else
            {
                bool shouldBeShadow = numLightPixels <= numShadowPixels;

                if (shouldBeShadow && debugTextureShadows)
                    Debug.DrawRay(sectorCenter, direction, Color.blue, Time.deltaTime + 0.1f);

                if (isInsideLightCone && shouldBeShadow == isShadow)
                {
                    numCorrectSectors++;

                    if (shouldBeShadow) numCorrectSectors += 4;
                }
                else
                {
                    numWrongSectors++;

                    if (shouldBeShadow) numWrongSectors += 4;
                }
            }

            numSectorsInFrame++;
            currentSector++;
        }

        if (currentSector >= numHorizSectors * numVertSectors)
        {
            float correctPercentage = ((float)numCorrectSectors / (float)(numCorrectSectors + numWrongSectors)) * 100;

            Debug.Log(correctPercentage + "%");

            currentSector = 0;
        }

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
