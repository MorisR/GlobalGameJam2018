using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRotator : MonoBehaviour {

    [SerializeField] float rotationSpeed;

    public float RotationSpeed
    {
        get
        {
            return rotationSpeed;
        }

        set
        {
            rotationSpeed = value;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.eulerAngles += Vector3.forward * RotationSpeed * Time.deltaTime;
	}
}
