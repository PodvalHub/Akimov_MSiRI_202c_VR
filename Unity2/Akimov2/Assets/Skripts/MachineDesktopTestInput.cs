
using System;
using UnityEngine;

public class MachineDesktopTestInput : MonoBehaviour
{
    [Header("Камера игрока")]
    public Camera viewCamera;

    [Header("Настройки")]
    public float maxDistance = 10f;

    [Header("Управление")]
    public KeyCode joystickPressKey = KeyCode.JoystickButton8;
    public KeyCode keyboardTestKey = KeyCode.E;

    private void Awake()
    {
        if (viewCamera == null)
            viewCamera = GetComponent<Camera>();

        if (viewCamera == null)
            viewCamera = Camera.main;
    }

    private void Start()
    {
        Debug.LogWarning("[TEST] DesktopTestInput запущен!");

        if (viewCamera == null)
            Debug.LogWarning("[TEST] ОШИБКА: камера не назначена!");
        else
            Debug.LogWarning("[TEST] Камера: " + viewCamera.name);
    }

    private void Update()
    {
        bool mousePressed = Input.GetMouseButtonDown(0);
        bool joystickPressed = Input.GetKeyDown(joystickPressKey);
        bool keyboardPressed = Input.GetKeyDown(keyboardTestKey);

        if (!mousePressed && !joystickPressed && !keyboardPressed)
            return;

        Debug.LogWarning("[TEST] Нажатие зарегистрировано!");

        if (viewCamera == null)
        {
            Debug.LogWarning("[TEST] Камера отсутствует!");
            return;
        }

        Vector3 screenPosition = mousePressed
            ? Input.mousePosition
            : new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        Ray ray = viewCamera.ScreenPointToRay(screenPosition);

        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            maxDistance,
            ~0,
            QueryTriggerInteraction.Collide
        );

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            MachinePhysicalButton button =
                hit.collider.GetComponentInParent<MachinePhysicalButton>();

            if (button == null)
                continue;

            Debug.LogWarning("[TEST] НАЖАТА КНОПКА: " + button.buttonType);

            button.Press();
            return;
        }

        Debug.LogWarning("[TEST] Кнопка не найдена под прицелом!");
    }
}
