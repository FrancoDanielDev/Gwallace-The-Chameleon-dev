using UnityEngine;

[CreateAssetMenu]
public class PCKeyCodeValues : KeyCodeValues
{
    public override void SetAllInputPossibilities()
    {
        _inputData = new MyDictionary<string, ActionData>
        {
            //{"A", new ActionData { informalName = "A", keyCode = KeyCode.A }},
            //{"D", new ActionData { informalName = "D", keyCode = KeyCode.D }},
            //{"S", new ActionData { informalName = "S", keyCode = KeyCode.S }},
            //{"W", new ActionData { informalName = "W", keyCode = KeyCode.W }},
            //{"Space", new ActionData { informalName = "Space", keyCode = KeyCode.Space }}
            //{"Tab", new ActionData { informalName = "Tab", keyCode = KeyCode.Tab }},

            // Existing Ones
            {"B", new ActionData { informalName = "B", keyCode = KeyCode.B }},
            {"C", new ActionData { informalName = "C", keyCode = KeyCode.C }},
            {"E", new ActionData { informalName = "E", keyCode = KeyCode.E }},
            {"F", new ActionData { informalName = "F", keyCode = KeyCode.F }},
            {"G", new ActionData { informalName = "G", keyCode = KeyCode.G }},
            {"H", new ActionData { informalName = "H", keyCode = KeyCode.H }},
            {"I", new ActionData { informalName = "I", keyCode = KeyCode.I }},
            {"J", new ActionData { informalName = "J", keyCode = KeyCode.J }},
            {"K", new ActionData { informalName = "K", keyCode = KeyCode.K }},
            {"L", new ActionData { informalName = "L", keyCode = KeyCode.L }},
            {"M", new ActionData { informalName = "M", keyCode = KeyCode.M }},
            {"N", new ActionData { informalName = "N", keyCode = KeyCode.N }},
            {"O", new ActionData { informalName = "O", keyCode = KeyCode.O }},
            {"P", new ActionData { informalName = "P", keyCode = KeyCode.P }},
            {"Q", new ActionData { informalName = "Q", keyCode = KeyCode.Q }},
            {"R", new ActionData { informalName = "R", keyCode = KeyCode.R }},
            {"T", new ActionData { informalName = "T", keyCode = KeyCode.T }},
            {"U", new ActionData { informalName = "U", keyCode = KeyCode.U }},
            {"V", new ActionData { informalName = "V", keyCode = KeyCode.V }},
            {"X", new ActionData { informalName = "X", keyCode = KeyCode.X }},
            {"Y", new ActionData { informalName = "Y", keyCode = KeyCode.Y }},
            {"Z", new ActionData { informalName = "Z", keyCode = KeyCode.Z }},
            {"LeftShift", new ActionData { informalName = "Shift", keyCode = KeyCode.LeftShift }},
            {"LeftControl", new ActionData { informalName = "Ctrl", keyCode = KeyCode.LeftControl }},
        };
    }
}
