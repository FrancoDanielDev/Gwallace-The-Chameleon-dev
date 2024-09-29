using System.Collections.Generic;
using UnityEngine;
using System;

public class BindedClueBlock : MonoBehaviour
{
    [SerializeField] private ActionKeybind _actionKey;
    [Space]
    [SerializeField] private ButtonMaterials[] _PCButtonMats;
    [SerializeField] private ButtonMaterials[] _gamepadButtonMats;
    [Space]
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private PCKeyCodeValues _PCKeyCodeValues;
    [SerializeField] private GamepadKeyCodeValues _gamepadKeyCodeValues;

    private bool _keyboard = true;
    private KeyCodeValues _keyValues;
    private Dictionary<string, Material> _myPCMats = new Dictionary<string, Material>();
    private Dictionary<string, Material> _myGamepadMats = new Dictionary<string, Material>();

    [Serializable] private struct ButtonMaterials
    {
        public string name;
        public Material mat;
    }

    private void Awake()
    {
        foreach (ButtonMaterials buttonMaterial in _PCButtonMats)
            _myPCMats[buttonMaterial.name] = buttonMaterial.mat;

        foreach (ButtonMaterials buttonMaterial in _gamepadButtonMats)
            _myGamepadMats[buttonMaterial.name] = buttonMaterial.mat;
    }

    public void KeyboardOn()
    {
        _keyValues = _PCKeyCodeValues;
        _keyboard = true;
        ChangeMat();
    }

    public void GamepadOn()
    {
        _keyValues = _gamepadKeyCodeValues;
        _keyboard = false;
        ChangeMat();
    }

    public void ChangeMat()
    {
        string name = _keyValues.GetData(_actionKey).informalName;
        _mesh.material = _keyboard ? _myPCMats[name] : _myGamepadMats[name];
    }
}
