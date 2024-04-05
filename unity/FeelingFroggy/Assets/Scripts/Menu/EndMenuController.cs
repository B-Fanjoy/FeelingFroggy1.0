using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class EndMenuController : MonoBehaviour
{
    [Header("UI Settings")]
    public float fadeInTime = 2f;

    [Header("UI Components")]
    public GameObject endMenuObject;

    public GameObject levelPassedObject;
    public GameObject levelFailedObject;

    public TextMeshProUGUI levelTitleText;

    public GameObject starsContainer;

    public TextMeshProUGUI star1RequirementText;
    public TextMeshProUGUI star2RequirementText;
    public TextMeshProUGUI star3RequirementText;

    public GameObject star1AchievedObject;
    public GameObject star2AchievedObject;
    public GameObject star3AchievedObject;

    public GameObject scoreContainerObject;
    public GameObject prevBestScoreContainerObject;

    public TextMeshProUGUI scoreValueText;
    public TextMeshProUGUI prevBestScoreValueText;

    public GameObject timeContainerObject;
    public GameObject prevBestTimeContainerObject;

    public TextMeshProUGUI timeValueText;
    public TextMeshProUGUI prevBestTimeValueText;

    public TextMeshProUGUI levelLostText;

    public GameObject nextLevelButtonObject;

    public string[] LevelLostMessages =
    {
        "It happens. Try again!",
        "Keep going, warrior.",
        "One loss, many wins ahead.",
        "No worries. Keep playing!",
        "Next game, new victory.",
        "Stay determined. Keep playing!",
        "Bounce back stronger. Try again!",
        "Failure is temporary. Keep trying!",
        "You'll get 'em next time!",
        "Try, try again. You got this!"
    };

    private void Start()
    {
        // Do not show until level ended
        endMenuObject.SetActive(false);

        // Hide by default
        levelPassedObject.SetActive(false);
        levelFailedObject.SetActive(false);
        starsContainer.SetActive(false);
        scoreContainerObject.SetActive(false);
        prevBestScoreContainerObject.SetActive(false);
        timeContainerObject.SetActive(false);
        prevBestTimeContainerObject.SetActive(false);

        nextLevelButtonObject.SetActive(LevelManager.Levels.Length > LevelManager.CurrentLevel.level);
    }

    private string FormatTime(float timeTaken)
    {
        var time = TimeSpan.FromSeconds(timeTaken);

        return $"{(int)time.TotalMinutes:00}:{(time.TotalSeconds % 60):00.00}";
    }

    public void ShowLevelWon(int score, float timeTaken, LevelProgress prevBest)
    {
        levelPassedObject.SetActive(true);
        starsContainer.SetActive(true);
        scoreContainerObject.SetActive(true);
        prevBestScoreContainerObject.SetActive(true);
        timeContainerObject.SetActive(true);
        prevBestTimeContainerObject.SetActive(true);

        levelTitleText.text = LevelManager.CurrentLevel.levelTitle;

        endMenuObject.SetActive(true);

        var starRequirements = LevelManager.CurrentLevel.starsScores;

        star1RequirementText.text = starRequirements[0].ToString();
        star2RequirementText.text = starRequirements[1].ToString();
        star3RequirementText.text = starRequirements[2].ToString();

        star1AchievedObject.SetActive(score >= starRequirements[0]);
        star2AchievedObject.SetActive(score >= starRequirements[1]);
        star3AchievedObject.SetActive(score >= starRequirements[2]);

        scoreValueText.text = score.ToString();
        timeValueText.text = FormatTime(timeTaken);

        if (prevBest != null)
        {
            prevBestScoreValueText.text = prevBest.score.ToString();
            prevBestScoreContainerObject.SetActive(true);

            prevBestTimeValueText.text = FormatTime(prevBest.timeTaken);
            prevBestTimeContainerObject.SetActive(true);
        }
        else
        {
            prevBestScoreContainerObject.SetActive(false);
            prevBestTimeContainerObject.SetActive(false);
        }

        StartCoroutine(FadeInEndScreen());
    }

    public void ShowLevelLost()
    {
        levelFailedObject.SetActive(true);

        levelTitleText.text = LevelManager.CurrentLevel.levelTitle;

        endMenuObject.SetActive(true);

        levelLostText.gameObject.SetActive(true);
        levelLostText.text = LevelLostMessages[UnityEngine.Random.Range(0, LevelLostMessages.Length)];

        StartCoroutine(FadeInEndScreen());
    }

    public void ClickMainMenu()
    {
        LevelManager.LoadMainMenu();
    }

    public void ClickRestartLevel()
    {
        LevelManager.LoadLevelScene(LevelManager.CurrentLevel);
    }

    public void ClickNextLevel()
    {
        var currentLevel = LevelManager.CurrentLevel;

        // We don't +1 because the level field is 1-indexed.
        var nextLevel = LevelManager.Levels[currentLevel.level];

        LevelManager.LoadLevelScene(nextLevel);
    }

    private IEnumerator FadeInEndScreen()
    {
        endMenuObject.SetActive(true);
        endMenuObject.GetComponent<CanvasGroup>().alpha = 0f;

        while (endMenuObject.GetComponent<CanvasGroup>().alpha < 1f)
        {
            endMenuObject.GetComponent<CanvasGroup>().alpha += Time.unscaledDeltaTime / fadeInTime;

            yield return null;
        }
    }
}
