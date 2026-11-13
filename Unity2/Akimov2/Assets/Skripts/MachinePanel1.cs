
using TMPro;
using UnityEngine;

public class MachinePanel1 : MonoBehaviour
{
    [Header("Поля информационной панели")]
    public TMP_Text stateText;
    public TMP_Text axesText;
    public TMP_Text messageText;

    private MachineState currentState;

    private void Start()
    {
        // Начальное состояние станка
        currentState = new MachineState
        {
            running = false,
            x = 0f,
            y = 0f,
            z = 0f
        };

        ShowState(currentState);
        ShowMessage("Система готова к работе");
    }

    // Отображение состояния и координат
    public void ShowState(MachineState data)
    {
        if (data == null)
            return;

        if (stateText != null)
        {
            stateText.text = data.running
                ? "РАБОТАЕТ"
                : "ОСТАНОВЛЕН";
        }

        if (axesText != null)
        {
            axesText.text =
                $"X: {data.x:F1} мм\n" +
                $"Y: {data.y:F1} мм\n" +
                $"Z: {data.z:F1} мм";
        }
    }

    // Отображение сообщений
    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    // Изменение состояния через физические кнопки
    public void SetRunning(bool running)
    {
        if (currentState == null)
        {
            currentState = new MachineState();
        }

        currentState.running = running;

        ShowState(currentState);

        ShowMessage(running
            ? "Станок запущен"
            : "Станок остановлен");
    }
}
