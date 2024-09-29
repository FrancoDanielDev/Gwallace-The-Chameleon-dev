using System.Collections;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public Vector3 LeftWaypoint { get { return _leftWaypoint.position; } }
    public Vector3 RightWaypoint { get { return _rightWaypoint.position; } }

    [SerializeField] private float _rotationSpeed = 260;
    [SerializeField] private float _slothInputBufferPercentage = 0.6f;
    [Space]
    [SerializeField] private Transform _leftWaypoint;
    [SerializeField] private Transform _rightWaypoint;
    [SerializeField] private GameObject _model;
    [SerializeField] private string _workingAudio;

    public void Spin(Player player)
    {
        StartCoroutine(RotateModel());

        IEnumerator RotateModel()
        {
            AudioManager.instance.Play(_workingAudio);
            player.Animator.SetBool("Sliding", true);
            player.Animator.SetBool("Climbing", false);
            player.Movement.spinning = true;
            player.Freeze();
            var oldParent = player.transform.parent;
            player.transform.parent = _model.transform;
            float elapsedTime = 0f;
            float rotationDuration = 180f / _rotationSpeed;
            Quaternion startRotation = _model.transform.rotation;
            Quaternion targetRotation = startRotation * Quaternion.Euler(0, 180, 0);

            while (elapsedTime < rotationDuration)
            {
                if (elapsedTime >= _slothInputBufferPercentage * rotationDuration)
                    player.Movement.JumpCommands();

                _model.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _model.transform.rotation = targetRotation;
            player.transform.parent = oldParent;
            player.Movement.Flip();
            player.Movement.spinning = false;
            player.Unfreeze();
        }
    }
}
