using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionSystem
{
    public static Action onPlayerHit;
    public static Action onPlayerRecover;
    public static Action onPlayerRevive;
    public static Action<BaseEnemy> onEnemyDeath;
}
