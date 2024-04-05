using UnityEngine;
using UnityEngine.Events;

public class ObjectiveSeriesNode : MonoBehaviour
{
    public UnityEvent NodeStarted;
    public UnityEvent NodeFinished;

    public LevelObjective[] objectives;

    public bool IsStarted { get; private set; }

    public bool IsFinished { get; private set; }

    private int _finishedObjectives;

    private void Start()
    {
        if (!IsStarted)
        {
            foreach (var objective in objectives)
            {
                objective.SetEnabled(false);
            }
        }
    }

    public void StartNode()
    {
        if (IsStarted)
        {
            return;
        }

        IsStarted = true;

        _finishedObjectives = 0;

        foreach (var objective in objectives)
        {
            if (objective.IsFinished)
            {
                _finishedObjectives++;
            }

            objective.Finished += OnObjectiveFinished;
            objective.Unfinished += OnObjectiveUnfinished;

            objective.SetEnabled(true);
        }

        NodeStarted?.Invoke();
    }

    private void OnObjectiveFinished()
    {
        _finishedObjectives++;

        CheckFinished();
    }

    private void OnObjectiveUnfinished()
    {
        _finishedObjectives--;

        CheckFinished();
    }

    private void CheckFinished()
    {
        if (IsFinished)
        {
            return;
        }

        if (_finishedObjectives < objectives.Length)
        {
            return;
        }

        IsFinished = true;

        NodeFinished?.Invoke();
    }
}
