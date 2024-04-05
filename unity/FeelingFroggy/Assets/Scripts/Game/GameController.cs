using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public LevelController levelController;
    public PauseMenuController pauseMenuController;
    public GameObject player;

    private PlayerInput _playerInput;

    public bool IsGamePaused { get; private set; }

    public bool IsInDialogue => CurrentDialogue != null;
    public DialogueObjective CurrentDialogue { get; private set; }

    public bool IsPlayerInputDisabled => IsGamePaused || IsInDialogue;

    private void Awake()
    {
        _playerInput = player.GetComponent<PlayerInput>();

        Instance = this;
    }

    private void Start()
    {
        DialogueObjective.DialogueStarted += OnDialogueStarted;
        DialogueObjective.DialogueEnded += OnDialogueEnded;

        UnpauseGame();
    }

    private void OnDestroy()
    {
        Instance = null;

        DialogueObjective.DialogueStarted -= OnDialogueStarted;
        DialogueObjective.DialogueEnded -= OnDialogueEnded;
    }

    public event Action GamePaused;
    public event Action GameUnpaused;

    public void PauseGame()
    {
        IsGamePaused = true;

        // Stop time
        Time.timeScale = 0;

        // Disable player input
        //_playerInput.enabled = false;
        _playerInput.DeactivateInput();

        // Unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Show pause menu
        if (LevelManager.GameState == GameState.LevelStarted)
        {
            pauseMenuController.Show();
        }

        // Run paused events
        GamePaused?.Invoke();
    }

    public void UnpauseGame()
    {
        IsGamePaused = false;

        // Hide pause menu
        pauseMenuController.Hide();

        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Enable player input
        _playerInput.ActivateInput();

        // Resume time
        Time.timeScale = 1;

        // Run unpaused events
        GameUnpaused?.Invoke();
    }

    public void SkipDialogueLine()
    {
        if (CurrentDialogue == null)
        {
            return;
        }

        CurrentDialogue.SkipLine();
    }

    [UsedImplicitly]
    private void OnPause(InputValue inputValue)
    {
        if (!inputValue.isPressed)
        {
            return;
        }

        if (IsGamePaused)
        {
            UnpauseGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void OnDialogueStarted(DialogueObjective dialogueObjective)
    {
        CurrentDialogue = dialogueObjective;
    }

    private void OnDialogueEnded(DialogueObjective dialogueObjective)
    {
        CurrentDialogue = null;
    }
}