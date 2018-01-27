using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRotator : Events.Tools.MonoBehaviour_EventManagerBase,Events.Groups.Pausable.IAll_Group_Events
{

    [SerializeField] float rotationSpeed;

    public float RotationSpeed
    {
        get
        {
            return rotationSpeed;
        }

        set
        {
            rotationSpeed = value;
        }
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

	
	// Update is called once per frame
	void Update () {
        if(!isPaused)
        transform.eulerAngles += Vector3.forward * RotationSpeed * Time.deltaTime;
	}



}
