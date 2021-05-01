using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClonableSO : ScriptableObject
{
    protected virtual ClonableSO Clone()
    {
        return Instantiate(this);
    }

    public static T Clone<T>(T original) where T : ClonableSO
    {
        return (T)original.Clone();
    }
}