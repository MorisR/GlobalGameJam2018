
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


[Serializable]
public class Wave
{
  // List<AstroidSpawner>

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

    protected virtual bool Spawn()
    {
        if (_astrodiPrefabs.Count == 0) return false;
        if (!isSpawning)return false;
        if (_currentSpawnCount == _spawnCount)
        {
            isSpawning = false;
            return false;
        }

       // _currentSpawnCount--;
        return true;
    }

    protected Astroid GetReadyAstroid()
    {
        lock (gameObject)
        {


            Astroid astroidScript = _astroidInstances.FirstOrDefault(x => x != null && !x.Instance.activeInHierarchy);
            if (astroidScript == null)
            {
                var astroidObject = Instantiate<GameObject>(_astrodiPrefabs[Random.Range(0, _astrodiPrefabs.Count)]);
                astroidScript = astroidObject.GetComponent<Astroid>();
                if (astroidScript == null)
                    astroidScript = astroidObject.GetComponentInChildren<Astroid>();
                _astroidInstances.Add(astroidScript);
                astroidScript.Instance.SetActive(false);
            }
            _currentSpawnCount++;
            return astroidScript;
        }
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


