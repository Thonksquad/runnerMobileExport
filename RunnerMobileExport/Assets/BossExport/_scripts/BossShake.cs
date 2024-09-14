using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShake : MonoBehaviour
{
    internal float shakeIntensity;
    internal float shakeDuration;
    private Vector3 originalPos;

    internal void Do_shake(float intensity, float duration)
    {
        shakeIntensity = intensity;
        shakeDuration = duration;

        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float tempTime = Time.time;
        Vector3 addedPos = new Vector3(0, 0 , transform.localScale.z);
      //  originalPos = transform.localPosition;

        while (tempTime + shakeDuration > Time.time)
        {
            addedPos.x = Random.Range(-shakeIntensity, shakeIntensity);
            addedPos.y = Random.Range(-shakeIntensity, shakeIntensity);
            transform.localPosition = transform.localPosition + addedPos;

            yield return new WaitForSeconds(0.01f);
            //  transform.localPosition = originalPos;
            transform.localPosition = transform.localPosition - addedPos;
            yield return new WaitForSeconds(0.03f);
        }

      //  transform.localPosition = originalPos;
    }
}
