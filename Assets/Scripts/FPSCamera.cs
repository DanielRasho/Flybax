using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Attach to the Main Camera.
/// Automatically positions it at the player's desk with mouse-look (FPS style).
/// No movement — the player is seated.
/// </summary>
public class FPSCamera : MonoBehaviour
{
    [Header("Mouse Look")]
    public float mouseSensitivity = 8f;

    [Header("Vertical Clamp (degrees)")]
    public float minPitch = -30f;
    public float maxPitch =  60f;

    private const float EYE_HEIGHT = 0.85f;

    private float pitch = 0f;
    private float yaw   = 180f;   // empieza mirando hacia la pizarra

    void Start()
    {
        SnapToPlayerSeat();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    void SnapToPlayerSeat()
    {
        GameObject seat = GameObject.FindWithTag("PlayerSeat");

        if (seat != null)
        {
            Vector3 eyePos = seat.transform.position + Vector3.up * EYE_HEIGHT;
            transform.position = eyePos;
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        else
        {
            Debug.LogWarning("FPSCamera: 'PlayerSeat' tag not found. " +
                             "Make sure ClassroomBuilder has run first.");
        }
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Multiplicar por Time.deltaTime para normalizar los píxeles brutos
        yaw   += mouseDelta.x * mouseSensitivity * Time.deltaTime;
        pitch -= mouseDelta.y * mouseSensitivity * Time.deltaTime;
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Escape para soltar el cursor
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
    }
}