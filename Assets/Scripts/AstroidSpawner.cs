using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Wave
{
    [SerializeField] private int astroidCount;
    [SerializeField] private Vector2 astroidSpeedRange;
    [SerializeField] private Vector2 delayBwtweenSpawns;
    [SerializeField] private Vector2 spawnPosition;


    public int AstroidCount
    {
        get { return astroidCount; }
        set { astroidCount = value; }
    }
    public Vector2 AstroidSpeedRange
    {
        get { return astroidSpeedRange; }
        set { astroidSpeedRange = value; }
    }
    public Vector2 DelayBwtweenSpawns
    {
        get { return delayBwtweenSpawns; }
        set { delayBwtweenSpawns = value; }
    }
    public Vector2 SpawnPosition
    {
        get { return spawnPosition; }
        set { spawnPosition = value; }
    }
}


public class AstroidSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _astrodiPrefabs;
    [SerializeField] private List<Astroid> _astroidInstances;
    [SerializeField] private List<Wave> _waves;
    private bool isSpawning = false;


    public void RemoveAstroids()
    {
        

    }
    public void StartSpawn()
    {
        isSpawning = true;

    }
    //todo add onWaveStart/ onWaveEnd Events




    void Start () {
		
	}
    
	// Update is called once per frame
	void Update () {
		
	}




}
