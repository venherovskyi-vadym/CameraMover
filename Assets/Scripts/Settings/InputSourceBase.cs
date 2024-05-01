using UnityEngine;

public abstract class InputSourceBase : ScriptableObject, IInputSource
{
    public abstract Vector3 GetInput();
}