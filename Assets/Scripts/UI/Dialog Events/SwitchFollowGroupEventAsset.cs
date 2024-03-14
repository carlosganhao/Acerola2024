using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SwitchFollowGroupEvent Asset")]
public class SwitchFollowGroupEventAsset : ScriptableObject
{
    public void SwitchFollowGroupEvent()
    {
        QuestManager.Instance.SwitchFollowGroup();
    }
}