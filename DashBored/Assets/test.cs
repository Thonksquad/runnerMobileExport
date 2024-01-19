using System.Collections;
using TMPro;
using UnityEngine;

public class test : MonoBehaviour
{

    public TextMeshProUGUI vcTxt;
    public Color vcMainColor;
    public Color vcSecondColor;
    public float timeToChoose = 3f;
    public int vcMainFontSize = 240;
    public int vcSecondFontSize = 120;
    private bool vcTxtOnMainState = true;


    public void runTime()
    {
        Time.timeScale = 1;
    }
    public void stopTime()
    {
        Time.timeScale = 0;
    }



    public void run()
    {
        StartCoroutine(blink());
    }


    private IEnumerator blink()
    {
        float vcTimer = 5;
        while (vcTimer > 0f)
        {
            yield return new WaitForSeconds(0.5f);
            vcTimer -= 0.5f;
            changeVCtext(vcTimer);
        }

        Debug.Log("blinked for " + 5 + " seconds");
    }


    private void changeVCtext(float t)
    {
        if (vcTxtOnMainState)
        {
            vcTxt.color = vcSecondColor;
            vcTxt.fontSize = vcSecondFontSize;
            vcTxtOnMainState = false;
        }
        else
        {
            vcTxtOnMainState = true;
            vcTxt.color = vcMainColor;
            vcTxt.fontSize = vcMainFontSize;
        }
        vcTxt.text = t.ToString();
    }


}
