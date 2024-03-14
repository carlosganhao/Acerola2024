using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Threading.Tasks;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set;}

    [SerializeField] private List<Transform> _questItemsSpawnPoints;
    [SerializeField] private List<GameObject> _questItems;
    [SerializeField] private List<string> _questActivatedMessages;
    [SerializeField] private List<DialogAsset> _questDialog;
    [SerializeField] private CinemachineTargetGroup _followGroup;
    [SerializeField] private CinemachineTargetGroup _targetGroup;
    [SerializeField] private HideProp _hidePropForCutscene;
    [SerializeField] private Transform _followCutsceneCamera;
    [HideInInspector] public QuestStep _questStepFulfilled = 0;
    [HideInInspector] public QuestStep _questStepAvailable = 0;
    private Animator _animator;

    void Awake()
    {
        Instance = this;

        _animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        EventBroker.QuestStepActivated += QuestStepActivated;
        EventBroker.QuestStepFulfilled += QuestStepFulfilled;
        EventBroker.DialogEnded += DialogEnded;
        EventBroker.BlowUpMouse += BlowUpMouseAnimation;
        EventBroker.ShowMouse += ShowMouseAnimation;
        EventBroker.RunAwayPlayer += HidePlayerAnimation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BootstrapSecondPhase()
    {
        _questStepFulfilled = QuestStep.A3;
        _questStepAvailable = QuestStep.B1;
        SpawnQuestItem(_questStepAvailable);
        EventBroker.InvokeQuestStepActivated(QuestStep.A1, "");
        EventBroker.InvokeQuestStepActivated(QuestStep.A2, "");
        EventBroker.InvokeQuestStepActivated(QuestStep.A3, "");
        EventBroker.InvokeQuestStepActivated(_questStepAvailable, _questActivatedMessages[(int)_questStepAvailable]);
    }

    public void QuestGiverTriggered() 
    {
        // Debug.Log($"Quest Giver Triggered {_questStepFulfilled}, {_questStepAvailable}");
        if(_questStepFulfilled != _questStepAvailable) return;

        _questStepAvailable++;
        switch(_questStepAvailable)
        {
            case QuestStep.None:
            case QuestStep.A1:
            case QuestStep.A2:
            case QuestStep.A3:
            case QuestStep.B1:
                // Debug.Log($"A1 - B1");
                _animator.SetTrigger("AnimationIn");
                SpawnQuestItem(_questStepAvailable);
                EventBroker.InvokeDialogStarted(_questDialog[(int)_questStepAvailable].Dialog);
                EventBroker.InvokeQuestStepActivated(_questStepAvailable, _questActivatedMessages[(int)_questStepAvailable]);
                break;
            case QuestStep.B2:
            case QuestStep.B3:
            case QuestStep.B4:
            case QuestStep.B5:
                // Debug.Log($"B2 - B5");
                SpawnQuestItem(_questStepAvailable);
                EventBroker.InvokeQuestStepActivated(_questStepAvailable, _questActivatedMessages[(int)_questStepAvailable]);
                break;
        }
    }

    private void QuestStepActivated(QuestStep step, string message) 
    {
        // Debug.Log($"Quest Activated {step}");
        switch (step)
        {
            case QuestStep.A1:
            case QuestStep.A2:
            case QuestStep.A3:
                _questStepAvailable = step;
                break;
        }
    }

    private void QuestStepFulfilled(QuestStep step, string message)
    {
        // Debug.Log($"Quest Fullfilled {step}");
        switch (step)
        {
            case QuestStep.A1:
            case QuestStep.A2:
            case QuestStep.A3:
                _questStepFulfilled = step;
                break;
            case QuestStep.B1:
            case QuestStep.B2:
            case QuestStep.B3:
            case QuestStep.B4:
                _questStepFulfilled = step;
                _questStepAvailable = step;
                QuestGiverTriggered();
                break;
            case QuestStep.B5:
                EventBroker.InvokeGameFinished();
                break;
        }
    }

    private void SpawnQuestItem(QuestStep step)
    {
        Instantiate(_questItems[(int)step], _questItemsSpawnPoints[(int)step]);
    }

    public void DialogEnded() => _animator.SetTrigger("AnimationOut");
    public void AnimationIn() => EventBroker.InvokeAnimationIn(); 
    public void AnimationOut() => EventBroker.InvokeAnimationOut(); 
    public void SwitchFollowGroup() => SwitchTargetGroup(_followGroup); 
    public void SwitchTargetGroup() => SwitchTargetGroup(_targetGroup);
    public void ShowMouseAnimation() => _animator.SetTrigger("ShowMouse");
    public void BlowUpMouseAnimation() => _animator.SetTrigger("MouseBlowUp");
    public void HidePlayerAnimation() => _animator.SetTrigger("Hide");
    public void Detach() => EventBroker.InvokeDetachChaser();
    public void MoveCharacterAway() => EventBroker.InvokeSoundTriggered(new Vector3(-16, 0, -8));
    public void HidePlayerInProp() => EventBroker.InvokeHidePlayerForCutscene(new Vector3(-32, 1, 12.5f) ,_hidePropForCutscene);
 
    private async void SwitchTargetGroup(CinemachineTargetGroup group)
    {
        while(group.m_Targets[1].weight != 3)
        {
            group.m_Targets[0].weight -= 0.017f;
            group.m_Targets[1].weight += 0.017f;
            await Task.Delay(17);
        }
    }
}

public enum QuestStep
{
    None = 0,
    A1 = 1,
    A2 = 2,
    A3 = 3,
    B1 = 4,
    B2 = 5,
    B3 = 6,
    B4 = 7,
    B5 = 8,
}
