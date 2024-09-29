using UnityEngine;

public class BreakableWall : MonoBehaviour, IDamageable
{
    public void ReceivesHit(int damage = 1)
    {
        Destroy(gameObject);
    }
}
