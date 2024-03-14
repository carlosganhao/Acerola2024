using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestArea : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        QuestManager.Instance.QuestGiverTriggered();
    }
}
