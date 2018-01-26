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

    float oldRotationSpeed;

    public void OnPause()
    {
        oldRotationSpeed = rotationSpeed;
        rotationSpeed = 0;
    }

    public void OnResume()
    {
        rotationSpeed = oldRotationSpeed;
        oldRotationSpeed = 0;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.eulerAngles += Vector3.forward * RotationSpeed * Time.deltaTime;
	}



}
