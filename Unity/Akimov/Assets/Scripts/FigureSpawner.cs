
using UnityEngine;

public class FigureSpawner : MonoBehaviour
{
    // Массив названий фигур
    public string[] figureNames = { "Cube", "Sphere", "Capsule" };

    // Массив количества фигур
    public int[] figureCounts = { 3, 2, 4 };

    void Start()
    {
        // Проверяем соответствие массивов
        if (figureNames.Length != figureCounts.Length)
        {
            Debug.LogError("Ошибка: массивы имеют разную длину!");
            return;
        }

        // Создаём фигуры из массива
        for (int i = 0; i < figureNames.Length; i++)
        {
            for (int j = 0; j < figureCounts[i]; j++)
            {
                GameObject figure = null;

                // Выбираем тип фигуры
                switch (figureNames[i])
                {
                    case "Cube":
                        figure = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        break;

                    case "Sphere":
                        figure = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        break;

                    case "Capsule":
                        figure = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        break;
                }

                if (figure != null)
                {
                    // Задаём название объекта
                    figure.name = figureNames[i] + "_" + (j + 1);

                    // Располагаем фигуры на сцене
                    figure.transform.position = new Vector3(
                        i * 3f,
                        1f,
                        j * 1.5f
                    );

                    // Добавляем физику
                    figure.AddComponent<Rigidbody>();

                    // Добавляем обработчик столкновений
                    figure.AddComponent<CollisionHandler>();

                    Debug.Log("Создана фигура: " + figure.name);
                }
            }
        }
    }
}
