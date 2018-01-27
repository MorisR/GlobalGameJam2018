using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesManger : MonoBehaviour {

    [SerializeField] List<Wave> waves;
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
            waves[i].StartWave();
            yield return new WaitUntil(()=>!waves[i].IsRunning);
        }
    }

	// Use this for initialization
	void Start () {
	    	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Q)) StartWave();


    }
}
