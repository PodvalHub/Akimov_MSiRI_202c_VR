
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SupportHandwheelController : MonoBehaviour
{
    [Header("Штурвал суппорта")]
    public CircularDrive handwheel;

    [Header("Границы перемещения Support_Linear")]
    public float minLocalZ = -0.606f;
    public float maxLocalZ = -0.606f;

    [Header("Настройки движения")]
    [Tooltip("Количество оборотов штурвала для прохождения всего диапазона")]
    public float turnsForFullTravel = 10f;

    [Tooltip("1 или -1 для изменения направления движения")]
    public float movementDirection = 1f;

    private float lastAngle;

    private void Start()
    {
        if (handwheel == null)
        {
            Debug.LogError("Не назначен штурвал CircularDrive!");
            enabled = false;
            return;
        }

        lastAngle = handwheel.outAngle;

        Debug.Log("Управление суппортом готово");
    }

    private void LateUpdate()
    {
        if (handwheel == null)
            return;

        // Получаем изменение угла штурвала
        float currentAngle = handwheel.outAngle;
        float deltaAngle = currentAngle - lastAngle;

        lastAngle = currentAngle;

        if (Mathf.Approximately(deltaAngle, 0f))
            return;

        // Проверяем диапазон перемещения
        float minZ = Mathf.Min(minLocalZ, maxLocalZ);
        float maxZ = Mathf.Max(minLocalZ, maxLocalZ);

        float travel = maxZ - minZ;

        if (travel <= 0.0001f)
            return;

        // Рассчитываем перемещение за один градус
        float turns = Mathf.Max(0.1f, turnsForFullTravel);

        float distancePerDegree =
            travel / (360f * turns);

        // Перемещаем весь суппорт вдоль локальной оси Z
        Vector3 position = transform.localPosition;

        position.z +=
            deltaAngle * distancePerDegree * movementDirection;

        // Не позволяем суппорту выйти за пределы направляющих
        position.z = Mathf.Clamp(position.z, minZ, maxZ);

        transform.localPosition = position;
    }
}
