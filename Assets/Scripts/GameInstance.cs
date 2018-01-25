using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameInstance : MonoBehaviour
{
    [SerializeField]private Collider2D collider;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {

    }


}
