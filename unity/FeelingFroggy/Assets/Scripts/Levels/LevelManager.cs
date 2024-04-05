using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Used to manage the levels in the game.
/// </summary>
public class LevelManager : MonoBehaviour
{
    private LevelProgress[] _levelsProgress;
    private const string LevelProgressDataKey = "level_progress";

    private const string DifficultyDataKey = "difficulty";

    private LevelInfo _currentLevel;
    private LevelController _currentLevelController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Don't destroy the object, even if scene changes
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        // Load the level progress data
        _levelsProgress = PersistenceManager.Load<LevelProgress[]>(LevelProgressDataKey);

        // If there is no level progress data, create an empty array
        _levelsProgress ??= Array.Empty<LevelProgress>();

        // Load difficulty. Default = Normal
        CurrentDifficulty = (Difficulty)PlayerPrefs.GetInt(DifficultyDataKey, 2);

        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Invoke event
        Loaded?.Invoke();
    }

    /// <summary>
    /// A static instance of the level manager.
    /// </summary>
    public static LevelManager Instance { get; private set; }

    public static Action Loaded;

    /// <summary>
    /// The information about each level in the game.
    /// </summary>
    public static readonly LevelInfo[] Levels = {
        new()
        {
            level = 1,
            levelTitle = "Hoppers Hollow",
            sceneName = SceneNames.Level1,
            starsScores = new [] { 400, 650, 850 }
        },
        new()
        {
            level = 2,
            levelTitle = "Tadpole Creek",
            sceneName = SceneNames.Level2,
            starsScores = new [] { 450, 675, 900 }
        },
        new()
        {
            level = 3,
            levelTitle = "Ribbit Ridge",
            sceneName = SceneNames.Level3,
            starsScores = new [] { 300, 400, 500 }
        },
        new()
        {
            level = 4,
            levelTitle = "Toadwater",
            sceneName = SceneNames.Level4,
            starsScores = new [] { 300, 500, 700 }
        },
        new()
        {
            level = 5,
            levelTitle = "Croakville",
            sceneName = SceneNames.Level5,
            starsScores = new [] { 500, 700, 1000 }
        },
        new()
        {
            level = 6,
            levelTitle = "Frogsmouth",
            sceneName = SceneNames.Level6,
            starsScores = new [] { 300, 500, 900 }
        },
        new()
        {
            level = 7,
            levelTitle = "Leapfrog Heights",
            sceneName = SceneNames.Level7,
            starsScores = new [] { 0, 0, 0 }
        },
        new()
        {
            level = 8,
            levelTitle = "The Frog King's Reign",
            sceneName = SceneNames.Level8,
            starsScores = new [] { 400, 600, 800 }
        }
    };
    
    /// <summary>
    /// The current level the player is in.
    /// </summary>
    public static LevelInfo CurrentLevel => Instance._currentLevel;

    public static LevelController CurrentLevelController => Instance._currentLevelController;

    /// <summary>
    /// The current game state.
    /// </summary>
    public static GameState GameState { get; private set; }


    public static Difficulty CurrentDifficulty { get; private set; } = Difficulty.Normal;

    /// <summary>
    /// Gets the player's progress for all levels they have progress in.
    /// </summary>
    /// <returns>The player's progress.</returns>
    public static LevelProgress[] GetAllProgress()
    {
        return Instance._levelsProgress;
    }

    /// <summary>
    /// Gets the player's progress for the specified level.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <returns>The player's progress.</returns>
    public static LevelProgress GetProgress(LevelInfo level)
    {
        return Instance._levelsProgress.FirstOrDefault(x => x.level == level.level);
    }

    /// <summary>
    /// Gets all levels the player can play.
    /// </summary>
    /// <returns>The levels the player can play.</returns>
    public static LevelInfo[] GetUnlockedLevels()
    {
        var allProgress = GetAllProgress();

        // The player unlocked all levels they completed AND the next level.
        var lastLevel = allProgress.Max(x => x.level) + 1;

        return Levels.Where(x => x.level <= lastLevel).ToArray();
    }

    /// <summary>
    /// Load a level's scene.
    /// </summary>
    /// <param name="level">The level to be loaded.</param>
    public static void LoadLevelScene(LevelInfo level)
    {
        GameState = GameState.LevelStarting;

        Instance._currentLevel = level;

        SceneManager.LoadScene(level.sceneName);
    }

    /// <summary>
    /// Loads the main menu scene and performs any necessary clean up.
    /// </summary>
    public static void LoadMainMenu()
    {
        Instance._currentLevel = null;
        Instance._currentLevelController = null;

        SceneManager.LoadScene(SceneNames.MainMenu);

        // Ensure time scale is regular
        Time.timeScale = 1;

        // Ensure cursor is visible and unlocked
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == SceneNames.MainMenu)
        {
            return;
        }

        GameState = GameState.LevelStarted;

        var levelObject = GameObject.Find("Level Controller");

        if (levelObject == null)
        {
            Debug.LogError("No object named 'Level Controller' in level scene!");
            return;
        }

        var levelController = levelObject.GetComponent<LevelController>();

        if (levelController == null)
        {
            Debug.LogError("'Level Controller' object does not have level controller component!");
            return;
        }

        _currentLevelController = levelController;

        levelController.LevelWon += TriggerLevelWon;
        levelController.LevelLost += TriggerLevelLost;
    }

    private static void TriggerLevelWon(int score, float timeTaken)
    {
        if (GameState != GameState.LevelStarted)
        {
            Debug.Log("Attempted to trigger level won when level is not started.");
            return;
        }

        var prevBest = GetProgress(CurrentLevel);
        
        GameState = GameState.LevelEnded;

        GameController.Instance.PauseGame();

        CurrentLevelController.endMenuController.ShowLevelWon(score, timeTaken, prevBest);
        
        // Save level score and time taken
        Instance.SaveLevelProgress(CurrentLevel, score, timeTaken);
    }

    private static void TriggerLevelLost()
    {
        if (GameState != GameState.LevelStarted)
        {
            Debug.Log("Attempted to trigger level lost when level is not started.");
            return;
        }

        GameState = GameState.LevelEnded;

        GameController.Instance.PauseGame();

        CurrentLevelController.endMenuController.ShowLevelLost();
    }

    private void SaveLevelProgress(LevelInfo levelInfo, int score, float timeTaken)
    {
        var levelProgress = GetProgress(levelInfo);

        if (levelProgress == null)
        {
            levelProgress = new LevelProgress
            {
                level = levelInfo.level,
                score = score,
                timeTaken = timeTaken
            };

            Instance._levelsProgress = Instance._levelsProgress.Append(levelProgress).ToArray();
        }
        else
        {
            // If previous score was higher and previous time taken was lower, don't update
            if (levelProgress.score > score && levelProgress.timeTaken < timeTaken)
            {
                return;
            }

            levelProgress.score = Mathf.Max(levelProgress.score, score);
            levelProgress.timeTaken = Mathf.Min(levelProgress.timeTaken, timeTaken);
        }

        PersistenceManager.Save(LevelProgressDataKey, Instance._levelsProgress);
    }

    public void DeleteAllLevelProgress()
    {
        PersistenceManager.Save<LevelProgress[]>(LevelProgressDataKey, null);

        _levelsProgress = Array.Empty<LevelProgress>();
    }

    public static void SetDifficulty(Difficulty difficulty)
    {
        CurrentDifficulty = difficulty;

        PlayerPrefs.SetInt(DifficultyDataKey, (int)difficulty);
    }
}
