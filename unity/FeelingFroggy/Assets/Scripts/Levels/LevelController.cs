using System.Linq;
using UnityEngine;

/// <summary>
/// Manages level functionality common to all levels.
/// </summary>
public class LevelController : MonoBehaviour
{
    public EndMenuController endMenuController;
    public LevelObjective[] levelObjectives;

    private float _startTime;
    private bool _levelEnded;

    public int Score { get; private set; }

    public static LevelController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        _startTime = Time.time;

        foreach (var levelObjective in levelObjectives)
        {
            levelObjective.Finished += CheckWinCondition;
        }
    }

    public delegate void LevelWonCallback(int score, float timeTaken);
    public event LevelWonCallback LevelWon;

    public delegate void LevelLostCallback();
    public event LevelLostCallback LevelLost;

    /// <summary>
    /// Adds score to the current level score.
    /// </summary>
    /// <param name="score">The score to add to the current level score. Can be positive or negative.</param>
    public void AddScore(int score)
    {
        Score += score;
    }

    /// <summary>
    /// Used to manually trigger a win for the level.
    /// </summary>
    public void TriggerWin()
    {
        if (_levelEnded || LevelManager.GameState != GameState.LevelStarted)
        {
            return;
        }

        _levelEnded = true;

        var timeTaken = Time.time - _startTime;

        LevelWon?.Invoke(Score, timeTaken);
    }

    /// <summary>
    /// Used to manually trigger a loss for the level.
    /// </summary>
    /// <remarks>
    /// For example, if the player falls into a lava pit.
    /// </remarks>
    public void TriggerLost()
    {
        if (_levelEnded || LevelManager.GameState != GameState.LevelStarted)
        {
            return;
        }

        _levelEnded = true;

        LevelLost?.Invoke();
    }

    private void CheckWinCondition()
    {
        // If any objectives aren't finished, return.
        if (levelObjectives.Any(levelObjective => !levelObjective.IsFinished))
        {
            return;
        }

        // If all objectives are finished, trigger win.
        TriggerWin();
    }
}
