using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovemntStopAtCollision : MonoBehaviour {

    public Vector2 dirction;
    public float speed;
    public string collisionTag;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == collisionTag)
        {
            speed = 0;
           // Events.Groups.Level.Invoke.OnLevelStart();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag== collisionTag)
            speed = 0;
    }






    // Update is called once per frame
    void Update () {

        transform.position += (Vector3)dirction.normalized * speed * Time.deltaTime;

    }




}
