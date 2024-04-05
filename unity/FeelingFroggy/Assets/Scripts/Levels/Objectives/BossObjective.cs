using System.Collections;
using UnityEngine;

public class BossObjective : LevelObjective
{
    public GameObject bossModel;
    public GameObject boomObject;

    public float boomDuration = 15f;

    public override void SetEnabled(bool isEnabled)
    {
        base.SetEnabled(isEnabled);

        if (isEnabled)
        {
            OnObjectiveEnabled();
        }
    }

    private void OnObjectiveEnabled()
    {
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        boomObject.SetActive(true);

        var startTime = Time.time;
        var startScale = bossModel.transform.localScale;

        // Scale down until timer runs out
        while (Time.time - startTime < boomDuration)
        {
            var scale = 1f - (Time.time - startTime) / boomDuration;
            bossModel.transform.localScale = startScale * scale;

            yield return null;
        }

        boomObject.SetActive(false);

        SetFinished(true);
    }
}