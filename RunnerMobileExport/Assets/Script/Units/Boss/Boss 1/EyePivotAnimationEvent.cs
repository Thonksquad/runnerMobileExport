using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyePivotAnimationEvent : MonoBehaviour
{
    [SerializeField] BossEye thisParentBossEye;

    public void AnimationCallEyeOpenDone()
    {
        thisParentBossEye.OpeningEyeDoneEnableThisEye();
    }
}
