using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class DeviceManager : MonoBehaviour
{
    public static DeviceManager instance;

    public bool KeyboardDevice { get; set; } = true;

    [SerializeField] private GameEvent _pcPlatform;
    [SerializeField] private GameEvent _mobilePlatform;
    [Space]
    [SerializeField] private GameEvent _keyboardOn;
    [SerializeField] private GameEvent _gamepadOn;

    private delegate void MyDelegate();
    private MyDelegate _Updating = delegate { };

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitialEvaluation();
    }

    private void Update()
    {
        _Updating();
    }

    private void InitialEvaluation()
    {
        var data = GameDataManager.instance;

        if (!Application.isMobilePlatform)
        {
            _pcPlatform.Raise();
            _Updating += EvaluatePCDevice;

            var deviceData = data.GetString("Device");
            if      (deviceData == "Gamepad")  _gamepadOn.Raise();
            else if (deviceData == "Keyboard") _keyboardOn.Raise();

            #if UNITY_EDITOR
            if (EditorComfort.instance.active) return;
            #endif

            DisableMouse();
        }
        else
        {
            _mobilePlatform.Raise();
        }
    }

    private void EvaluatePCDevice()
    {
        var gamepad = Gamepad.current;
        var keyboard = Keyboard.current;
        var data = GameDataManager.instance;
        var deviceData = data.GetString("Device");
        var gamepadButtonPressed = gamepad != null && gamepad.allControls.Any(x => x.IsPressed());

        if (deviceData != "Gamepad" && gamepad != null && gamepadButtonPressed)
        {
            data.SetString("Device", "Gamepad");
            _gamepadOn.Raise();
        }
        else if (deviceData != "Keyboard" && keyboard != null && keyboard.IsActuated(0))
        {
            data.SetString("Device", "Keyboard");
            _keyboardOn.Raise();
        } 
    }

    public void DisableMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void KeyboardOn()
    {
        KeyboardDevice = true;
    }

    public void GamepadOn()
    {
        KeyboardDevice = false;
    }
}
