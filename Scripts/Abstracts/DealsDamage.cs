using UnityEngine;

public abstract class DealsDamage : MonoBehaviour
{
    [SerializeField] protected int _damage = 1;

    protected virtual void OnTriggerEnter(Collider other)
    {
        IDamageable entity = other.GetComponent<IDamageable>();
        if (entity != null) entity.ReceivesHit(_damage);
    }
}
