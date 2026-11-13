using UnityEngine;
using UnityEngine.InputSystem;
using Valve.VR;

// Desktop (без шлема): WASD-ходьба по полу с гравитацией и коллизиями.
// Мышь (правая кнопка) — обзор. В VR (SteamVR активен) скрипт не работает —
// движение и обзор даёт трекинг, поэтому это не конфликтует со сценой.
// Ввод через Input System (проект на нём).
public class DesktopWalk : MonoBehaviour
{
    [Tooltip("Скорость ходьбы, м/с")]
    public float speed = 3f;
    public float gravity = -9.81f;
    [Tooltip("Чувствительность обзора мышью (меньше = медленнее)")]
    public float mouseSensitivity = 1f;

    private CharacterController cc;
    private float verticalVel;
    private float yaw;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        if (cc == null)
            cc = gameObject.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.center = new Vector3(0f, 0.9f, 0f);
        cc.radius = 0.3f;
        yaw = transform.eulerAngles.y;
    }

    private void Update()
    {
        // В VR не трогаем — движение даёт трекинг.
        if (SteamVR.active)
            return;
        if (cc == null || !cc.enabled)
            return;

        var kb = Keyboard.current;
        var mouse = Mouse.current;
        if (kb == null || mouse == null)
            return;

        // Обзор: правая мышь (как в FallbackCameraController).
        if (mouse.rightButton.isPressed)
        {
            var d = mouse.delta.ReadValue();
            yaw += d.x * mouseSensitivity;
            transform.localEulerAngles = new Vector3(0f, yaw, 0f);
            var cam = Camera.main;
            if (cam != null)
            {
                float pitch = cam.transform.localEulerAngles.x - d.y * mouseSensitivity;
                pitch = Mathf.Clamp(pitch, -80f, 80f);
                cam.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
            }
        }

        float h = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float f = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
        Vector3 move = transform.right * h + transform.forward * f;
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        verticalVel += gravity * Time.deltaTime;
        Vector3 vel = move * speed + Vector3.up * verticalVel;

        CollisionFlags flags = cc.Move(vel * Time.deltaTime);
        if ((flags & CollisionFlags.CollidedBelow) != 0)
            verticalVel = 0f;
    }
}

// r 1794515224

// r 1794515261

// r 1794515300
