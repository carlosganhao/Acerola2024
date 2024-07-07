using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventBroker
{
    #region Game Events
    public static event Action GameOver;

    public static void InvokeGameOver()
    {
        if (GameOver != null)
            GameOver();
    }

    public static event Action GameFinished;

    public static void InvokeGameFinished()
    {
        if (GameFinished != null)
            GameFinished();
    }

    public static event Action DetachChaser;

    public static void InvokeDetachChaser()
    {
        if (DetachChaser != null)
            DetachChaser();
    }

    public static event Action<int, bool> PlayerHealthChanged;
    
    public static void InvokePlayerHealthChanged(int healthChange, bool isControllingChaser)
    {
        if (PlayerHealthChanged != null)
            PlayerHealthChanged(healthChange, isControllingChaser);
    }

    public static event Action<QuestStep, string> QuestStepActivated;

    public static void InvokeQuestStepActivated(QuestStep step, string message = "")
    {
        if (QuestStepActivated != null)
            QuestStepActivated(step, message);
    }

    public static event Action<QuestStep, string> QuestStepFulfilled;

    public static void InvokeQuestStepFulfilled(QuestStep step, string message = "")
    {
        if (QuestStepFulfilled != null)
            QuestStepFulfilled(step, message);
    }

    public static event Action<string, string, string> WriteMessage;

    public static void InvokeWriteMessage(string message, string leftParticipantName, string rightParticipantName)
    {
        if (WriteMessage != null)
            WriteMessage(message, leftParticipantName, rightParticipantName);
    }

    public static event Action AnimationIn;

    public static void InvokeAnimationIn()
    {
        if (AnimationIn != null)
            AnimationIn();
    }

    public static event Action AnimationOut;

    public static void InvokeAnimationOut()
    {
        if (AnimationOut != null)
            AnimationOut();
    }

    public static event Action<List<Dialog>> DialogStarted;
    
    public static void InvokeDialogStarted(List<Dialog> dialogs)
    {
        if (DialogStarted != null)
            DialogStarted(dialogs);
    }

    public static event Action DialogEnded;
    
    public static void InvokeDialogEnded()
    {
        if (DialogEnded != null)
            DialogEnded();
    }

    public static event Action<Vector3> CharacterLookAt;

    public static void InvokeCharacterLookAt(Vector3 position)
    {
        if (CharacterLookAt != null)
            CharacterLookAt(position);
    }

    public static event Action CharacterAim;

    public static void InvokeCharacterAim()
    {
        if (CharacterAim != null)
            CharacterAim();
    }

    public static event Action CharacterShoot;

    public static void InvokeCharacterShoot()
    {
        if (CharacterShoot != null)
            CharacterShoot();
    }

    public static event Action ShowMouse;

    public static void InvokeShowMouse()
    {
        if (ShowMouse != null)
            ShowMouse();
    }
    
    public static event Action BlowUpMouse;

    public static void InvokeBlowUpMouse()
    {
        if (BlowUpMouse != null)
            BlowUpMouse();
    }
    
    public static event Action RunAwayPlayer;

    public static void InvokeRunAwayPlayer()
    {
        if (RunAwayPlayer != null)
            RunAwayPlayer();
    }
    
    public static event Action<Vector3> SoundTriggered;

    public static void InvokeSoundTriggered(Vector3 position)
    {
        if (SoundTriggered != null)
            SoundTriggered(position);
    }

    public static event Action<Vector3, HideProp> HidePlayerForCutscene;

    public static void InvokeHidePlayerForCutscene(Vector3 position, HideProp prop)
    {
        if (HidePlayerForCutscene != null)
            HidePlayerForCutscene(position, prop);
    }

    public static event Action ItemPickedUp;

    public static void InvokeItemPickedUp()
    {
        if (ItemPickedUp != null)
            ItemPickedUp();
    }

    public static event Action<float> DeafenChaser;

    public static void InvokeDeafenChaser(float duration)
    {
        if (DeafenChaser != null)
            DeafenChaser(duration);
    }

    #endregion
}
