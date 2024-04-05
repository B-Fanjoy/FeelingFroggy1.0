using System.Collections.Generic;
using UnityEngine;

public class WaypointArrow : MonoBehaviour
{
    public ObjectiveSeriesController objectiveSeriesController;
    public GameObject player;

    public GameObject arrow;

    public float minDistance = 8f;
    public float arrowDistance = 4f;

    private void Update()
    {
        if (objectiveSeriesController?.CurrentNode == null)
        {
            return;
        }

        Vector3? objectivePosition = null;
        var objectivePositionDistSqr = float.MaxValue;

        void CheckObjectives(IEnumerable<LevelObjective> objectives)
        {
            foreach (var objective in objectives)
            {
                if (objective == null)
                {
                    continue;
                }

                if (objective is ObjectiveCollection objectiveCollection)
                {
                    CheckObjectives(objectiveCollection.objectives);
                }
                else
                {
                    var distSqr = (objective.transform.position - player.transform.position).sqrMagnitude;

                    if (distSqr < objectivePositionDistSqr)
                    {
                        objectivePosition = objective.transform.position;
                        objectivePositionDistSqr = distSqr;
                    }
                }
            }
        }

        CheckObjectives(objectiveSeriesController.CurrentNode.objectives);

        if (!objectivePosition.HasValue)
        {
            return;
        }

        var dist = Mathf.Sqrt(objectivePositionDistSqr);

        if (dist < minDistance)
        {
            // Hide arrow
            arrow.SetActive(false);
            return;
        }

        var direction = (objectivePosition.Value - player.transform.position).normalized;
        var rotation = Quaternion.LookRotation(direction, Vector3.up);

        transform.rotation = rotation;
        transform.position = player.transform.position + direction * arrowDistance;

        // Show arrow
        arrow.SetActive(true);
    }
}
