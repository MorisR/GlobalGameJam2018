using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Dimmer_Over_Player : MonoBehaviour {
    [SerializeField]
    SpriteRenderer UI_Sprite;


    Collider2D my_collider;

    // Use this for initialization
    void Start () {
        my_collider = GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            UI_Sprite.color = new Color(UI_Sprite.color.r, UI_Sprite.color.g, UI_Sprite.color.b, 0.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.tag == "Player")
        {
            UI_Sprite.color = new Color(UI_Sprite.color.r, UI_Sprite.color.g, UI_Sprite.color.b);

        }
    }


}
