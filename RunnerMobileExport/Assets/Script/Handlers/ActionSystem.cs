using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionSystem
{
    public static Action onPlayerHit;
    public static Action onPlayerRestart;
    public static Action onPlayerRecover;
    public static Action onAdRevive;
    public static Action<BaseEnemy> onEnemyDeath;
}
