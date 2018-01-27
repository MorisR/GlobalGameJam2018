using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovment : Events.Tools.MonoBehaviour_EventManagerBase, Events.Groups.Pausable.IAll_Group_Events
{



    [Header("speed properties")]
    [SerializeField] float acceleration;
    [SerializeField] float decelertion;
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] Rigidbody2D movedObject;
    float currnetSpeed;
    Vector2 direction;

    [Space,Space,Header("settings")]
    [SerializeField] bool isMovmentEnabled;

    public void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        direction.Set(horizontal, vertical);
        direction = direction.normalized;
  
        if(Input.GetButton("Horizontal") || Input.GetButton("Vertical" ))
            if (currnetSpeed < maxSpeed)
                currnetSpeed += acceleration;
            else currnetSpeed = maxSpeed;

        else
        {
            if (currnetSpeed > 0)
                currnetSpeed -= decelertion ;

            else currnetSpeed = 0;
        }
        


            movedObject.position += direction* currnetSpeed * Time.deltaTime;

    }




    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(isMovmentEnabled && !isPaused)
        Move();
    
        

	}
    
    float oldSpeed;
    bool isPaused;
    public void OnPause()
    {
        isPaused = true;
    }

    public void OnResume()
    {
        isPaused = false;

    }
}
