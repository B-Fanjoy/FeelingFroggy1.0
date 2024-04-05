using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GE_Controller : MonoBehaviour
{
    public Transform target;
    public Transform[] points;

    private NavMeshAgent _navMeshAgent;

    public bool sightRequired;
    public float sightFov = 60;
    public float sightLostDelay = 2f;

    public float dangerZoneDistance = 5f;

    private bool _chase;
    private int _time;
    private int _currentPoint;

    private float _lastSeen;

    private float _initalPitch = 0;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _initalPitch = transform.eulerAngles.x;
    }

    private void Update()
    {
        _time = (int)Time.timeSinceLevelLoad;

        if (_chase)
        {
            /* Using navmesh's rotation instead of custom implementation
            transform.LookAt(target.position);

            // Quick hack for limiting pitch.
            var angles = transform.eulerAngles;
            angles.x = _initalPitch;
            transform.eulerAngles = angles;*/

            _navMeshAgent.SetDestination(target.position);
        }

        if (!_chase && _time % 10 == 0)
        {
            if (points.Length > 0 && transform.position != points[_currentPoint].position)
            {
                _navMeshAgent.SetDestination(points[_currentPoint].position);
                _currentPoint = (_currentPoint+1)%points.Length;
            }
        }


    }

    private bool CanSeePlayer()
    {
        if (!sightRequired)
        {
            return true;
        }

        var dist = Vector3.Distance(target.position, transform.position);

        if (dist < dangerZoneDistance)
        {
            return true;
        }

        // direction
        var playerDirVector = target.position - transform.position;

        var playerDir = Quaternion.LookRotation(playerDirVector);
        var aiDir = transform.rotation;

        // Calc angle between AI's forward and player direction
        var angle = Quaternion.Angle(playerDir, aiDir);

        // Out of FOV
        if (angle > sightFov / 2)
        {
            return false;
        }

        // Create ray from AI to player
        var ray = new Ray(transform.position, target.position - transform.position);

        // No hit
        if (!Physics.Raycast(ray, out var hitInfo, dist))
        {
            return false;
        }

        // Check if hit is player
        return hitInfo.transform.CompareTag("Player");
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("PlayerHitbox"))
        {
            return;
        }

        var hasSight = CanSeePlayer();

        if (hasSight)
        {
            _chase = true;
            _lastSeen = Time.time;
        }
        else if (Time.time - _lastSeen > sightLostDelay)
        {
            _chase = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _chase=false;
    }
}
