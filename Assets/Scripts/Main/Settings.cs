using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultSettings", menuName = "ScriptableObjects/Settings", order = 1)]
public class Settings : ScriptableObject
{
    [SerializeField] private ControlSettings _controls;
    [SerializeField] private RenderingSettings _rendering;

    public ControlSettings GetControlSettings() { return _controls; }

    public RenderingSettings GetRenderingSettings() { return _rendering; }

    public void Save()
    {
        // Todo
    }

    public void Load()
    {
        // Todo
    }

    [Serializable]
    public class ControlSettings
    {

    }
    
    [Serializable]
    public class RenderingSettings
    {
        public RangeDetails rangeDetails;
    }
}