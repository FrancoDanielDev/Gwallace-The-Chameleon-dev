using UnityEngine;
using UnityEngine.EventSystems;

public class JoyController : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public static JoyController instance;

    private Vector3 _moveDir;
    private Vector2 _initialPos;

    [SerializeField] private Transform _stick = null;
    [SerializeField] private float _maxMagnitude;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        _initialPos = _stick.transform.position;
    }

    public Vector3 GetMovementInput()
    {
        Vector3 correctedPos = new Vector3(_moveDir.x, 0, _moveDir.y);
        correctedPos = correctedPos / _maxMagnitude;
        return correctedPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _moveDir = Vector3.ClampMagnitude(eventData.position - _initialPos, _maxMagnitude);
        _stick.position = (Vector3)_initialPos + _moveDir;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _stick.position = _initialPos;
        _moveDir = Vector3.zero;
    }
}
