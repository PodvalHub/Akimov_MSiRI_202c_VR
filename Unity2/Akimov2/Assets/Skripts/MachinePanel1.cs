
using UnityEngine;

public class MachinePanel1 : MonoBehaviour
{
    [Header("Текст панели")]
    public TMPro.TMP_Text stateText;
    public TMPro.TMP_Text axesText;
    public TMPro.TMP_Text messageText;

    [Header("Состояние станка")]
    public MachineState currentState;

    // Проверка, запущен ли станок
    public bool IsRunning =>
        currentState != null && currentState.running;

    private void Start()
    {
        if (currentState == null)
        {
            currentState = new MachineState();
        }

        currentState.running = false;

        UpdatePanel();
    }

    public void SetRunning(bool running)
    {
        if (currentState == null)
            currentState = new MachineState();

        currentState.running = running;

        UpdatePanel();
    }

    public void UpdatePanel()
    {
        if (currentState == null)
            return;

        if (stateText != null)
        {
            stateText.text = currentState.running
                ? "РАБОТАЕТ"
                : "ОСТАНОВЛЕН";
        }

        if (axesText != null)
        {
            axesText.text = "X: 0,0 мм\nY: 0,0 мм\nZ: 0,0 мм";
        }

        if (messageText != null)
        {
            messageText.text = currentState.running
                ? "Станок готов к работе"
                : "Нажмите START для запуска";
        }
    }
}
