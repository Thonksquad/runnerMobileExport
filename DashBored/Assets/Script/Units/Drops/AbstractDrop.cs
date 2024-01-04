using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDrop : ScriptableObject
{
    public bool CanPickUp;
    public float _pickupRange;
    public float _pickupSpeed;
    public DropType DropType;
}

public enum DropType
{
    powerup = 0,
    currency = 1
}
