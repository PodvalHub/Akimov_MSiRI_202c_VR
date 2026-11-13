
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SupportHandwheelController : MonoBehaviour
{
    [Header("Штурвал")]
    public CircularDrive handwheel;

    [Header("Панель станка")]
    public MachinePanel1 machinePanel;

    [Header("Границы перемещения")]
    public float minLocalZ = -0.606f;
    public float maxLocalZ = -0.606f;

    [Header("Настройки движения")]
    public float turnsForFullTravel = 10f;
    public float movementDirection = 1f;

    private float lastAngle;

    private void Start()
    {
        if (handwheel == null)
        {
            Debug.LogError("Не назначен штурвал!");
            enabled = false;
            return;
        }

        lastAngle = handwheel.outAngle;
    }

    private void LateUpdate()
    {
        if (handwheel == null)
            return;

        float currentAngle = handwheel.outAngle;
        float deltaAngle = currentAngle - lastAngle;

        // Обновляем угол даже при остановленном станке,
        // чтобы после запуска не было скачка перемещения.
        lastAngle = currentAngle;

        if (machinePanel == null || !machinePanel.IsRunning)
            return;

        float minZ = Mathf.Min(minLocalZ, maxLocalZ);
        float maxZ = Mathf.Max(minLocalZ, maxLocalZ);
        float travel = maxZ - minZ;

        if (travel <= 0.0001f)
            return;

        float turns = Mathf.Max(0.1f, turnsForFullTravel);
        float distancePerDegree = travel / (360f * turns);

        Vector3 position = transform.localPosition;

        position.z += deltaAngle *
                      distancePerDegree *
                      movementDirection;

        position.z = Mathf.Clamp(position.z, minZ, maxZ);

        transform.localPosition = position;
    }
}
