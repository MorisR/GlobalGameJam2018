using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovment : MonoBehaviour {

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
        /*

            if (currnetSpeed.sqrMagnitude < maxSpeed* maxSpeed)
                currnetSpeed += acceleration * direction*Time.deltaTime;
            else currnetSpeed = direction*maxSpeed;


            if (currnetSpeed.sqrMagnitude > 0)
                currnetSpeed -= decelertion * direction * Time.deltaTime;

            else currnetSpeed =  Vector2.zero;

        */

        if(Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
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
        if(isMovmentEnabled)
        Move();
	}
}
