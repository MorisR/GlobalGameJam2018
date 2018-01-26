using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerShip))]

public class ShipBlinkAbility : AbiltyBase
{


    Vector2 shadowPosition;
    [Space, Space, Header("Abilty Settings")]
    [SerializeField] float maxFule;
    [SerializeField] float currentFule;

    [Space, Space, Header("Shadow Settings")]
    [SerializeField]
    Transform shadowHolderTransform;
    [SerializeField] Transform shadowSpriteTransform;
    [SerializeField] float shadowDistanceMax;
    [SerializeField] float shadowDistanceScale;
    float shadowDistance = 0f;



    public void ShowShadow()
    {
        shadowHolderTransform.gameObject.SetActive(true);
        float shadowHorizontal = Input.GetAxis("HorizontalShadow");
        float shadowVertical = Input.GetAxis("VerticalShadow");

        shadowPosition.Set(shadowHorizontal, shadowVertical);
        shadowPosition = shadowPosition.normalized;
        shadowPosition.x *= -1;

        shadowHolderTransform.eulerAngles = new Vector3(0, 0, Angle(shadowPosition) + 90f);

        shadowDistance = Mathf.Min(shadowDistanceMax, shadowDistance + shadowDistanceScale);

        shadowSpriteTransform.localPosition = shadowDistance * Vector3.right;



        if (Input.GetButtonDown("Blink"))
           
                Use();

    }

    public void HideShadow()
    {
        shadowHolderTransform.gameObject.SetActive(false);
        shadowDistance = 0f;
    }

    protected override void Start()
    {
        shadowHolderTransform.gameObject.SetActive(false);

        base.Start();
    }
    protected override void Update()
    {
        base.Update();

        if (IsAvailbale && !isPaused)
        {
            if (Input.GetButton("HorizontalShadow") || Input.GetButton("VerticalShadow"))
                ShowShadow();

            else
                HideShadow();
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

    public override void Use()
    {
        base.Use();
        if (!isPaused)
            ship.transform.position = shadowSpriteTransform.position;
    }
}