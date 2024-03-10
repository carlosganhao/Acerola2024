using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static void DeleteChild(this Transform transform, int index)
    {
        GameObject child = transform.GetChild(index).gameObject;
        GameObject.Destroy(child);
    }
}
