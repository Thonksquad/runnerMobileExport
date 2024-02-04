using System.Collections;
using UnityEngine;

public class mobileUiController : MonoBehaviour
{

    public float waitTime = 10f;
    public GameObject shootUi;
    public GameObject flyUi;


    private void Start()
    {
        StartCoroutine(hide());
    }


    private IEnumerator hide()
    {
        yield return new WaitForSecondsRealtime(waitTime);
        shootUi.SetActive(false);
        flyUi.SetActive(false);
    }

}
