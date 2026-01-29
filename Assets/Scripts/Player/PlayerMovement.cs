using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 12f;
    public float gravity = -20f;
    public Transform playerCamera;

    private CharacterController controller;
    private Vector3 velocity;

    // 👇 ВАЖНО
    private bool groundedInitialized = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        velocity = Vector3.zero;
    }

    void Update()
    {
        MovePlayer();
        ApplyGravitySafe();
    }

    private void MovePlayer()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 forward = playerCamera.forward;
        Vector3 right = playerCamera.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * v + right * h;

        if (move.magnitude > 1f)
            move.Normalize();

        controller.Move(move * speed * Time.deltaTime);
    }

    private void ApplyGravitySafe()
    {
        // ❌ пока НЕ стояли на земле — НИКАКОЙ гравитации
        if (!groundedInitialized)
        {
            if (controller.isGrounded)
            {
                groundedInitialized = true;
                velocity.y = -1f;
            }
            return;
        }

        // ✅ обычная гравитация
        if (controller.isGrounded)
        {
            velocity.y = -1f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }
}