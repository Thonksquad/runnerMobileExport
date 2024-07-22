using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencySO : AbstractDrop
{
    public CurrencyAmount CurrencyAmount;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            Debug.Log("Player pick something up");
        }
    }

}

public enum CurrencyAmount
{
    OneG = 0,
    TenG = 1,
    ThirtyG = 2
}
