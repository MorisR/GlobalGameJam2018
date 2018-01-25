using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbiltyBase : MonoBehaviour {


    [SerializeField] float maxCoolDown;
    [SerializeField] float currentCoolDown;

    public void Use()
    {

    }

    public bool IsAvailbale
    {
        get
        {
            return false;//todo implement
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
