using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SwitchTargetGroupEvent Asset")]
public class SwitchTargetGroupEventAsset : ScriptableObject
{
    public void SwitchTargetGroupEvent()
    {
        QuestManager.Instance.SwitchTargetGroup();
    }
}
