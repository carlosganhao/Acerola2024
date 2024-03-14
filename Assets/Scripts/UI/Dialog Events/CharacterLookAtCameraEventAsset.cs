using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterLookAtCameraEvent Asset")]
public class CharacterLookAtCameraEventAsset : ScriptableObject
{
    public Vector3 position;
    public void CharacterLookAtCameraEvent()
    {
        EventBroker.InvokeCharacterLookAt(position);
    }
}
