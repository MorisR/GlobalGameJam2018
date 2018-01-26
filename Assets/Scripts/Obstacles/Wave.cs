using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Wave : MonoBehaviour
{
    //  [SerializeField] private <AstroidSpawner> astroidSpawner;
    [SerializeField] private List<WaveSettings> _waves;

    private bool isRunning;
    public void StartWave()
    {
        // StopCoroutine("WaveStartCorotiene");
        if (!isRunning)
            StartCoroutine("WaveStartCorotiene");
    }
    IEnumerator WaveStartCorotiene()
    {
        lock (gameObject)
        {
            if (isRunning) yield break;
            isRunning = true;
            for (int i = 0; i < _waves.Count; i++)
            {
                if (false /*todo: is paused*/) yield return StartCoroutine(WaitUntilEnabled());

                _waves[i].Spawner.StartSpawn();
                yield return new WaitForSeconds(_waves[i].Delay);
            }
            isRunning = false;
        }
    }
    IEnumerator WaitUntilEnabled()
    {
        while (true) //todo: is gamePaused
        {

        }

    }

    public bool IsRunning { get { return isRunning; } }

    protected virtual void Update()
    {

        if (Input.GetKeyDown(KeyCode.B))
            StartWave();

    }

}

[Serializable]
public class WaveSettings
{
    [SerializeField] private float delay;
    [SerializeField] private AstroidSpawner spawner;

    public AstroidSpawner Spawner
    {
        get
        {
            return spawner;
        }


    }
    public float Delay
    {
        get { return delay; }

    }
}
