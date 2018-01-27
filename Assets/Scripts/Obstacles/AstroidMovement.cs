using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class AstroidMovement : Events.Tools.MonoBehaviour_EventManagerBase,Events.Groups.Pausable.IAll_Group_Events
{
   [SerializeField] private Transform movedObject;
   [SerializeField] public Vector3 direction;
   [SerializeField] public float speed;
    
    // Use this for initialization
    void Start ()
	{
	    if (movedObject == null)
	        movedObject = transform;
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
