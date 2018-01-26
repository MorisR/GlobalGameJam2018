using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstentRotition : MonoBehaviour {


   [SerializeField] Vector3 rotition;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.eulerAngles = rotition;
	}
}
