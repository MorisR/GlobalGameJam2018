using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStopAtCollision :Events.Tools.MonoBehaviour_EventManagerBase ,Events.Groups.Pausable.IAll_Group_Events {

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
        if(!isPaused)
            transform.position += (Vector3)dirction.normalized * speed * Time.deltaTime;

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
}
