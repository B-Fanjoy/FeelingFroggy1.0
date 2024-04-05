using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FinishLineObjective : LevelObjective, IUISupportedObjective
{
    [Header("Objective UI Settings")]
    public string objectiveName;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsEnabled)
        {
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            SetFinished(true);
        }

        ProgressUpdated?.Invoke(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsEnabled)
        {
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            SetFinished(false);
        }

        ProgressUpdated?.Invoke(this);
    }

    public string GetUIText()
    {
        return objectiveName;
    }

    public event Action<IUISupportedObjective> ProgressUpdated;
}
