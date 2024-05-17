using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseControl : MonoBehaviour
{
    public LeaperEnemy leaper;
    private Coroutine currentRoutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.TryGetComponent<Player>(out Player player))
        {
            currentRoutine = StartCoroutine(StartChaseCounter());
        }
    }

    private void ChasePlayer()
    {
        leaper.chase = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out Player player))
        {
            StopChase();
        }
    }

    public void StopChase()
    {
        leaper.chase = false;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = null;
    }

    private IEnumerator StartChaseCounter()
    {
        yield return new WaitForSecondsRealtime(4f * (1/CameraManager.Instance.CamSpeed));
        ChasePlayer();
    }
}
