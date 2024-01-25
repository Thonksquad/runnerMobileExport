using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class OpenLinks : MonoBehaviour
{
    public static void OpenUrl(string url)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        OpenTab(url);
#endif
    }

#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void OpenTab(string url);
#endif
}
