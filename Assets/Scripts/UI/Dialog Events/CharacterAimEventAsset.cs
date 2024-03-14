using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAimEvent Asset")]
public class CharacterAimEventAsset : ScriptableObject
{
    public void CharacterAimEvent()
    {
        EventBroker.InvokeCharacterAim();
    }
}
