using UnityEngine;

[CreateAssetMenu]
public class GamepadKeyCodeValues : KeyCodeValues
{
    public override void SetAllInputPossibilities()
    {
        _inputData = new MyDictionary<string, ActionData>
        {
            //{"KeyCode.JoystickButton0", new ActionData { informalName = "A", keyCode = KeyCode.JoystickButton0 }},
            //{"KeyCode.JoystickButton10", new ActionData { informalName = "LT", keyCode = KeyCode.JoystickButton10 }},
            //{"KeyCode.JoystickButton11", new ActionData { informalName = "RT", keyCode = KeyCode.JoystickButton11 }},

            // Existing Ones
            {"JoystickButton1", new ActionData { informalName = "B", keyCode = KeyCode.JoystickButton1 }},
            {"JoystickButton2", new ActionData { informalName = "X", keyCode = KeyCode.JoystickButton2 }},
            {"JoystickButton3", new ActionData { informalName = "Y", keyCode = KeyCode.JoystickButton3 }},
            {"JoystickButton4", new ActionData { informalName = "LB", keyCode = KeyCode.JoystickButton4 }},
            {"JoystickButton5", new ActionData { informalName = "RB", keyCode = KeyCode.JoystickButton5 }},
            {"JoystickButton6", new ActionData { informalName = "View", keyCode = KeyCode.JoystickButton6 }},
        };
    }
}
