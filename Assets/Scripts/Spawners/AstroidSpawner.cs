
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;



public class AstroidSpawner : MonoBehaviour
{
    [SerializeField] protected List<GameObject> _astrodiPrefabs;


    [SerializeField] protected int _spawnCount;
    protected int _currentSpawnCount;


    [SerializeField]  Vector2 _astroidSpeedRange;
    [SerializeField]  Vector2 _delayBwtweenSpawns;


    [Space][Space][Header("Spawn Pos")]
     protected Transform  _spawnPositionTransform;
    [SerializeField] protected Vector3 _distanceFromPos;


    protected bool isSpawning = false;
    [SerializeField] protected List<Astroid> _astroidInstances;


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
        if (Input.GetKeyDown(KeyCode.X))
            StartSpawn();
        if (Input.GetKeyDown(KeyCode.C))
            ResetAndStopSpawn();

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
        var pos = _spawnPositionTransform.position;

        return new Vector2(Random.Range(pos.x, pos.x+ _distanceFromPos.x)
           , Random.Range(pos.y, pos.y + _distanceFromPos.y) );
    }
    protected float GetRandomDelayBetweenSecondsInRange()
    {
        var posTemp = new Vector2(Mathf.Min(_delayBwtweenSpawns.x, _delayBwtweenSpawns.y), Mathf.Max(_delayBwtweenSpawns.x, _delayBwtweenSpawns.y));
        return Random.Range(posTemp.x, posTemp.y);
    }
    protected float GetRandomSpeedInRange()
    {
        var posTemp = new Vector2(Mathf.Min(_astroidSpeedRange.x, _astroidSpeedRange.y), Mathf.Max(_astroidSpeedRange.x, _astroidSpeedRange.y));
        return Random.Range(_astroidSpeedRange.x, _astroidSpeedRange.y);

    }

    private void OnDrawGizmosSelected()
    {

        Gizmos.DrawLine(transform.position, transform.position+ _distanceFromPos.x* Vector3.right);
        Gizmos.DrawLine(transform.position, transform.position+ _distanceFromPos.y* Vector3.up);
        Gizmos.DrawLine(transform.position + _distanceFromPos.x * Vector3.right, transform.position+ _distanceFromPos);
        Gizmos.DrawLine(transform.position + _distanceFromPos.y * Vector3.up, transform.position + _distanceFromPos);

    }






}


