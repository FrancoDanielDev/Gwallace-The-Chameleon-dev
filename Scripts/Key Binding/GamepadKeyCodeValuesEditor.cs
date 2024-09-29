#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GamepadKeyCodeValues))]
public class GamepadKeyCodeValuesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GamepadKeyCodeValues editable = (GamepadKeyCodeValues)target;

        if (GUILayout.Button("Set all Input Possibilities"))
        {
            editable.SetAllInputPossibilities();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(editable);
        }
    }
}

#endif

