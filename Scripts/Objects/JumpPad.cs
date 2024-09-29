using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float _bouncePower = 20f;
    [SerializeField] private Animator _animator;

    public void Bounce(GameObject subject)
    {
        _animator.SetTrigger("Bounce");
        AudioManager.instance.Play("Jump Pad");

        Rigidbody subjectRigidbody = subject.GetComponent<Rigidbody>();
        Vector3 currentVelocity = subjectRigidbody.velocity;
        currentVelocity.y = _bouncePower;
        subjectRigidbody.velocity = currentVelocity;
    }
}

