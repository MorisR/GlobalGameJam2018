using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesManger : Events.Tools.MonoBehaviour_EventManagerBase, 
    Events.Groups.Level.Methods.IOnLevelStart,
    Events.Groups.Pausable.IAll_Group_Events

{

    [SerializeField] List<Wave> waves;
    public MoveStopAtCollision endPlanetMovement;



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
            yield return new WaitUntil(()=>!isPaused &&!waves[i].IsRunning);
            
        }

        endPlanetMovement.enabled = true;
    }


	

    public void OnLevelStart()
    {
         StartWave();
    }

    private bool isPaused;


    public void OnPause()
    {
        isPaused = true;
    }

    public void OnResume()
    {
        isPaused = false;
    }
}
