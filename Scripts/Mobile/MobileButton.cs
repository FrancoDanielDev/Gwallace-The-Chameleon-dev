using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameEvent _buttonPressed;
    [SerializeField] private GameEvent _buttonReleased;
    [Space]
    [SerializeField] private Button _button;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_buttonPressed != null) _buttonPressed.Raise();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_buttonReleased != null) _buttonReleased.Raise();
    }
}
