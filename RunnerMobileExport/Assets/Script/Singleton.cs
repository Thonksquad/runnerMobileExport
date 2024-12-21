using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Think T as thing
public abstract class StaticInstance<T> : MonoBehaviour where T : StaticInstance<T>
{
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;
}

public abstract class SingletonPersistent<T> : StaticInstance<T> where T : SingletonPersistent<T>
{
    protected override void Awake()
    {
        if (Instance != null) //If an instance exist 
        {
            Destroy(gameObject); //Destroy this object
        }
        else
        { //If Instance is null aka doesn't exist
            base.Awake();
            DontDestroyOnLoad(gameObject); //When switching scenes don't destroy
        }
    }
}