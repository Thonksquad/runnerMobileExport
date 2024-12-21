using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LoginDataSingleton
{
    public static string WalletAddress { get; set; }
    public static string ShortWallet { get; set; }
    public static string oidcID {get; set; }
    public static string ugsPlayerID { get; set; }
    public static string PlayerAvatarURL { get; set; }
    public static int PlayerCoins { get; set; }
    public static Sprite PlayerSprite { get; set; }
}
