
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        // Создание персонажа
        Character player = new Character("Воин", 100);

        // Вывод информации
        print(player.GetInfo());

        // Получение урона
        player.TakeDamage(30);
        print("После урона: " + player.GetInfo());

        // Восстанавление здоровья
        player.Heal(20);
        print("После лечения: " + player.GetInfo());

        // Проверка состояния
        print("Мёртв ли персонаж: " + player.IsDead());
    }
}
