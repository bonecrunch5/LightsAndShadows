using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelector : MonoBehaviour
{
    private const float oneStarThreshold = 80;
    private const float twoStarThreshold = 90;
    private const float threeStarThreshold = 95;
    public Button[] buttons;

    private Color authorColor = new Color(1.0f, 0.5f, 0.0f, 1.0f);
    private Color disabledColor = new Color(0.25f, 0.25f, 0.25f, 1.0f);

    void Start()
    {
        int nLevels = buttons.Length;
        float[] scores = new float[nLevels];

        for (int i = 0; i < nLevels; i++)
        {
            scores[i] = PlayerPrefs.GetFloat("ScoreLevel" + (i + 1), 0);
            Button button = buttons[i];

            if ((i != 0) && (scores[i - 1] < oneStarThreshold))
            {
                button.image.color = disabledColor;
                button.interactable = false;
            }

            Image[] images = button.GetComponentsInChildren<Image>();
            TextMeshProUGUI[] scoreTexts = button.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI scoreText in scoreTexts)
            {
                if (scoreText.gameObject.name == "Score")
                {
                    scoreText.gameObject.SetActive(scores[i] > oneStarThreshold);
                    scoreText.text = "Score: " + scores[i].ToString("F2") + "%";
                }
            }

            float authorScore = PlayerPrefs.GetFloat("AuthorScoreLevel" + (i + 1), 100);

            foreach (Image star in images)
            {
                if (star.gameObject.name == "Star1")
                    star.gameObject.SetActive(scores[i] > oneStarThreshold);
                else if (star.gameObject.name == "Star2")
                    star.gameObject.SetActive(scores[i] > twoStarThreshold);
                else if (star.gameObject.name == "Star3")
                    star.gameObject.SetActive(scores[i] > threeStarThreshold);
                if (((star.gameObject.name == "Star1") || (star.gameObject.name == "Star2") || (star.gameObject.name == "Star3")) && (scores[i] >= authorScore))
                    star.color = authorColor;
            }
        }
    }

    public void PlayLevel(int level)
    {
        PlayerPrefs.SetInt("Level", level);
        SceneManager.LoadScene("Level" + level);
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
