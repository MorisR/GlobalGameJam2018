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

    public void Initliaze(Vector3 direction, float speed)
    {
        this.direction = direction.normalized;
        this.speed = speed;
        movedObject.transform.gameObject.SetActive(true);

    }


    void ResetInstance()
    {
        this.direction = Vector3.zero;
        this.speed = 0;
        movedObject.transform.gameObject.SetActive(false);
        startRemoveInstance = false;
    }
    private bool IsInView(GameObject origin, GameObject toCheck)
    {
        var cam = Camera.main;   
        Vector3 pointOnScreen = cam.WorldToScreenPoint(toCheck.GetComponentInChildren<Renderer>().bounds.center);

        //Is in front
        if (pointOnScreen.z < 0)
        {
            Debug.Log("Behind: " + toCheck.name);
            return false;
        }

        //Is in FOV
        if ((pointOnScreen.x < 0) || (pointOnScreen.x > Screen.width) ||
            (pointOnScreen.y < 0) || (pointOnScreen.y > Screen.height))
        {
            Debug.Log("OutOfBounds: " + toCheck.name);
            return false;
        }

        RaycastHit hit;
        Vector3 heading = toCheck.transform.position - origin.transform.position;
        Vector3 direction = heading.normalized;// / heading.magnitude;

        if (Physics.Linecast(cam.transform.position, toCheck.GetComponentInChildren<Renderer>().bounds.center, out hit))
        {
            if (hit.transform.name != toCheck.name)
            {
                /* -->
                Debug.DrawLine(cam.transform.position, toCheck.GetComponentInChildren<Renderer>().bounds.center, Color.red);
                Debug.LogError(toCheck.name + " occluded by " + hit.transform.name);
                */
                Debug.Log(toCheck.name + " occluded by " + hit.transform.name);
                return false;
            }
        }
        return true;
    }

}
