using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour {

    [SerializeField] bool isMovementEnabled;
    [SerializeField] bool isAbillitiesEnabled;
    [SerializeField] int maxHP;
    [SerializeField] int currentHP;
    [SerializeField] float currentTimeSampleIvisibility;
    [SerializeField] float invisibilityDuration;



    public bool IsInvonrable
    {
        get
        {
            return Time.time - currentTimeSampleIvisibility < invisibilityDuration;
        }
    }

    public void UseAbilty(int index)
    {

    }

    public void OnHit()
    {
        currentHP--;
        currentTimeSampleIvisibility = Time.time;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
