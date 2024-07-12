using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private RawImage _gameView;
    [SerializeField] private Animator _healthScreenAnimator;
    [SerializeField] private Transform _healthIndiciatorContainer;
    [SerializeField] private Image _screenCrackPrefab;
    [SerializeField] private List<Sprite> _crackList;
    [SerializeField] private GameObject _gameOverEarlyUI;
    [SerializeField] private GameObject _gameOverFinalUI;
    [SerializeField] private TextMeshProUGUI _objectiveText;
    [SerializeField] private GameObject _speechBubble;
    [SerializeField] private TextMeshProUGUI _speechText;
    [SerializeField] private TextMeshProUGUI _rightParticipantName;
    [SerializeField] private GameObject _rightParticipant => _rightParticipantName.transform.parent.gameObject;
    [SerializeField] private TextMeshProUGUI _leftParticipantName;
    [SerializeField] private GameObject _leftParticipant => _leftParticipantName.transform.parent.gameObject;
    [SerializeField] private TextMeshProUGUI  _interactionText;
    private BaseControls _controls;
    private Coroutine _messageCoroutine;

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
        _gameView.material.SetFloat("_TexelOffset", 0);
        _gameView.material.SetFloat("_OffsetDirection",0);
        _gameView.material.SetFloat("_Grainyness", 0);

        EventBroker.PlayerHealthChanged += HealthChanged;
        EventBroker.QuestStepActivated += QuestActivated;
        EventBroker.QuestStepFulfilled += QuestFulfilled;
        EventBroker.WriteMessage += InitiateWriteMessage; 
        EventBroker.DialogStarted += InitiateWriteDialog;
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
        EventBroker.WriteMessage -= InitiateWriteMessage;
        EventBroker.DialogStarted -= InitiateWriteDialog;
    }

    void HealthChanged(int healthChange, float percentHealth, bool isControllingChaser)
    {
        if(isControllingChaser)
        {
            _healthScreenAnimator.SetFloat("healthPercentage", percentHealth);
        }
        else
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
    }

    void QuestActivated(QuestStep step, string message) => _objectiveText.SetText(message);

    void QuestFulfilled(QuestStep step, string message) => _objectiveText.SetText(message);

    void InitiateWriteMessage(string message, string leftParticipantName, string rightParticipantName)
    {
        if(_messageCoroutine != null) StopCoroutine(_messageCoroutine);
        _messageCoroutine = StartCoroutine(WriteMessage(message, leftParticipantName, rightParticipantName));
    }

    IEnumerator WriteMessage(string message, string leftParticipantName, string rightParticipantName)
    {
        _speechBubble.SetActive(true);
        _rightParticipant.SetActive(false);
        _leftParticipant.SetActive(false);
        _interactionText.transform.parent.gameObject.SetActive(false);
        _speechText.SetText(message);
        yield return new WaitForSecondsRealtime(5);
        _speechBubble.SetActive(false);
    }

    void InitiateWriteDialog(List<Dialog> dialogs)
    {
        if(_messageCoroutine != null) StopCoroutine(_messageCoroutine);
        _messageCoroutine = StartCoroutine(WriteDialog(dialogs));
    }

    IEnumerator WriteDialog(List<Dialog> dialogs)
    {
        _speechBubble.SetActive(true);
        foreach (var dialog in dialogs)
        {
            SetParticipantName(0, dialog.LeftParticipantName);
            SetParticipantName(1, dialog.RightParticipantName);
            _speechText.SetText(dialog.Text);
            if(dialog.AllowSkip)
            {
                _interactionText.transform.parent.gameObject.SetActive(true);
                yield return new WaitUntilForRealtime(() => _controls.PlayerActions.Interact.WasPressedThisFrame(), dialog.WaitTime);
            }
            else
            {
                _interactionText.transform.parent.gameObject.SetActive(false);
                yield return new WaitForSecondsRealtime(dialog.WaitTime);
            }
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

    public void ShowDeathScreen()
    {
        _gameOverEarlyUI.SetActive(true);
    }

    public void ShowGameOverScreen()
    {
        _gameOverFinalUI.SetActive(true);
    }

    public void GlitchEffect()
    {
        Tween offsetTween = _gameView.material.DOFloat(100, "_TexelOffset", 1.5f);
        var directionTween = _gameView.material.DOFloat(-1, "_OffsetDirection", 1.5f);
        var noiseTween = _gameView.material.DOFloat(0.25f, "_Grainyness", 1.5f);
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
