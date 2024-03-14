using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _zombieContainer;
    [SerializeField] private PlayerController _player;
    [SerializeField] private List<Transform> _spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        EventBroker.DetachChaser += DetachChaser;
        EventBroker.GameOver += GameOver;

        #if UNITY_EDITOR
        PlayerPrefs.SetInt("Phase", 0);
        #endif

        if(PlayerPrefs.GetInt("Phase") == 2)
        {
            EventBroker.InvokeDetachChaser();
            _player.TakeDamage(1);
            _player.Teleport(_spawnPoints[Random.Range(0, _spawnPoints.Count)].position);
            QuestManager.Instance.BootstrapSecondPhase();
        }
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

    private void DetachChaser()
    {
        PlayerPrefs.SetInt("Phase", 2);
        Destroy(_zombieContainer);
    }

    private void GameOver()
    {
        Debug.Log("Gameover");
        Application.Quit();
    }
}
