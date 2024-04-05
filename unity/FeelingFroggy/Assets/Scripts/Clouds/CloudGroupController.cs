using UnityEngine;

public class CloudGroupController : MonoBehaviour
{
    public Vector3 velocity;
    public float maxDistance;

    private float _maxDistanceSqr;
    private Vector3 _startPosition;


    private void Start()
    {
        _startPosition = transform.position;
        _maxDistanceSqr = maxDistance * maxDistance;
    }

    private void Update()
    {
        transform.position += velocity * Time.deltaTime;

        if (Vector3.SqrMagnitude(transform.position - _startPosition) > _maxDistanceSqr)
        {
            Destroy(gameObject);
        }
    }
}
