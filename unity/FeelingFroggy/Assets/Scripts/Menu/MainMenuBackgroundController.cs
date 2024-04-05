using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MainMenuBackgroundController : MonoBehaviour
{
    [Header("Required Components")]
    public GameObject player;
    public new Camera camera;
    public Transform cloudParent;

    [Header("Player Rotation")]
    public float torqueMultiplier;
    public float torqueDelay;

    [Header("Player Translation")]
    public float playerFallSpeed = 10;
    public float playerTurnAroundHeight = 100000;

    [Header("Camera Translation")]
    public float cameraAdjustSpeed = 0.1f;
    public float cameraMaxAdjustX = 1f;
    public float cameraMaxAdjustY = 2f;
    public float cameraMaxAdjustZ = 1f;

    [Header("Cloud Generation")]
    public float cloudRadius = 50;
    public float cloudHeightStep = 50;
    public float cloudSpawnHeight = 500;
    public float cloudDeleteHeight = 500;

    public float cloudCircleMinAngle = 0;
    public float cloudCircleMaxAngle = 360;

    public GameObject[] cloudPrefabs;

    private Vector3 _cameraOffset;
    private Vector3 _cameraCurrentAdjust;
    private Vector3 _cameraTargetAdjust;

    private Rigidbody _playerRigidbody;
    private float _lastTorqueChange = -1000;

    private readonly List<GameObject> _cloudInstances = new();
    private float _previousSpawnHeight;

    private void Start()
    {
        _cameraOffset = camera.transform.position - player.transform.position;

        _playerRigidbody = player.GetComponent<Rigidbody>();
        _playerRigidbody.velocity = new Vector3(0, -1 * playerFallSpeed, 0);

        // Make sure playerTurnAroundHeight is multiple of cloudHeightStep
        playerTurnAroundHeight = SteppedFloor(playerTurnAroundHeight);

        InitClouds();
        AddPlayerTorque();
    }

    private void Update()
    {
        CheckClouds();

        UpdateHeights();
    }

    private void FixedUpdate()
    {
        AddPlayerTorque();
    }

    private void LateUpdate()
    {
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        _cameraCurrentAdjust =
            Vector3.Lerp(_cameraCurrentAdjust, _cameraTargetAdjust, cameraAdjustSpeed * Time.deltaTime);

        if ((_cameraCurrentAdjust - _cameraTargetAdjust).sqrMagnitude < 0.5f)
        {
            _cameraTargetAdjust = new Vector3(
                Random.Range(-cameraMaxAdjustX, cameraMaxAdjustX),
                Random.Range(-cameraMaxAdjustY, cameraMaxAdjustY),
                Random.Range(-cameraMaxAdjustZ, cameraMaxAdjustZ));
        }

        camera.transform.position = player.transform.position + _cameraOffset + _cameraCurrentAdjust;
    }

    private void AddPlayerTorque()
    {
        if (_playerRigidbody == null)
        {
            return;
        }

        if (Time.time - _lastTorqueChange < torqueDelay)
        {
            return;
        }

        _lastTorqueChange = Time.time;

        _playerRigidbody.AddTorque(Random.onUnitSphere * torqueMultiplier);
    }

    private void InitClouds()
    {
        // Delete all existing clouds (main menu preset)
        foreach (Transform child in cloudParent)
        {
            Destroy(child.gameObject);
        }

        var playerHeight = player.transform.position.y;

        var cloudStartHeight = SteppedFloor(playerHeight + cloudDeleteHeight);
        var cloudEndHeight = SteppedFloor(playerHeight - cloudSpawnHeight);

        for (var height = cloudStartHeight; height >= cloudEndHeight; height -= cloudHeightStep)
        {
            AddClouds(height);
        }
    }

    private void AddClouds(float height)
    {
        _previousSpawnHeight = Mathf.Min(_previousSpawnHeight, height);

        var cloudPrefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];
        
        var circleAngleDegs = Random.Range(cloudCircleMinAngle, cloudCircleMaxAngle);
        var circleAngleRads = circleAngleDegs * Mathf.Deg2Rad;

        var circlePos = new Vector2(Mathf.Sin(circleAngleRads) * cloudRadius, Mathf.Cos(circleAngleRads) * cloudRadius);
        
        var cloudPos = new Vector3(circlePos.x, height, circlePos.y);
        
        var cloudRotation = Quaternion.AngleAxis(circleAngleDegs + 90, Vector3.up);

        var cloudInstance = Instantiate(cloudPrefab, cloudPos, cloudRotation, cloudParent);

        _cloudInstances.Add(cloudInstance);
    }

    private void AddClouds()
    {
        var playerHeight = player.transform.position.y;

        var newCloudsHeight = playerHeight - cloudSpawnHeight;
        newCloudsHeight = SteppedFloor(newCloudsHeight);

        if (newCloudsHeight >= _previousSpawnHeight)
        {
            return;
        }

        AddClouds(newCloudsHeight);
    }

    private void DeleteClouds()
    {
        var deleteHeight = player.transform.position.y + cloudDeleteHeight;

        var cloudsToDelete = _cloudInstances.Where(x => x.transform.position.y > deleteHeight).ToList();

        foreach (var cloud in cloudsToDelete)
        {
            _cloudInstances.Remove(cloud);
            Destroy(cloud);
        }
    }

    private void CheckClouds()
    {
        DeleteClouds();

        AddClouds();
    }

    private void UpdateHeights()
    {
        if (player.transform.position.y > -1 * playerTurnAroundHeight)
        {
            return;
        }
        
        player.transform.position = new Vector3(player.transform.position.x,
            player.transform.position.y + playerTurnAroundHeight,
            player.transform.position.z);

        _previousSpawnHeight += playerTurnAroundHeight;

        foreach (var cloud in _cloudInstances)
        {
            cloud.transform.position = new Vector3(cloud.transform.position.x,
                cloud.transform.position.y + playerTurnAroundHeight,
                cloud.transform.position.z);
        }
    }

    private float SteppedFloor(float value)
    {
        return (int)(value / cloudHeightStep) * cloudHeightStep;
    }
}
