using System.Collections.Generic;
using UnityEngine;
using UnityServiceLocator;

public class ParallaxManager : MonoBehaviour
{
    public static ParallaxManager Instance;
    public List<ParallaxSetting> parallaxSettings = new List<ParallaxSetting>();

    private void Awake()
    {
        ServiceLocator.ForSceneOf(this).Register<ParallaxManager>(this);
    }
}

[System.Serializable]
public class ParallaxSetting
{
    public ParallaxLayer ParallaxLayer;
    [Range(0f, 1f)] public float parallaxEffect;
}

public enum ParallaxLayer
{
    BlackBackground=0,
    Volcano=1
}