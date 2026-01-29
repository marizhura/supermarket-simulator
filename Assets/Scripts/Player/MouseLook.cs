using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;

    private Transform playerBody;   // тело игрока, которое будет вращаться по горизонтали
    private Camera playerCamera;    // камера игрока, для вращения по вертикали

    private float xRotation = 0f;   // текущий угол наклона камеры по вертикали

    void Start()
    {
        // получаем камеру, которая должна быть дочерним объектом Player
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            Debug.LogError("MouseLook: Camera не найдена! Сделайте Main Camera дочерним объектом Player.");
        }

        playerBody = transform; // скрипт висит на Player

        // блокируем курсор в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // получаем движение мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // вращаем камеру по вертикали
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // ограничиваем взгляд вверх/вниз
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // вращаем тело игрока по горизонтали
        playerBody.Rotate(Vector3.up * mouseX);
    }
}

