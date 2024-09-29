using UnityEngine;

public class ClueBlock : MonoBehaviour
{
    [SerializeField] private GameObject _keyboardModel;
    [SerializeField] private GameObject _gamepadModel;

    public void KeyboardOn()
    {
        _keyboardModel.SetActive(true);
        _gamepadModel.SetActive(false);
    }

    public void GamepadOn()
    {
        _keyboardModel.SetActive(false);
        _gamepadModel.SetActive(true);
    }
}
