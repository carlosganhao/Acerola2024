using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ExtensionMethods
{
    public static void DeleteChild(this Transform transform, int index)
    {
        GameObject child = transform.GetChild(index).gameObject;
        GameObject.Destroy(child);
    }

    public static bool HasLayer(this LayerMask layerMask, int layer)
    {
        return layerMask == (layerMask | (1 << layer));
    }

    public static void QuitGame()
    {
        #if UNITY_EDITOR
            Debug.Log("Quitting Editor");
            UnityEditor.EditorApplication.ExitPlaymode();
        #else
            Debug.Log("Quitting Application");
            Application.Quit();
        #endif
    }
}

public class WaitUntilFor : CustomYieldInstruction
{
    public override bool keepWaiting => !CheckDone();

    private Func<bool> _predicate;
    private float _timeout;
    private float _elapsedTime = 0;

    public WaitUntilFor(Func<bool> predicate, float timeout)
    {
        this._predicate = predicate;
        this._timeout = timeout;
    }

    public override void Reset()
    {
        _elapsedTime = 0;
    }

    private bool CheckDone()
    {
        bool result = _predicate() || _elapsedTime >= _timeout;
        _elapsedTime += Time.deltaTime;
        return result;
    }
}

public class WaitUntilForRealtime : CustomYieldInstruction
{
    public override bool keepWaiting => !CheckDone();

    private Func<bool> _predicate;
    private float _timeout;
    private float _elapsedTime = 0;

    public WaitUntilForRealtime(Func<bool> predicate, float timeout)
    {
        this._predicate = predicate;
        this._timeout = timeout;
    }

    public override void Reset()
    {
        _elapsedTime = 0;
    }

    private bool CheckDone()
    {
        bool result = _predicate() || _elapsedTime >= _timeout;
        _elapsedTime += Time.unscaledDeltaTime;
        return result;
    }
}