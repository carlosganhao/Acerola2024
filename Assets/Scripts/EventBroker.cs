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

    public static event Action DetachChaser;

    public static void InvokeDetachChaser()
    {
        if (DetachChaser != null)
            DetachChaser();
    }

    public static event Action<int> PlayerHealthChanged;
    
    public static void InvokePlayerHealthChanged(int healthChange)
    {
        if (PlayerHealthChanged != null)
            PlayerHealthChanged(healthChange);
    }

    public static event Action<int> QuestStepActivated;

    public static void InvokeQuestStepActivated(int step)
    {
        if (QuestStepActivated != null)
            QuestStepActivated(step);
    }

    public static event Action<int> QuestStepFulfilled;

    public static void InvokeQuestStepFulfilled(int step)
    {
        if (QuestStepFulfilled != null)
            QuestStepFulfilled(step);
    }
    #endregion
}
