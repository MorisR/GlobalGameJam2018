﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour {

    [SerializeField] bool isMovementEnabled;
    [SerializeField] bool isAbillitiesEnabled;
    [SerializeField] int maxHP;
    [SerializeField] int currentHP;
    [SerializeField] float currentTimeSampleIvisibility;
    [SerializeField] float invisibilityDuration;
    [SerializeField] Animator shipAnimator;
    Color shipInitialColor;

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
        if(IsInvonrable)shipAnimator.SetBool("OnHit",true);

    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if(!IsInvonrable)shipAnimator.SetBool("OnHit",false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag== "Obstacle")
        {
            if (!IsInvonrable)
            { OnHit();
                print("BOOM!!");
            }

        }
    }

}
