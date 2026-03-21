using UnityEngine;
using UnityEngine.InputSystem;

public class FPSCamera : MonoBehaviour
{
    [Header("Mouse Look")]
    public float mouseSensitivity = 8f;

    [Header("Vertical Clamp (degrees)")]
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch =  70f;

    [Header("Picked items")] 
    [SerializeField] private Transform pickedItemPosition;
    public Transform PickedItemPosition => pickedItemPosition;

    private const float EYE_HEIGHT = 0.85f;

    private float pitch;
    private float yaw;

    void Start()
    {
        SnapToPlayer();

        // ✅ Seed pitch/yaw from whatever rotation the camera already has
        // instead of hardcoded defaults — this preserves your editor state.
        pitch = transform.eulerAngles.x;
        yaw   = transform.eulerAngles.y;

        // eulerAngles returns 0–360, but pitch needs to be in the -180–180
        // range so clamping works correctly (e.g. 330° → -30°)
        if (pitch > 180f) pitch -= 360f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    void SnapToPlayer()
    {
        GameObject seat = transform.parent.gameObject;
        if (seat != null)
        {
            Vector3 eyePos = seat.transform.position + Vector3.up * EYE_HEIGHT;
            transform.position = eyePos;
        }
        else
        {
            Debug.LogWarning("FPSCamera: Parent seat not found.");
        }
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yaw   += mouseDelta.x * mouseSensitivity * Time.deltaTime;
        pitch -= mouseDelta.y * mouseSensitivity * Time.deltaTime;
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
    }
}