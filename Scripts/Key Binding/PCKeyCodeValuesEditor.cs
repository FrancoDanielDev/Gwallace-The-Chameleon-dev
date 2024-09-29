#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PCKeyCodeValues))]
public class PCKeyCodeValuesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PCKeyCodeValues editable = (PCKeyCodeValues)target;

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
