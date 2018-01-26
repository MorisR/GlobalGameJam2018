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


        shadowHolderTransform.eulerAngles = new Vector3(0, 0, Vector3.Angle(Vector3.zero, shadowPosition));




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
}
