using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RunAwayPlayerEvent Asset")]
public class RunAwayPlayerEventAsset : ScriptableObject
{
    public void RunAwayPlayerEvent()
    {
        EventBroker.InvokeRunAwayPlayer();
    }
}
