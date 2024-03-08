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
    #endregion
}
