using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour {

    public AudioSource audioClip;
    //public Collider2D spriteCollider;

	// Use this for initialization
	void Start () {
        audioClip.Play();

    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "AstroidDistroyer")
        {
            audioClip.Stop();
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "AstroidDistroyer")
        {
            audioClip.Stop();
        }
        
    }
}
