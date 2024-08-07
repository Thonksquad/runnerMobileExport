using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fart : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        Invoke("DisableThis", 0.5f);
    }

    private void DisableThis()
    {
        gameObject.SetActive(false);
    }
}
