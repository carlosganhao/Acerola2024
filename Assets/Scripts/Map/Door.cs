using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip _openAudio;
    [SerializeField] private AudioClip _closeAudio;
    [SerializeField] private bool _locked;
    [SerializeField] private string _lockedMessage;
    [SerializeField] private QuestStep _unlockStep;
    private Animator _animator;
    private bool _open = false;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        EventBroker.QuestStepActivated += QuestActivatedListener;
    }

    void OnDestroy()
    {
        EventBroker.QuestStepActivated -= QuestActivatedListener;
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
        if(_locked)
        {
            EventBroker.InvokeWriteMessage(_lockedMessage);
            return;
        }

        _animator.SetTrigger("Interact");
        _open = !_open;
    }

    public void PlayDoorOpenAudio() => AudioSource.PlayClipAtPoint(_openAudio, transform.position);
    public void PlayDoorCloseAudio() => AudioSource.PlayClipAtPoint(_closeAudio, transform.position);

    public void QuestActivatedListener(QuestStep step, string message)
    {
        if(step == _unlockStep) _locked = false;
    }
}
