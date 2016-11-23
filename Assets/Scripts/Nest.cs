using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class Nest : MonoBehaviour
{
    [ReadOnly]
    public Transform pilar;

    [ReadOnly]
    public float pilarSpeed = 40f;

    [ReadOnly]
    public Ant antPrefab;

    [ReadOnly]
    public bool autoEnableOnStart = true;

    [ReadOnly]
    public float autoEnableOnStartDelay = 3f;

    [ReadOnly]
    public bool spawnEnabled = false;

    [ReadOnlyRange(1, 1000)]
    public int maxNumberOfAnts = 100;

#if UNITY_EDITOR
    [ReadOnly(RunMode.Any)]
    public int currentNumberOfAnts;
#endif

    [ReadOnly]
    public float spawnRate = 10f;

    [ReadOnly]
    public float minSpawnRadius = 1;

    [ReadOnly]
    public float maxSpawnRadius = 5;

    [ReadOnly]
    public float spawnHeight = 2f;

    private bool canSpawn = false;
    private float spawnDeltaTime = 0;
    private List<Ant> ants = new List<Ant>();

    #region Unity Events

    void Start()
    {
        StartCoroutine(ShowPilar());

        if (autoEnableOnStart)
        {
            StartCoroutine(AutoEnableOnStart());
        }
	}

    void Update()
    {
        // Update Ant Spawn
        if (canSpawn != spawnEnabled)
        {
            if (spawnEnabled)
                spawnDeltaTime = 0;

            canSpawn = spawnEnabled;
        }

        // Spawn Ants according to configured spawn rate
        if (canSpawn)
        {
            spawnDeltaTime += Time.deltaTime;
            float spawnPeriod = 1f / spawnRate;
            while (ants.Count < maxNumberOfAnts && spawnDeltaTime >= spawnPeriod)
            {
                Ant ant = CreateAnt();
                ants.Add(ant);
                #if UNITY_EDITOR
                currentNumberOfAnts = ants.Count;
                #endif
                spawnDeltaTime -= spawnPeriod;
            }
        }


    }

    #endregion

    private IEnumerator AutoEnableOnStart()
    {
        yield return new WaitForSeconds(autoEnableOnStartDelay);
        spawnEnabled = true;
    }

    private IEnumerator ShowPilar()
    {
        Vector3 position = pilar.position;
        while (position.y < 0)
        {
            yield return new WaitForEndOfFrame();
            position += new Vector3(0, pilarSpeed * Time.deltaTime, 0);
            if (position.y > 0)
                position.y = 0;
            pilar.position = position;
        }
    }

    private Ant CreateAnt()
    {
        Vector3 position = transform.position;

        float spawnDistance = Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector2 point = (Random.insideUnitCircle.normalized) * spawnDistance;
        Vector3 spawnPoint = position + new Vector3(point.x, spawnHeight, point.y);

        Ant ant = Instantiate<Ant>(antPrefab);
        ant.transform.parent = transform;
        ant.transform.position = spawnPoint;
        ant.transform.LookAt(2 * spawnPoint - new Vector3(position.x, spawnPoint.y, position.z));
        return ant;
    }
}
