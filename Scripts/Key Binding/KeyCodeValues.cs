using UnityEngine;

public enum ActionKeybind { ActionKeybind, SwitchKeybind, ResetKeybind }

public enum KeybindError { None, Invalid, Occupied }

[System.Serializable]
public struct ActionData
{
    public string informalName;
    public KeyCode keyCode;
}

public abstract class KeyCodeValues : ScriptableObject
{
    [SerializeField] protected MyDictionary<string, ActionData> _inputData;
    [SerializeField] protected string _platform;

    private ActionKeybind[] _allActions = { ActionKeybind.ActionKeybind, ActionKeybind.SwitchKeybind, ActionKeybind.ResetKeybind };

    #region Useful Methods

    public KeybindError CanChangeValue(ActionKeybind actionKeybind, KeyCode keyCode)
    {
        for (int i = 0; i < _allActions.Length; i++)
        {
            if (keyCode == GetData(_allActions[i]).keyCode)
                return KeybindError.Occupied;
        }

        foreach (var data in _inputData)
        {
            if (data.Value.keyCode == keyCode)
            {
                SetActionKeyCode(actionKeybind, keyCode);
                return KeybindError.None;
            }
        }

        return KeybindError.Invalid;
    }

    public ActionData GetData(ActionKeybind actionKeybind)
        => GetData(GameDataManager.instance.GetString(actionKeybind.ToString() + _platform));

    public void SetActionKeyCode(ActionKeybind actionKeybind, KeyCode keyCode)
        => GameDataManager.instance.SetString(actionKeybind.ToString() + _platform, keyCode.ToString());

    private ActionData GetData(string keyCodeString) => _inputData[keyCodeString];

    #endregion

    #region Unity Editor

    public abstract void SetAllInputPossibilities();

    #endregion
}
