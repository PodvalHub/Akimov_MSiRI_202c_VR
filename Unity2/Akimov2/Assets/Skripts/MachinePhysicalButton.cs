
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MachinePhysicalButton : MonoBehaviour
{
    public enum ButtonType
    {
        Start,
        Stop
    }

    [Header("Настройки кнопки")]
    public ButtonType buttonType;
    public MachinePanel1 panel;

    // Вызывается SteamVR при наведении руки
    private void HandHoverUpdate(Hand hand)
    {
        if (hand == null)
            return;

        // Получаем начало нажатия захвата.
        // В режиме 2D Fallback это также работает от ЛКМ.
        GrabTypes grabType = hand.GetGrabStarting();

        if (grabType != GrabTypes.None)
        {
            Press();
        }
    }

    // Общая логика нажатия
    public void Press()
    {
        if (panel == null)
        {
            Debug.LogError("MachineCanvas не назначен!", this);
            return;
        }

        bool running = buttonType == ButtonType.Start;

        panel.SetRunning(running);

        Debug.Log("Нажата кнопка: " + buttonType);
    }
}
