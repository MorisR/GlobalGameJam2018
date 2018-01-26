using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerShip))]
public class AbiltyBase : MonoBehaviour {


    [SerializeField]  float coolDownDuration;
    float coolDownTimeSimple;

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
		
	}

    // Update is called once per frame
    protected virtual void Update () {
		
	}
}
