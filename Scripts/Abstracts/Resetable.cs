using UnityEngine;

public abstract class Resetable : MonoBehaviour
{
    [SerializeField] protected GameObject _model;
    [SerializeField] protected Collider _collider;

    protected virtual void SubscribeToReset()
    {
        EventManager.instance.Subscribe(DoReset, Events.ResetParameters);
    }

    protected virtual void UnsubscribeToReset()
    {
        EventManager.instance.Unsubscribe(DoReset, Events.ResetParameters);
    }

    protected virtual void DoReset()
    {
        UnsubscribeToReset();
        Turn(true);
    }

    protected virtual void Turn(bool on)
    {
        _model.SetActive(on);
        _collider.enabled = on;
    }
}
