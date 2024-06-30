using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    private BaseControls _controls;
    [SerializeField] private GameObject _zombieContainer;
    [SerializeField] private PlayerController _player;
    [SerializeField] private List<Transform> _spawnPoints;

    private int _phase = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        EventBroker.DetachChaser += DetachChaser;
        EventBroker.GameOver += GameOver;
        _controls = new BaseControls();
        _controls.PlayerActions.Enable();
        _controls.PlayerActions.Escape.performed += Escape;

        #if UNITY_EDITOR
        PlayerPrefs.SetInt("Phase", 0);
        #endif

        if(PlayerPrefs.GetInt("Phase") == 2)
        {
            EventBroker.InvokeDetachChaser();
            _phase = 2;
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
        _phase = 2;
        Destroy(_zombieContainer);
    }

    private void GameOver()
    {
        // Debug.Log("Gameover");
        if(_phase == 0)
        {
            StartCoroutine(DeathAnimation());
            return;
        }
        StartCoroutine(GameOverAnimation());
    }

    private void Escape(InputAction.CallbackContext context)
    {
        Application.Quit();
    }

    private IEnumerator DeathAnimation()
    {
        UIManager.Instance.ShowDeathScreen();
        yield return new WaitUntil(() => Keyboard.current.anyKey.wasPressedThisFrame);
        SceneManager.LoadScene("SampleScene");
    }

    private IEnumerator GameOverAnimation()
    {
        Debug.Log("Stopping Time");
        Time.timeScale = 0.0f;
        // Glitch Effect Here
        UIManager.Instance.GlitchEffect();
        yield return new WaitForSecondsRealtime(2);
        Debug.Log("Showing Game Over Screen");
        UIManager.Instance.ShowGameOverScreen();
        yield return new WaitForSecondsRealtime(1);
        Debug.Log("Quitting Game");
        ExtensionMethods.QuitGame();
    }
}
