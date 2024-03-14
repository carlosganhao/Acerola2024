using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlowUpMouseEvent Asset")]
public class BlowUpMouseEventAsset : ScriptableObject
{
    public void BlowUpMouseEvent()
    {
        EventBroker.InvokeBlowUpMouse();
    }
}
