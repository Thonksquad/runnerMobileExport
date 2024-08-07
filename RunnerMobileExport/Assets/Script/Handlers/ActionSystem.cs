using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionSystem
{
    public static Action onPlayerDeath;
    public static Action onPlayerRevive;
    public static Action onPlayerHoundPickup;
   // public static Action<BossHandler> onBossSpawn;
    public static Action<BaseEnemy> onEnemyDeath;
    public static Action<BaseBoss> onBossDeath;
    public static Action<int> onBossTakeDamage;
}
