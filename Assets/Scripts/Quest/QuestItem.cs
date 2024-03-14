using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItem : MonoBehaviour, IInteractable
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private QuestStep _eventStep;
    [SerializeField] private string _eventMessage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
    }

    public void Interact(PlayerController controller)
    {
        EventBroker.InvokeQuestStepFulfilled(_eventStep, _eventMessage);
        Destroy(gameObject);
    }
}
