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
    [SerializeField] Animator shipAnimator;
    [SerializeField] Animator hurtAnimator;

    Color shipInitialColor;

    public bool IsInvonrable
    {
        get
        {
            return Time.time - currentTimeSampleIvisibility < invisibilityDuration;

        }
    }

    public void OnHit()
    {
        currentHP--;
        Events.Groups.Player.Invoke.OnHit(currentHP);
        currentTimeSampleIvisibility = Time.time;
        hurtAnimator.SetTrigger("OnHitTrigger");
        shipAnimator.SetBool("OnHit", true);
    }

    public void UseAbilty(int index)
    {

    }

    // Use this for initialization
    void Start ()
    {
        currentHP = maxHP;
    }
	
	// Update is called once per frame
	void Update () {
        if (!IsInvonrable)
        {
            shipAnimator.SetBool("OnHit", false);
           // hurtAnimator.SetTrigger("OnHitTrigger");
        }

            if (Input.GetKeyDown((KeyCode.Z)))
	        Events.Groups.Astroid.Invoke.FlyAwayFromPlayer(transform.position,20f);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag== "Obstacle")
        {
            if (!IsInvonrable)
            {
                OnHit();
                print("BOOM!!");
            }

        }
    }

}
