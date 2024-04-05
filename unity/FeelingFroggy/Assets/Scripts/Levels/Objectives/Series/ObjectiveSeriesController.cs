using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ObjectiveSeriesController : MonoBehaviour
{
    public ObjectiveSeriesNode[] objectiveSeries;

    private int _currentNodeIndex = -1;
    public ObjectiveSeriesNode CurrentNode { get; private set; }

    private void Start()
    {
        NextObjective();
    }

    private void NextObjective()
    {
        if (CurrentNode != null)
        {
            CurrentNode.NodeFinished.RemoveListener(OnNodeFinished);
            CurrentNode = null;
        }

        _currentNodeIndex++;

        if (_currentNodeIndex >= objectiveSeries.Length)
        {
            return;
        }

        CurrentNode = objectiveSeries[_currentNodeIndex];
        CurrentNode.NodeFinished.AddListener(OnNodeFinished);
        CurrentNode.StartNode();
    }

    private void OnNodeFinished()
    {
        NextObjective();
    }
}
