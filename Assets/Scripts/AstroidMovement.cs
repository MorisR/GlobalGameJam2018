using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class AstroidMovement : GameInstance,Events.Groups.Pausable.IAll_Group_Events
{
   [SerializeField] private Transform movedObject;
   //[SerializeField] private bool IsInScene;
   [SerializeField] private Vector3 direction;
   [SerializeField] private float speed;


    //[SerializeField] private SpriteRenderer renderer;
    //[SerializeField] private float RemoveObjectAfterDelay = 0.5f;//remove the object while out of camera range 
     //private float RemoveObjectTimeSample = -0.5f;//remove the object while out of camera range 
   // private bool startRemoveInstance;
   // private Vector3 camPos;
    
    // Use this for initialization
    void Start ()
	{
	    if (movedObject == null)
	        movedObject = transform;
	   // camPos = Camera.main.gameObject.transform.position;
	    float height = Camera.main.orthographicSize * 2.0f;
	    float width = height * Screen.width / Screen.height;
	    
    }
    
    // Update is called once per frame
    public void Initliaze(Vector3 direction, float speed)
    {
        if (isPaused) return;
        
        this.direction = direction.normalized;
        this.speed = speed;
        movedObject.transform.gameObject.SetActive(true);

    }

    void Update () {
	    movedObject.position += Time.deltaTime * speed * direction.normalized;

    //    if(camPos.x)
    }


    public void ResetInstance()
    {
        if (isPaused) return;

        this.direction = Vector3.zero;
        this.speed = 0;
        //  startRemoveInstance = false;
        lastSpeed = 0;
    }

    bool isPaused;
    float lastSpeed;
    public void OnPause()
    {
        lastSpeed = speed;
        speed = 0;
        
    }

    public void OnResume()
    {
        speed = lastSpeed;
    }
}
