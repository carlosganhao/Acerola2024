using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShowMouseEvent Asset")]
public class ShowMouseEventAsset : ScriptableObject
{
    public void ShowMouseEvent()
    {
        EventBroker.InvokeShowMouse();
    }
}
