using UnityEngine;

public class MyInput : MonoBehaviour
{
    public static MyInput instance;

    [SerializeField] private PCKeyCodeValues _PCKeyCodeValues;
    [SerializeField] private GamepadKeyCodeValues _gamepadKeyCodeValues;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public bool GetKeyDown(ActionKeybind actionKey)
        => Input.GetKeyDown(_PCKeyCodeValues.GetData(actionKey).keyCode)
        || Input.GetKeyDown(_gamepadKeyCodeValues.GetData(actionKey).keyCode);

    public bool GetKey(ActionKeybind actionKey)
        => Input.GetKey(_PCKeyCodeValues.GetData(actionKey).keyCode)
        || Input.GetKey(_gamepadKeyCodeValues.GetData(actionKey).keyCode);

    public bool GetKeyUp(ActionKeybind actionKey)
        => Input.GetKeyUp(_PCKeyCodeValues.GetData(actionKey).keyCode)
        || Input.GetKeyUp(_gamepadKeyCodeValues.GetData(actionKey).keyCode);
}
