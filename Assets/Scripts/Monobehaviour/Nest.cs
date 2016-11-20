using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class Nest : MonoBehaviour
{
    [ReadOnly]
    public Ant antPrefab;

    [ReadOnly]
    public bool autoEnableOnStart = true;

    [ReadOnly]
    public float autoEnableOnStartDelay = 3f;

    [ReadOnly]
    public bool antSpawnEnabled = false;
    private bool canSpawnAnts = false;

    [ReadOnlyRange(1, 1000)]
    public int maxNumberOfAnts = 100;

    [ReadOnly]
    public float antSpawnRate = 10f;
    private float antSpawnDeltaTime = 0;

    [ReadOnly]
    public float minAntSpawnRadius = 1;

    [ReadOnly]
    public float maxAntSpawnRadius = 5;

#if UNITY_EDITOR
    [ReadOnly(RunMode.Any)]
    public int numberOfSpawnedAnts;
#endif

    public bool fearTransferEnabled = false;
    private bool canTransferFear = false;

    private List<Ant> ants = new List<Ant>();    

    private IEnumerator AutoEnableOnStart()
    {
        yield return new WaitForSeconds(autoEnableOnStartDelay);
        antSpawnEnabled = true;
    }

    #region Unity Events

    void Start()
    {
        if (autoEnableOnStart)
        {
            StartCoroutine(AutoEnableOnStart());
        }
	}

    void Update()
    {
        // Update Fear Transfer
        // if (canTransferFear != fearTransferEnabled)
        // {
        //     // Propagate Fear Transfer Settings
        //     for (int i = 0; i < ants.Count; ++i)
        //         ants[i].isFearContagious = fearTransferEnabled;
        // 
        //     canTransferFear = fearTransferEnabled;
        // }

        // Update Ant Spawn
        if (canSpawnAnts != antSpawnEnabled)
        {
            if (antSpawnEnabled)
                antSpawnDeltaTime = 0;

            canSpawnAnts = antSpawnEnabled;
        }

        // Spawn Ants according to configured spawn rate
        if (canSpawnAnts)
        {
            antSpawnDeltaTime += Time.deltaTime;
            float spawnPeriod = 1f / antSpawnRate;
            while (ants.Count < maxNumberOfAnts && antSpawnDeltaTime >= spawnPeriod)
            {
                Ant ant = CreateAnt();
                ants.Add(ant);
                #if UNITY_EDITOR
                numberOfSpawnedAnts = ants.Count;
                #endif
                antSpawnDeltaTime -= spawnPeriod;
            }
        }


    }

    #endregion

    private Ant CreateAnt()
    {
        float spawnDistance = Random.Range(minAntSpawnRadius, maxAntSpawnRadius);
        Vector2 point = (Random.insideUnitCircle.normalized) * spawnDistance;
        Vector3 spawnPoint = transform.position + new Vector3(point.x, 0, point.y);

        Ant ant = Instantiate<Ant>(antPrefab);
        ant.transform.position = spawnPoint;
        // ant.isFearContagious = canTransferFear;
        return ant;
    }
}
