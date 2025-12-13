using System;
using UnityEngine;

public class Target : MonoBehaviour
{
    public event Action<Target> OnTargetDestroyed;

    private void OnDestroy()
    {
        OnTargetDestroyed?.Invoke(this);
    }
}