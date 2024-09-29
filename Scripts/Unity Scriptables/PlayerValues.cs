using UnityEngine;

[CreateAssetMenu(fileName = "Player Values", menuName = "Player Values/Player Values")]
public class PlayerValues : ScriptableObject
{
    [Header("COMMON VARIABLES")]
    public float formSwitchCooldown = 1f;
    public float movementSpeed = 8f;
    public float jumpForce = 16f;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;
    public float maxGroundDistance = 0.2f;
    public float maxWallDistance = 0.2f;
    public float wallSlidingSpeed = 3.4f;
    public float wallJumpingTime = 0.2f;
    public float wallJumpingDuration = 0.4f;
    public float slothClimbing = 7f;
    public Vector2 wallJumpingPower = new Vector2(10f, 12f);

    [Header("ACCELERATION VARIABLES")]
    public float acceleration = 3f;
    public float deceleration = 6f;

    [Header("SELECTABLE VARIABLES")]
    public LayerMask groundLayer; // Surface & Ground
    public LayerMask wallLayer;   // Surface & Wall & Vine
    public LayerMask vineLayer;   // Vine

    // Y axis Unity Gravity: -30
}
