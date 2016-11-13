using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Nest : MonoBehaviour
{
    [ReadOnly]
    public Ant antPrefab;

    [ReadOnlyRange(1, 1000)]
    public float numberOfAnts = 100;

    private List<Ant> ants = new List<Ant>();

    #region Unity Events

    void Start()
    {
	
	}
	
	void Update()
    {
	
	}

    #endregion

    private void SpawnAnt()
    {
        if (ants.Count < numberOfAnts)
        {
            Ant ant = Instantiate<Ant>(antPrefab);
            ant.transform.position = transform.position;
            ant.isFearContagious = Simulation.Instance.FearEnabled;
            ants.Add(ant);
        }
    }
}
