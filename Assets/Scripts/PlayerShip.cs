using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : Events.Tools.MonoBehaviour_EventManagerBase ,Events.Groups.Player.Methods.IOnDashAvailable {

    [SerializeField] bool isMovementEnabled;
    [SerializeField] bool isAbillitiesEnabled;
    [SerializeField] int maxHP;
    [SerializeField] int currentHP;
    [SerializeField] float currentTimeSampleIvisibility;
    [SerializeField] float invisibilityDuration;
    [SerializeField] Animator shipAnimator;
    [SerializeField] public Animator hurtAnimator;
    //public AudioSource soundEffect;



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

        if (currentHP > 0)
        {
            currentHP--;
            currentTimeSampleIvisibility = Time.time;
            hurtAnimator.SetTrigger("OnHitTrigger");
            shipAnimator.SetBool("OnHit", true);
            Events.Groups.Player.Invoke.OnHit(currentHP);
        }

        if (currentHP == 0)
        {
            Events.Groups.Player.Invoke.OnDie();

            //todo play palyer die animaation
            shipAnimator.SetTrigger("OnDieTrigger");
            hurtAnimator.SetTrigger("OnDieTrigger");
            //soundEffect.Play();
        }
    }

    // Use this for initialization
    void Start ()
    {
        currentHP = maxHP;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!IsInvonrable)
        {
            shipAnimator.SetBool("OnHit", false);
           // hurtAnimator.SetTrigger("OnHitTrigger");
        }

        if (Input.GetKeyDown((KeyCode.Z)))
	        Events.Groups.Astroid.Invoke.FlyAwayFromPlayer(transform.position,20f);

        if (hurtAnimator.GetCurrentAnimatorStateInfo(0).IsName("Avatar_Dead"))
            LevelManager.Instance.LoadScene("GameOver");
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
        if (collision.tag == "EndPlanet")
            LevelManager.Instance.LoadScene("Credits");

        if (collision.tag == "Pizza")
        {
            Events.Groups.Level.Invoke.OnLevelStart();
            collision.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "EndPlanet")
            LevelManager.Instance.LoadScene("Credits");
        if (collision.gameObject.tag == "Pizza")
        {
            Events.Groups.Level.Invoke.OnLevelStart();
            collision.gameObject.SetActive(false);
        }
    }

    public void OnDashAvailable()
    {
       this.shipAnimator.SetTrigger("IsAbilityReady");
    }
}
