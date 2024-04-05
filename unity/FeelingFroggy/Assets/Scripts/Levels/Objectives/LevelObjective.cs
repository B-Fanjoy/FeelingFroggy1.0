using System;
using UnityEngine;

public abstract class LevelObjective : MonoBehaviour
{
    public bool IsFinished { get; private set; }
    public bool IsEnabled { get; private set; } = true; // enabled by default

    public event Action Finished;
    public event Action Unfinished;

    protected void SetFinished(bool isFinished)
    {
        if (IsFinished == isFinished)
        {
            return;
        }

        IsFinished = isFinished;

        if (IsFinished)
        {
            Finished?.Invoke();
        }
        else
        {
            Unfinished?.Invoke();
        }
    }

    public virtual void SetEnabled(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }

    public void AddToObjectivesUI()
    {
        if (this is IUISupportedObjective uiSupportedObjective)
        {
            ObjectivesUIController.Instance.AddObjective(uiSupportedObjective);
        }
    }

    public void RemoveFromObjectivesUI()
    {
        if (this is IUISupportedObjective uiSupportedObjective)
        {
            ObjectivesUIController.Instance.RemoveObjective(uiSupportedObjective);
        }
    }
}