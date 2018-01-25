using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroidMovement : MonoBehaviour {

    [SerializeField] bool isInSecne;
    [SerializeField] Vector2 dirction;
    [SerializeField] float speed;


    public void Initialize(Vector2 dirction, float speed)
    {

    }

    public void ResetInstent()
    {
        speed = 0;
        dirction = Vector2.zero;
        isInSecne = false;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
