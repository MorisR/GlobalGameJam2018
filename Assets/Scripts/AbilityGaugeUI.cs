using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGaugeUI : MonoBehaviour {

    [SerializeField]
    SpriteRenderer UI_Sprite;


    [SerializeField]
    AbiltyBase shipAbility;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        UI_Sprite.size = new Vector2(shipAbility.PercentCooldown * 5, UI_Sprite.size.y);
	}
}
