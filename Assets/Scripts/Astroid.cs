using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AstroidMovement))]
public class Astroid :   MonoBehaviour
    ,Events.Groups.Resetable.IAll_Group_Events
{
    [SerializeField] private GameObject instance;
    [SerializeField] private AstroidMovement astroidMovementComponent;

    public AstroidMovement AstroidMovementComponent
    {
        get { return astroidMovementComponent; }
    }

    public GameObject Instance
    {
        get { return instance; }
    }


    // Use this for initialization
	void Start ()
	{
	    astroidMovementComponent = GetComponent<AstroidMovement>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void ResetInstance()
    {
        AstroidMovementComponent.ResetInstance();
        instance.SetActive(false);
    }

    public void InitlialInitliaze(Vector3 movementDirection, float movementSpeed)
    {
        AstroidMovementComponent.Initliaze(movementDirection, movementSpeed);
        instance.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "AstroidDestroyer")
            ResetInstance();
    }





}
