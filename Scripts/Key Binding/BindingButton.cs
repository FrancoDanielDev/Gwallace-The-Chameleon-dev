using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BindingButton : MonoBehaviour
{
    [SerializeField] private ActionKeybind _actionKey;
    [Space]
    [SerializeField] private TMP_Text _inputText;
    [SerializeField] private PCKeyCodeValues _pcKeys;
    [SerializeField] private GamepadKeyCodeValues _gamepadKeys;
    [SerializeField] private GameEvent _keyCodeChanged;
    [Space]
    [SerializeField] private GameObject _selectionMenu;
    [SerializeField] private GameObject _pressText;
    [SerializeField] private GameObject _valid;
    [SerializeField] private GameObject _invalid;
    [SerializeField] private GameObject _occupied;
    [SerializeField] private GameObject _returnButton;
    [SerializeField] private Button _thisButton;

    private KeyCodeValues _currentValues;

    private delegate void MyDelegate();
    private MyDelegate _Updating = delegate { };

    private void OnEnable()
    {
        InitialSet();
    }

    private void Update()
    {
        _Updating();
    }

    #region Methods

    private void UpdateInput()
    {
        _inputText.text = _currentValues.GetData(_actionKey).informalName;
    }

    public void CheckInputChange()
    {
        _selectionMenu.SetActive(true);
        _returnButton.SetActive(false);
        _valid.SetActive(false);
        _invalid.SetActive(false);
        _occupied.SetActive(false);
        _pressText.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        this.StartDelayedMethod(() => _Updating = Do, 0.2f);

        void Do()
        {
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    SelectInput(keyCode);
                    _Updating = delegate { };
                    break;
                }
            }
        }
    }

    private void SelectInput(KeyCode keyCode)
    {
        _pressText.SetActive(false);

        var error = _currentValues.CanChangeValue(_actionKey, keyCode);

        switch (error)
        {
            case KeybindError.None:
                UpdateInput();
                _valid.SetActive(true);
                _keyCodeChanged.Raise();
                break;
            case KeybindError.Invalid:
                _invalid.SetActive(true);
                break;
            case KeybindError.Occupied:
                _occupied.SetActive(true);
                break;

            default:
                _invalid.SetActive(true);
                break;
        }

        this.StartDelayedMethod(Do, 1f);

        void Do()
        {
            _selectionMenu.SetActive(false);
            _returnButton.SetActive(true);
            _thisButton.Select();
        }
    }

    #endregion

    #region Device

    public void ChangeToPCValues()
    {
        _currentValues = _pcKeys;
        UpdateInput();
    }

    public void ChangeToGamepadValues()
    {
        _currentValues = _gamepadKeys;
        UpdateInput();
    }

    private void InitialSet()
    {
        if (DeviceManager.instance.KeyboardDevice) ChangeToPCValues();
        else ChangeToGamepadValues();
    }

    #endregion
}
