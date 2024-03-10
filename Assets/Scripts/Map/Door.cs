using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private Animator _animator;
    private bool _open = false;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        if(!_open)
        {
            _animator.SetTrigger("Interact");
            _open = true;
        }
    }

    public void Interact(PlayerController controller)
    {
        _animator.SetTrigger("Interact");
        _open = !_open;
    }
}
