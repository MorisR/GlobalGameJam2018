using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class AstroidMovement : GameInstance
{
   [SerializeField] private Transform movedObject;
   //[SerializeField] private bool IsInScene;
   [SerializeField] private Vector3 direction;
   [SerializeField] private float speed;


    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private float RemoveObjectAfterDelay = 0.5f;//remove the object while out of camera range 
     private float RemoveObjectTimeSample = -0.5f;//remove the object while out of camera range 
    private bool startRemoveInstance;

    private Vector3 camPos;
    private float height;
    private float width;

    // Use this for initialization
    void Start ()
	{
	    if (movedObject == null)
	        movedObject = transform;
	    camPos = Camera.main.gameObject.transform.position;
	     height = Camera.main.orthographicSize * 2.0f;
	     width = height * Screen.width / Screen.height;
	    
    }
	
	// Update is called once per frame
    public void Initliaze(Vector3 direction, float speed)
    {
        this.direction = direction.normalized;
        this.speed = speed;
        movedObject.transform.gameObject.SetActive(true);

    }

    void Update () {
	    movedObject.position += Time.deltaTime * speed * direction.normalized;

	    if (!startRemoveInstance 
            && (Mathf.Abs(transform.position.x - camPos.x) > width / 2
	        || Mathf.Abs(transform.position.y - camPos.y) > height / 2))
	    {
	        startRemoveInstance = true;
	        RemoveObjectTimeSample = Time.time;

	    }


	    if (startRemoveInstance &&Time.time - RemoveObjectTimeSample > RemoveObjectAfterDelay)
	    {
	        ResetInstance();

	    }

    }


    void ResetInstance()
    {
        this.direction = Vector3.zero;
        this.speed = 0;
        movedObject.transform.gameObject.SetActive(false);
        startRemoveInstance = false;
    }
  

}
