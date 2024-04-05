using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    [Header("Dependencies")]
    public GameObject deletePlayerDataConfirmObject;
    public GameObject deletePlayerDataDoneObject;
    public TMP_Dropdown difficultyDropdown;

    [Header("Settings")]
    public float clearPlayerDataConfirmTime = 15f;

    private float _lastClearPlayerDataClick = -10000; // start negative in case player clicks when they start the game

    private void Awake()
    {
        LevelManager.Loaded += OnLevelManagerLoaded;
    }

    private void OnLevelManagerLoaded()
    {
        difficultyDropdown.value = LevelManager.CurrentDifficulty switch
        {
            Difficulty.Easy => 0,
            Difficulty.Normal => 1,
            Difficulty.Hard => 2,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void ClearPlayerData()
    {
        LevelManager.Instance.DeleteAllLevelProgress();
    }

    public void ClickClearPlayerData()
    {
        if (Time.time - _lastClearPlayerDataClick < clearPlayerDataConfirmTime)
        {
            ClearPlayerData();
            deletePlayerDataConfirmObject.SetActive(false);
            deletePlayerDataDoneObject.SetActive(true);
        }
        else
        {
            deletePlayerDataConfirmObject.SetActive(true);
            _lastClearPlayerDataClick = Time.time;
        }
    }

    public void ClickBack()
    {
        _lastClearPlayerDataClick = -10000;
        deletePlayerDataConfirmObject.SetActive(false);
        deletePlayerDataDoneObject.SetActive(false);
    }

    public void DifficultyChange()
    {
        LevelManager.SetDifficulty(difficultyDropdown.value switch
        {
            0 => Difficulty.Easy,
            1 => Difficulty.Normal,
            2 => Difficulty.Hard,
            _ => throw new Exception("Invalid difficulty setting")
        });
    }
}
