
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
    Transform _spawnPositionTransform;
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




    protected virtual void Start ()
    {
        _spawnPositionTransform = transform;

    }

    // Update is called once per frame
    protected virtual void Update ()
	{
	    Spawn();

	}

    protected virtual void Spawn()
    {
        if (_astrodiPrefabs.Count == 0) return;
        if (!isSpawning)return;
        if (_currentSpawnCount == _spawnCount)
        {
            isSpawning = false;
            return;
        }


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
        return new Vector2(Random.Range(_spawnPositionTransform.position.x, _spawnPositionTransform.position.x+ _distanceFromPos.x)
           , Random.Range(_spawnPositionTransform.position.y, _spawnPositionTransform.position.y + _distanceFromPos.y) );
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

        Gizmos.DrawLine(transform.position, transform.position+ _distanceFromPos.x* Vector3.right);
        Gizmos.DrawLine(transform.position, transform.position+ _distanceFromPos.y* Vector3.up);
        Gizmos.DrawLine(transform.position + _distanceFromPos.x * Vector3.right, transform.position+ _distanceFromPos);
        Gizmos.DrawLine(transform.position + _distanceFromPos.y * Vector3.up, transform.position + _distanceFromPos);

    }




}