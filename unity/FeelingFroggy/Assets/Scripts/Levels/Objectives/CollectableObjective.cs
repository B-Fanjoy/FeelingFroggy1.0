using System;
using System.Collections;
using UnityEngine;

public class CollectableObjective : LevelObjective
{
    public event Action<CollectableObjective> OnCollected;

    [Header("Score")]
    public int score = 10;

    [Header("Animation")]
    public float spinSpeed;
    public float fadeTime = 3;

    [Header("References")]
    public GameObject @object;
    public Light light;

    private bool _collected;

    private void Update()
    {
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!IsEnabled)
        {
            return;
        }

        if (!collider.gameObject.CompareTag("Player"))
        {
            return;
        }

        Collect();
    }

    public void Collect()
    {
        if (!IsEnabled || _collected)
        {
            return;
        }

        _collected = true;

        LevelController.Instance.AddScore(score);

        SetFinished(true);

        OnCollected?.Invoke(this);

        StartCoroutine(FadeOut());
    }
    
    private IEnumerator FadeOut()
    {
        if (light != null || @object != null)
        {
            var startTime = Time.time;
            var endTime = startTime + fadeTime;

            var originalScale = @object?.transform.localScale ?? Vector3.zero;
            var originalIntensity = light?.intensity ?? 0;

            while (Time.time < endTime)
            {
                var t = (Time.time - startTime) / fadeTime;

                if (@object != null)
                {
                    @object.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t * t);
                }

                if (light != null)
                {
                    light.intensity = Mathf.Lerp(originalIntensity, 0, t * t);
                }

                yield return null;
            }
        }
        
        Destroy(gameObject);
    }
}
