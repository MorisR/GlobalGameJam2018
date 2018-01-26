using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerShip))]
public class AbiltyBase : MonoBehaviour {

    [ Header("Cooldown")]

    [SerializeField]  float coolDownDuration;
    float coolDownTimeSimple;
    protected PlayerShip ship;

    public void Use()
    {

        coolDownTimeSimple = Time.time;
    }

    public bool IsAvailbale
    {
        get
        {
            return Time.time - coolDownTimeSimple >= coolDownDuration ;
        }
    }

    // Use this for initialization
    protected virtual void Start () {
        ship=GetComponent<PlayerShip>();
	}

    // Update is called once per frame
    protected virtual void Update () {
		
	}
}
