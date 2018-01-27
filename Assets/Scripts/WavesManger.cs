using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesManger : Events.Tools.MonoBehaviour_EventManagerBase, Events.Groups.Level.Methods.IOnLevelStart{

    [SerializeField] List<Wave> waves;
    public MovemntStopAtCollision endPlanetMovement;



    bool isStarted;
    public void StartWave()
    {
        if (isStarted) return;
        isStarted = true;
        StartCoroutine("StartWaveC");
    }
    IEnumerator StartWaveC()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            if (waves[i] == null) continue;

            waves[i].StartWave();
            yield return new WaitUntil(()=>!waves[i].IsRunning);
            
        }

        endPlanetMovement.enabled = true;
    }

	// Use this for initialization
	void Start () {
	    	
	}
	
	// Update is called once per frame
	void Update () {



    }

    public void OnLevelStart()
    {
         StartWave();
    }
}
