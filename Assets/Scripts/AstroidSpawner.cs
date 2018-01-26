
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct Wave
{
   

/*
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
    }*/
}


public class AstroidSpawner : MonoBehaviour
{
    [SerializeField] protected List<GameObject> _astrodiPrefabs;


    [SerializeField] protected int _spawnCount;
    protected int _currentSpawnCount;


    [SerializeField]  Vector2 _astroidSpeedRange;
    [SerializeField]  Vector2 _delayBwtweenSpawns;


    [Space][Space][Header("Spawn Pos")]
    [SerializeField]  Vector3 _spawnPosition;
    [SerializeField]  Vector3 _distanceFromPos;


    protected bool isSpawning = false;
    [SerializeField] protected List<Astroid> _astroidInstances;


    public virtual void RemoveAstroids()
    {
        //todo create an event that all the asteroids implement 

    }
    public virtual void StartSpawn()
    {
        isSpawning = true;

    }

    public virtual void ResetAndStopSpawn()
    {
        _currentSpawnCount = 0;
        //todo Implement
    }
    //todo add onWaveStart/ onWaveEnd Events




    void Start () {
		
	}
    
	// Update is called once per frame
	void Update ()
	{
        if(_astrodiPrefabs.Count == 0)return;
	    Spawn();

	}

    protected virtual void Spawn()
    {
	    if (!isSpawning)return;

    }

    protected Astroid GetReadyAstroid()
    {
        Astroid astroidScript = _astroidInstances.FirstOrDefault(x=>x!= null && x.gameObject.activeInHierarchy);
        if (astroidScript == null)
        {
            var astroidObject = Instantiate<GameObject>(_astrodiPrefabs[Random.Range(0, _astrodiPrefabs.Count)]);
            astroidScript = astroidObject.GetComponent<Astroid>();
            if (astroidScript == null)
                astroidScript = astroidObject.GetComponentInChildren<Astroid>();
            _astroidInstances.Add(astroidScript);
        }

        return astroidScript;
    }

    protected Vector2 GetRandomPosInRange()
    {
        return new Vector2(Random.Range(_spawnPosition.x, _spawnPosition.x+ _distanceFromPos.x)
           , Random.Range(_spawnPosition.y, _spawnPosition.y + _distanceFromPos.y) );
    }
    protected float GetRandomDelayBetweenSecondsInRange()
    {
        return Random.Range(_delayBwtweenSpawns.x, _delayBwtweenSpawns.y);
    }
    protected float GetRandomSpeedInRange()
    {
        return Random.Range(_astroidSpeedRange.x, _astroidSpeedRange.y);
    }

    private void OnDrawGizmos()
    {

        Gizmos.DrawLine(_spawnPosition, _spawnPosition+ _distanceFromPos.x* Vector3.right);
        Gizmos.DrawLine(_spawnPosition, _spawnPosition+ _distanceFromPos.y* Vector3.up);
        Gizmos.DrawLine(_spawnPosition + _distanceFromPos.x * Vector3.right, _spawnPosition+ _distanceFromPos);
        Gizmos.DrawLine(_spawnPosition + _distanceFromPos.y * Vector3.up, _spawnPosition + _distanceFromPos);

    }




}