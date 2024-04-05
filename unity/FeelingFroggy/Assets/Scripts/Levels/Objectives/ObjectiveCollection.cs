using System;
using UnityEngine;

public class ObjectiveCollection : LevelObjective, IUISupportedObjective
{
    [Header("Difficulty Settings")]

    [Tooltip("The minimum number of completed objectives for this objective to be considered completed (-1 means all) when on the Easy difficulty.")]
    public int minimumCompletedEasy = -1;

    [Tooltip("The minimum number of completed objectives for this objective to be considered completed (-1 means all) when on the Normal difficulty.")]
    public int minimumCompletedNormal = -1;

    [Tooltip("The minimum number of completed objectives for this objective to be considered completed (-1 means all) when on the Hard difficulty.")]
    public int minimumCompletedHard = -1;

    [Header("UI Settings")]
    public string textFormat = "Collect Stuff ({0}/{1})";

    [Header("Objectives")]
    public LevelObjective[] objectives;

    public int MinimumCompleted { get; private set; }

    public int CompletedObjectives { get; private set; }

    private void Awake()
    {
        MinimumCompleted = LevelManager.CurrentDifficulty switch
        {
            Difficulty.Easy => minimumCompletedEasy,
            Difficulty.Normal => minimumCompletedNormal,
            Difficulty.Hard => minimumCompletedHard,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (MinimumCompleted == -1)
        {
            MinimumCompleted = objectives.Length;
        }
    }

    private void Start()
    {
        CompletedObjectives = 0;

        foreach (var objective in objectives)
        {
            objective.Finished += OnObjectiveFinished;
            objective.Unfinished += OnObjectiveUnfinished;

            if (objective.IsFinished)
            {
                CompletedObjectives++;
            }
        }
    }

    private void CheckFinished()
    {
        SetFinished(CompletedObjectives >= MinimumCompleted);
    }

    private void OnObjectiveFinished()
    {
        CompletedObjectives++;

        CheckFinished();

        ProgressUpdated?.Invoke(this);
    }

    private void OnObjectiveUnfinished()
    {
        CompletedObjectives--;

        CheckFinished();

        ProgressUpdated?.Invoke(this);
    }

    public string GetUIText()
    {
        return string.Format(textFormat, CompletedObjectives, MinimumCompleted);
    }

    public event Action<IUISupportedObjective> ProgressUpdated;

    public override void SetEnabled(bool isEnabled)
    {
        base.SetEnabled(isEnabled);

        foreach (var objective in objectives)
        {
            objective.SetEnabled(isEnabled);
        }
    }
}