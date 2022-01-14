using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShadowDetection : MonoBehaviour
{
    public Texture2D shadowMap;

    public Light spotlight;

    public TextMeshProUGUI percentageText;
    public Button nextLevelButton;

    private MeshFilter meshFilter;

    private int numCorrectSectors = 0;
    private int numWrongSectors = 0;

    private int currentSector = 0;
    public int sectorsPerFrame = 10;

    public int numHorizSectors = 64;
    public int numVertSectors = 32;

    private int shadowWidthJump;
    private int shadowHeightJump;

    public bool debugSectors = false;
    public bool debugShadows = false;
    public bool debugTextureShadows = false;
    public bool debugTextureWhatever = false;
    public Color debugColor = Color.green;

    private bool paused = false;
    private const int nLevels = 4;
    private Image star1;
    private Image star2;
    private Image star3;
    private const float oneStarThreshold = 80;
    private const float twoStarThreshold = 90;
    private const float threeStarThreshold = 95;
    public float authorThreshold = 100;
    private Color defaultColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private Color authorColor = new Color(1.0f, 0.5f, 0.0f, 1.0f);
    private float correctPercentage = 0;

    void OnEnable()
    {
        sectorsPerFrame = PlayerPrefs.GetInt("sectorsPerFrame", 100);
    }

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        if ((numHorizSectors == 0) || (numHorizSectors & (numHorizSectors - 1)) != 0)
        {
#if UNITY_EDITOR
            Debug.LogError("Invalid numHorizSectors; must be power of 2 (" + numHorizSectors + " is not power of 2)");
#else
            Utils.ForceCrash(ForcedCrashCategory.Abort);
#endif
        }

        if ((numVertSectors == 0) || (numVertSectors & (numVertSectors - 1)) != 0)
        {
#if UNITY_EDITOR
            Debug.LogError("Invalid numVertSectors; must be power of 2 (" + numVertSectors + " is not power of 2)");
#else
            Utils.ForceCrash(ForcedCrashCategory.Abort);
#endif
        }

        shadowWidthJump = Mathf.FloorToInt(shadowMap.width / numHorizSectors);
        shadowHeightJump = Mathf.FloorToInt(shadowMap.height / numVertSectors);

        nextLevelButton.onClick.AddListener(HandleNextLevel);
        Image[] stars = nextLevelButton.GetComponentsInChildren<Image>();

        foreach (Image star in stars)
        {
            if (star.gameObject.name == "Star1") star1 = star;
            else if (star.gameObject.name == "Star2") star2 = star;
            else if (star.gameObject.name == "Star3") star3 = star;
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if ((numHorizSectors == 0) || (numHorizSectors & (numHorizSectors - 1)) != 0)
            Debug.LogError("Invalid numHorizSectors; must be power of 2 (" + numHorizSectors + " is not power of 2)");

        if ((numVertSectors == 0) || (numVertSectors & (numVertSectors - 1)) != 0)
            Debug.LogError("Invalid numVertSectors; must be power of 2 (" + numVertSectors + " is not power of 2)");
#endif

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

#if UNITY_EDITOR
        shadowWidthJump = Mathf.FloorToInt(shadowMap.width / numHorizSectors);
        shadowHeightJump = Mathf.FloorToInt(shadowMap.height / numVertSectors);
#endif

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
#if UNITY_EDITOR
                    if (debugShadows)
                        Debug.DrawRay(sectorCenter, direction, Color.red, Time.deltaTime + 0.1f);
#endif
                }
                else
                    isShadow = false;
            }
            else
            {
                isInsideLightCone = false;
#if UNITY_EDITOR
                if (debugShadows)
                    Debug.DrawRay(sectorCenter, direction, Color.red, Time.deltaTime + 0.1f);
#endif
            }

            Color[] shadowMapPixels = shadowMap.GetPixels(indexHoriz * shadowWidthJump, (numVertSectors - indexVert - 1) * shadowHeightJump, shadowWidthJump, shadowHeightJump, 0);

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
#if UNITY_EDITOR
                if (debugTextureWhatever)
                    Debug.DrawRay(sectorCenter, direction, Color.yellow, Time.deltaTime + 0.1f);
#endif
            }
            else
            {
                bool shouldBeShadow = numLightPixels <= numShadowPixels;

#if UNITY_EDITOR
                if (shouldBeShadow && debugTextureShadows)
                    Debug.DrawRay(sectorCenter, direction, Color.blue, Time.deltaTime + 0.1f);
#endif

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
            correctPercentage = ((float)numCorrectSectors / (float)(numCorrectSectors + numWrongSectors)) * 100;

            if (!paused)
            {
                if (correctPercentage >= 95)
                    percentageText.text = correctPercentage.ToString("F2") + "%";
                else
                    percentageText.text = correctPercentage.ToString("F0") + "%";

                nextLevelButton.gameObject.SetActive((correctPercentage >= oneStarThreshold));
                star1.gameObject.SetActive((correctPercentage >= oneStarThreshold));
                star2.gameObject.SetActive((correctPercentage >= twoStarThreshold));
                star3.gameObject.SetActive((correctPercentage >= threeStarThreshold));

                star1.color = (correctPercentage >= authorThreshold) ? authorColor : defaultColor;
                star2.color = (correctPercentage >= authorThreshold) ? authorColor : defaultColor;
                star3.color = (correctPercentage >= authorThreshold) ? authorColor : defaultColor;
            }           

            currentSector = 0;
        }

        // DEBUG: draw plane bounding box
#if UNITY_EDITOR
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
#endif
    }

    public void Pause()
    {
        paused = !paused;
    }

    private void HandleNextLevel()
    {
        int currentLevel = PlayerPrefs.GetInt("Level", 0);
        int nextLevel = currentLevel + 1;
        PlayerPrefs.SetFloat("ScoreLevel" + currentLevel, correctPercentage);
        PlayerPrefs.SetFloat("AuthorScoreLevel" + currentLevel, authorThreshold);

        if (nextLevel > nLevels)
            SceneManager.LoadScene("MainMenu");
        else
        {
            PlayerPrefs.SetInt("Level", nextLevel);
            SceneManager.LoadScene("Level" + nextLevel);
        }
    }
}
