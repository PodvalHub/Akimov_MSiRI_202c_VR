
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SpindleDirectionController : MonoBehaviour
{
    [Header("Рычаг направления")]
    public CircularDrive directionLever;

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
            Debug.LogError(
                "Не назначен рычаг или вращающийся узел шпинделя!",
                this
            );

            enabled = false;
            return;
        }

        // Запоминаем начальное положение рычага как нейтральное.
        neutralAngle = directionLever.outAngle;

        Debug.Log("Контроллер шпинделя готов. Нейтральное положение установлено.");
    }

    private void Update()
    {
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

        // Вращаем шпиндель.
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
                Debug.Log("Шпиндель: нейтраль");
        }
    }
}
