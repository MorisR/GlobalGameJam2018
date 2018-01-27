using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstentRotition : MonoBehaviour {


   [SerializeField] Vector3 rotition;
	
	// Update is called once per frame
	void Update () {
        transform.eulerAngles = rotition;
	}
}
