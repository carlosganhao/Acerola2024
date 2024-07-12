using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterShootEvent Asset")]
public class CharacterShootEventAsset : ScriptableObject
{
    public void CharacterShootEvent(bool hurtPlayer)
    {
        EventBroker.InvokeCharacterShoot();
        if(hurtPlayer)
            EventBroker.InvokePlayerHealthChanged(-1, 1, false);
    }
}
