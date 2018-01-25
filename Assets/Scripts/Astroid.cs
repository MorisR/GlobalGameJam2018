using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroid :   MonoBehaviour
    ,Events.Groups.AstroidEvents.IAll_Group_Events
{
    [SerializeField] private GameObject instance;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Reset()
    {
     //todo Implement   
    }
}
