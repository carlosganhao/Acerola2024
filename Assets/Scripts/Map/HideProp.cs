using System.Collections;
using System.Collections.Generic;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

public class HideProp : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _hidePosition;
    private Vector3 _previousPosition;

    public void Interact(PlayerController controller)
    {
        if(controller.IsControllingChaser) return;

        if(controller.PropHidingInsideOf is null)
        {
            controller.Hide(this);
            _previousPosition = controller.transform.position;
            controller.transform.position = _hidePosition.position;
        }
        else
        {
            controller.transform.position = _previousPosition;
            controller.Unhide();
        }
    }
}
