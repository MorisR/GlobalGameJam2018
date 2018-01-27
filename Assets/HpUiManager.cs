using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpUiManager : Events.Tools.MonoBehaviour_EventManagerBase, Events.Groups.Player.Methods.IOnHit_Int32
{
    [SerializeField]private int currentHpIndex;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void OnHit(int currentPlayerHp)
    {

        spriteRenderer.enabled = currentPlayerHp >= currentHpIndex;

    }
}
