using System;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    public List<Target> targets = new List<Target>();

    public Target SelectedTarget { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target)) return;
        if (!targets.Contains(target))
        {
            targets.Add(target);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target)) return;
        if (targets.Contains(target))
        {
            targets.Remove(target);
        }
    }

    public bool SelectTarget()
    {
        if (targets.Count == 0) return false;
        SelectedTarget = targets[0];
        return true;
    }

    public void DeselectTarget() => SelectedTarget = null;
}