using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 12f;               // скорость ходьбы
    public float gravity = -9.81f;          // гравитация
    public Transform playerCamera;           // назначь сюда Main Camera

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null)
            Debug.LogError("PlayerMovement: назначьте камеру в инспекторе!");
    }

    void Update()
    {
        MovePlayer();
        ApplyGravity();
    }

    private void MovePlayer()
    {
        // Получаем ввод
        float h = Input.GetAxis("Horizontal"); // A/D
        float v = Input.GetAxis("Vertical");   // W/S

        // Движение относительно камеры
        Vector3 forward = playerCamera.forward;
        Vector3 right = playerCamera.right;

        // убираем вертикальную составляющую
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * v + right * h;

        // Защита от ускорения по диагонали
        if (move.magnitude > 1f)
            move.Normalize();

        // Движение
        controller.Move(move * speed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        // Если игрок на земле, сброс вертикальной скорости
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Применяем гравитацию
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}