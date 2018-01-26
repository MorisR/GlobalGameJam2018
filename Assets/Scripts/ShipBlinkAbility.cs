using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerShip))]

public class ShipBlinkAbility : AbiltyBase {


    Vector2 shadowPosition;
    [Space, Space, Header("Abilty Settings")]
    [SerializeField] Transform movedObject;
    [SerializeField]  float maxFule;
    [SerializeField]  float currentFule;

    [Space,Space,Header("Shadow Settings")]
    [SerializeField] Transform shadowHolderTransform;
    [SerializeField] Transform shadowSpriteTransform;
    [SerializeField] float shadowDistence;



    public void ShowShadow()
    {
        shadowHolderTransform.gameObject.SetActive(true);
            float shadowHorizontal = Input.GetAxis("HorizontalShadow");
            float shadowVertical = Input.GetAxis("VerticalShadow");

            shadowPosition.Set(shadowHorizontal , shadowVertical);
            shadowPosition = shadowPosition.normalized;
        shadowPosition.x *= -1;

        shadowHolderTransform.eulerAngles = new Vector3(0, 0, Angle(shadowPosition)+90f);




        if (Input.GetButtonDown("Blink"))     
            ship.transform.position = shadowSpriteTransform.position;
        
    }

    public void HideShadow()
    {
        shadowHolderTransform.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        shadowHolderTransform.gameObject.SetActive(false);

        shadowSpriteTransform.localPosition = shadowDistence*Vector3.right;
        base.Start();
    }
    protected override void Update()
    {
        base.Update();


        if (IsAvailbale)
        {
            if (Input.GetButton("HorizontalShadow") || Input.GetButton("VerticalShadow"))
                ShowShadow();
        }
        else HideShadow();
        
    }

    public static float Angle(Vector2 p_vector2)
    {
        if (p_vector2.x < 0)
        {
            return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
        }
    }
}
