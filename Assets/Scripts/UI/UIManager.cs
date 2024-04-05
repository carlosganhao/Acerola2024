using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Transform _healthIndiciatorContainer;
    [SerializeField] private Image _screenCrackPrefab;
    [SerializeField] private List<Sprite> _crackList;
    [SerializeField] private GameObject _gameFinishedUI;
    [SerializeField] private TextMeshProUGUI _objectiveText;
    [SerializeField] private GameObject _speechBubble;
    [SerializeField] private TextMeshProUGUI _speechText;
    [SerializeField] private TextMeshProUGUI _rightParticipantName;
    [SerializeField] private GameObject _rightParticipant => _rightParticipantName.transform.parent.gameObject;
    [SerializeField] private TextMeshProUGUI _leftParticipantName;
    [SerializeField] private GameObject _leftParticipant => _leftParticipantName.transform.parent.gameObject;
    private BaseControls _controls;

    void Awake()
    {
        Instance = this;
        _controls = new BaseControls();
    }

    // Start is called before the first frame update
    void Start()
    {
        _controls.Enable();
        _objectiveText.SetText("");
        _speechBubble.SetActive(false);

        EventBroker.PlayerHealthChanged += HealthChanged;
        EventBroker.QuestStepActivated += QuestActivated;
        EventBroker.QuestStepFulfilled += QuestFulfilled;
        EventBroker.WriteMessage += WriteMessage;
        EventBroker.DialogStarted += WriteDialog;
        EventBroker.GameFinished += GameFinished;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        EventBroker.PlayerHealthChanged -= HealthChanged;
        EventBroker.QuestStepActivated -= QuestActivated;
        EventBroker.QuestStepFulfilled -= QuestFulfilled;
        EventBroker.WriteMessage -= WriteMessage;
    }

    void HealthChanged(int healthChange)
    {
        if(healthChange < 0)
        {
            var crack = Instantiate(_screenCrackPrefab, new Vector3(UnityEngine.Random.value * Screen.width, UnityEngine.Random.value * Screen.height, 0), Quaternion.identity, _healthIndiciatorContainer);
            float value = UnityEngine.Random.value;
            crack.rectTransform.sizeDelta = new Vector2(Mathf.Max(value*500, 100), Mathf.Max(value*500, 100));
            crack.sprite = _crackList[UnityEngine.Random.Range(0, _crackList.Count)];
        }
        else
        {
            _healthIndiciatorContainer.DeleteChild(0);
        }
    }

    void QuestActivated(QuestStep step, string message) => _objectiveText.SetText(message);

    void QuestFulfilled(QuestStep step, string message) => _objectiveText.SetText(message);

    async void WriteMessage(string message)
    {
        _speechBubble.SetActive(true);
        _rightParticipant.SetActive(false);
        _leftParticipant.SetActive(false);
        _speechText.SetText(message);
        await Task.Delay(5000);
        _speechBubble.SetActive(false);
    }

    async void WriteDialog(List<Dialog> dialogs)
    {
        _speechBubble.SetActive(true);
        foreach (var dialog in dialogs)
        {
            SetParticipantName(0, dialog.LeftParticipantName);
            SetParticipantName(1, dialog.RightParticipantName);
            _speechText.SetText(dialog.Text);
            List<Task> tasks = new List<Task>(){Task.Delay(dialog.WaitTime)};
            if(dialog.AllowSkip)
            {
                tasks.Add(AwaitForKey(_controls.PlayerActions.Interact));
            }
            await Task.WhenAny(tasks);
            dialog.EndAction.Invoke();
        }
        _speechBubble.SetActive(false);
        EventBroker.InvokeDialogEnded();
    }

    private Task AwaitForKey(InputAction input)
    {
        var completionSource = new TaskCompletionSource<bool>();
        
        Action<InputAction.CallbackContext> keyPressedDelegate = null;
        keyPressedDelegate = (context) => {
            completionSource.SetResult(true);
            input.performed -= keyPressedDelegate;
        };

        input.performed += keyPressedDelegate;

        return completionSource.Task;
    }

    private void SetParticipantName(int side, string name)
    {
        switch (side)
        {
            case 0:
                _leftParticipant.SetActive(true);
                _leftParticipantName.SetText(name);
                if(name is null || name == string.Empty) _leftParticipant.SetActive(false);
                break;
            case 1:
                _rightParticipant.SetActive(true);
                _rightParticipantName.SetText(name);
                if(name is null || name == string.Empty) _rightParticipant.SetActive(false);
                break;
        }
    }

    private void GameFinished()
    {
        _gameFinishedUI.SetActive(true);
    }
}

[System.Serializable]
public class Dialog
{
    public string LeftParticipantName;
    public string RightParticipantName;
    public string Text;
    public int WaitTime;
    public bool AllowSkip = true;
    public UnityEvent EndAction;
}
