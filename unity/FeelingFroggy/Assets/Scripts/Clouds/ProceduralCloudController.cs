using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralCloudController : MonoBehaviour
{
    [Header("Cloud Group Movement")]
    public Vector3 cloudGroupVelocity = Vector3.forward;
    public float cloudGroupTravelDistance = 1000f;

    [Header("Cloud Group Generation")]
    public GameObject cloudGroupSpawnPosition;
    public float cloudGroupSpawnDelay = 10f;

    public float cloudGroupSpanLeftRight = 100f;
    public float cloudGroupSpanFrontBack = 10f;
    public float cloudGroupMinDistance = 10f;

    public int cloudGroupMinPopulation = 4;
    public int cloudGroupMaxPopulation = 8;

    public GameObject[] cloudGroupPrefabs;

    private float _lastCloudGroupSpawn;

    private void Start()
    {
        _lastCloudGroupSpawn = Time.time;

        InitClouds();
    }

    private void Update()
    {
        SpawnNewClouds();
    }

    private void InitClouds()
    {
        var distanceInterval = cloudGroupVelocity.magnitude * cloudGroupSpawnDelay;

        for (var i = 0f; i < (int)(cloudGroupTravelDistance / distanceInterval); i++)
        {
            var position = cloudGroupSpawnPosition.transform.position + cloudGroupVelocity * cloudGroupSpawnDelay * i;

            GenerateCloudGroup(position, cloudGroupTravelDistance - distanceInterval * i);
        }

    }

    private void SpawnNewClouds()
    {
        if (Time.time - _lastCloudGroupSpawn <= cloudGroupSpawnDelay)
        {
            return;
        }

        _lastCloudGroupSpawn = Time.time;

        GenerateCloudGroup(cloudGroupSpawnPosition.transform.position, cloudGroupTravelDistance);
    }

    private void GenerateCloudGroup(Vector3 position, float travelDistance)
    {
        var cloudGroup = new GameObject
        {
            transform =
            {
                parent = transform,
                position = position,
                rotation = Quaternion.LookRotation(cloudGroupVelocity)
            }
        };

        var cloudGroupController = cloudGroup.AddComponent<CloudGroupController>();

        cloudGroupController.velocity = cloudGroupVelocity;
        cloudGroupController.maxDistance = travelDistance;

        var population = Random.Range(cloudGroupMinPopulation, cloudGroupMaxPopulation + 1);

        var cloudPositions = new List<Vector3>(population);

        for (var i = 0; i < population; i++)
        {
            var cloudPrefab = cloudGroupPrefabs[Random.Range(0, cloudGroupPrefabs.Length)];

            var tries = 5;

            var cloudPosition = Vector3.zero;
            var cloudPositionInvalid = true;

            while (tries-- > 0 && cloudPositionInvalid)
            {
                var x = Random.Range(-cloudGroupSpanLeftRight, cloudGroupSpanLeftRight);
                var z = Random.Range(-cloudGroupSpanFrontBack, cloudGroupSpanFrontBack);

                cloudPosition = new Vector3(x, 0, z);

                cloudPositionInvalid = cloudPositions.Any(otherCloudPosition =>
                    (cloudPosition - otherCloudPosition).sqrMagnitude < cloudGroupMinDistance * cloudGroupMinDistance);
            }

            if (cloudPositionInvalid)
            {
                continue;
            }

            var cloud = Instantiate(cloudPrefab, cloudGroup.transform);
            cloud.transform.localPosition = cloudPosition;
            cloud.transform.localRotation = Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up);

            cloudPositions.Add(cloudPosition);
        }
    }
}
