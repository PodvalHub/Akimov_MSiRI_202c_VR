
using UnityEngine;
using Valve.VR.InteractionSystem;

public class CrossSlideHandwheelController : MonoBehaviour
{
    [Header("Штурвал поперечной подачи")]
    public CircularDrive handwheel;

    [Header("Панель станка")]
    public MachinePanel1 machinePanel;

    [Header("Границы перемещения по X")]
    public float minLocalX = 0.00273f;
    public float maxLocalX = 0.00466f;

    [Header("Настройки движения")]
    public float turnsForFullTravel = 10f;
    public float movementDirection = 1f;

    private float lastAngle;

    private void Start()
    {
        if (handwheel == null)
        {
            Debug.LogError("Не назначен штурвал поперечной подачи!");
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
        lastAngle = currentAngle;

        if (machinePanel == null || !machinePanel.IsRunning)
            return;

        float minX = Mathf.Min(minLocalX, maxLocalX);
        float maxX = Mathf.Max(minLocalX, maxLocalX);

        float travel = maxX - minX;

        if (travel <= 0.000001f)
            return;

        float turns = Mathf.Max(0.1f, turnsForFullTravel);
        float distancePerDegree = travel / (360f * turns);

        Vector3 position = transform.localPosition;

        position.x += deltaAngle * distancePerDegree * movementDirection;
        position.x = Mathf.Clamp(position.x, minX, maxX);

        transform.localPosition = position;
    }
}
