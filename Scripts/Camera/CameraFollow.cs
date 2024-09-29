using UnityEngine;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _cinemachineCam;

    private void Start()
    {
        if (_cinemachineCam.Follow == null)
        {
            Player player = FindObjectOfType<Player>();

            if (player != null)
            {
                _cinemachineCam.Follow = player.CameraFollow;
            }
            else
            {
                throw new System.NullReferenceException("No player available on scene.");
            }
        }
    }
}
