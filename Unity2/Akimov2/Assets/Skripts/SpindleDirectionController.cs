
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SpindleDirectionController : MonoBehaviour
{
    [Header("Рычаг направления")]
    public CircularDrive directionLever;

    [Header("Панель станка")]
    public MachinePanel1 machinePanel;

    [Header("Вращающийся узел шпинделя")]
    public Transform spindleAssembly;

    [Header("Настройки вращения")]
    public Vector3 spindleLocalAxis = Vector3.up;
    public float spindleRPM = 30f;
    public float neutralDeadZone = 8f;

    [Header("Направление")]
    public bool invertDirection = false;

    private float neutralAngle;
    private int previousDirection = 0;

    private void Start()
    {
        if (directionLever == null || spindleAssembly == null)
        {
            Debug.LogError("Не назначен рычаг или шпиндель!");
            enabled = false;
            return;
        }

        neutralAngle = directionLever.outAngle;
    }

    private void Update()
    {
        if (directionLever == null || spindleAssembly == null)
            return;

        float angleDifference = Mathf.DeltaAngle(
            neutralAngle,
            directionLever.outAngle
        );

        if (invertDirection)
            angleDifference *= -1f;

        int direction = 0;

        if (angleDifference > neutralDeadZone)
            direction = 1;
        else if (angleDifference < -neutralDeadZone)
            direction = -1;

        // При остановленном станке шпиндель не вращается.
        if (machinePanel == null || !machinePanel.IsRunning)
            direction = 0;

        if (direction != 0)
        {
            float degreesPerSecond = spindleRPM * 6f;

            spindleAssembly.Rotate(
                spindleLocalAxis.normalized,
                direction * degreesPerSecond * Time.deltaTime,
                Space.Self
            );
        }

        if (direction != previousDirection)
        {
            previousDirection = direction;

            if (direction == 1)
                Debug.Log("Шпиндель: вращение вперёд");
            else if (direction == -1)
                Debug.Log("Шпиндель: вращение назад");
            else
                Debug.Log("Шпиндель: остановлен");
        }
    }
}
