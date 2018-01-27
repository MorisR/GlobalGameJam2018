using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerShip))]
public class AbiltyBase : Events.Tools.MonoBehaviour_EventManagerBase, Events.Groups.Pausable.IAll_Group_Events
{

    [ Header("Cooldown")]

    [SerializeField]  float coolDownDuration;
    float coolDownTimeSimple;
    protected PlayerShip ship;

    private bool isReadyVisualShown;

    public virtual void Use()
    {
        if (IsAvailbale && !isPaused)
            coolDownTimeSimple = Time.time;
    }

    public bool IsAvailbale
    {
        get
        {
            return Time.time - coolDownTimeSimple >= coolDownDuration ;
        }
    }

    public float PercentCooldown
    {
        get
        {
            return Mathf.Min(((Time.time - coolDownTimeSimple) / coolDownDuration), 1f);
        }
    }



    // Use this for initialization
    protected virtual void Start () {
        ship=GetComponent<PlayerShip>();
        coolDownTimeSimple = -coolDownDuration;
    }

    // Update is called once per frame
    protected virtual void Update ()
    {
        if (isPaused)
        {
            coolDownTimeSimple = Time.time - tempTimeSimple;
        }

        if (IsAvailbale && !isReadyVisualShown)
        {
            isReadyVisualShown = true;
            ShowVisuals();
        }

    }

    public virtual void ShowVisuals()
    {
        
    }

    protected bool isPaused;
    float tempTimeSimple;

    public void OnPause()
    {
        isPaused = true;
        tempTimeSimple = Time.time - coolDownTimeSimple;
    }

    public void OnResume()
    {
        isPaused = false;
        //tempTimeSimple = 0;
    }
}
