using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
        if(Keyboard.current.oKey.wasPressedThisFrame)
        {
            Debug.Log("o Pressed");
            EventBroker.InvokeDetachChaser();
        }
        #endif
    }
}
