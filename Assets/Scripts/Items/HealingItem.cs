using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItem : MonoBehaviour, IInteractable
{
    [SerializeField] private float _rotationSpeed;

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
        controller.Heal(1);
        EventBroker.InvokeItemPickedUp();
        Destroy(gameObject);
    }
}
