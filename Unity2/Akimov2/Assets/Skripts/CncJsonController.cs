using System;
using System.Collections;
using System.Globalization;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Клиент CNC-сервера для токарки 16К20.
/// X API  -> PovorotSalazki  (салазки, в сторону детали, X+)
/// Z API  -> Support_Linear  (суппорт, к патрону, Z+)
/// C API  -> вращение Patron + Shpindel (угол в градусах)
/// Y API  -> пока только в лог (на токарке не привязано)
/// </summary>
public class CncJsonController : MonoBehaviour
{
    [Header("Server")]
    public string url = "https://cnc.kovalev.team/get/3";
    public int channel = 1;
    [Tooltip("Пауза между запросами, сек")]
    public float interval = 0.5f;

    [Header("Machine Nodes")]
    [Tooltip("Суппорт: продольное движение (ось Z станка)")]
    public Transform supportNode;
    [Tooltip("Поворотные салазки: поперёк (ось X станка)")]
    public Transform crossSlideNode;
    [Tooltip("Вращающиеся узлы: Patron, Shpindel")]
    public Transform[] spinNodes;

    [Header("Directions (поменяй знак, если едет не туда)")]
    public float xSign = 1f;
    public float zSign = 1f;
    public float spinSign = 1f;

    [Header("Scale")]
    [Tooltip("Мм станка -> метры Unity")]
    public float mmToMeters = 0.001f;
    [Tooltip("Множитель хода салазок (X)")]
    public float xScale = 0.5f;
    [Tooltip("Множитель хода суппорта (Z)")]
    public float zScale = 0.5f;
    [Tooltip("Максимальный ход салазок, м (не вылетит за станок)")]
    public float xMax = 0.1f;
    [Tooltip("Максимальный ход суппорта, м")]
    public float zMax = 0.2f;
    [Tooltip("Скорость сглаживания поступательного движения, м/с")]
    public float moveSpeed = 0.05f;
    [Tooltip("Скорость сглаживания вращения, град/с")]
    public float spinSpeed = 180f;

    private Vector3 startSupport;
    private Vector3 startCross;
    private Quaternion[] startSpin; // мировые вращения
    private Vector3[] startPos;     // мировые позиции
    private Vector3 spinPivot;      // центр вращения (X, Y)

    private float targetX;
    private float targetZ;
    private bool hasTarget;

    private IEnumerator Start()
    {
        if (supportNode == null || crossSlideNode == null)
        {
            Debug.LogError(
                "CncJsonController: назначьте Support и CrossSlide в Inspector."
            );
            yield break;
        }

        startSupport = supportNode.localPosition;
        startCross = crossSlideNode.localPosition;

        if (spinNodes != null)
        {
            startSpin = new Quaternion[spinNodes.Length];
            startPos = new Vector3[spinNodes.Length];

            // Origin в FBX лежит в центре станка, а не в центре
            // патрона. Считаем центр геометрии из bounds и крутим
            // вокруг него.
            Vector3 centerSum = Vector3.zero;
            int count = 0;
            for (int i = 0; i < spinNodes.Length; i++)
            {
                if (spinNodes[i] == null)
                    continue;
                startSpin[i] = spinNodes[i].rotation;
                startPos[i] = spinNodes[i].position;

                Renderer r = spinNodes[i].GetComponentInChildren<Renderer>();
                if (r != null)
                {
                    centerSum += r.bounds.center;
                    count++;
                }
            }
            if (count > 0)
                spinPivot = centerSum / count;

            // Вращение вокруг мировой Z: важны только X и Y центра.
            spinPivot = new Vector3(spinPivot.x, spinPivot.y, 0f);
            Debug.Log("Центр вращения патрона: " + spinPivot);
        }

        Debug.Log("CncJsonController: старт, канал " + channel);

        while (true)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = 10;
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        ApplyData(request.downloadHandler.text);
                    }
                    catch (Exception error)
                    {
                        Debug.LogWarning(
                            "Ошибка чтения JSON: " + error.Message
                        );
                    }
                }
                else
                {
                    Debug.LogWarning(
                        "Ошибка запроса: " + request.error
                    );
                }
            }

            yield return new WaitForSecondsRealtime(
                Mathf.Max(0.1f, interval)
            );
        }
    }

    private void ApplyData(string json)
    {
        JObject data = JObject.Parse(json);

        float x = GetCoordinate(data, "X");
        float z = GetCoordinate(data, "Z");

        targetX = Mathf.Clamp(
            x * mmToMeters * xScale * xSign, -xMax, xMax
        );
        targetZ = Mathf.Clamp(
            z * mmToMeters * zScale * zSign, -zMax, zMax
        );

        // Ось C читаем только для лога: в симуляции она статична,
        // вращение патрона идёт своим плавным ходом (см. Update).
        float angle = GetCoordinate(data, "C", "S");
        hasTarget = true;

        // Лог только при реальном изменении, чтобы не спамить консоль.
        if (Mathf.Abs(x - lastX) > 0.01f ||
            Mathf.Abs(z - lastZ) > 0.01f ||
            Mathf.Abs(angle - lastAngle) > 0.01f)
        {
            lastX = x;
            lastZ = z;
            lastAngle = angle;

            Debug.Log(
                $"Канал {channel} | X = {x:F2} мм | " +
                $"Z = {z:F2} мм | C = {angle:F2}°"
            );
        }
    }

    private float lastX;
    private float lastZ;
    private float lastAngle;
    private float spinAccum; // накопленный угол вращения, град

    private void Update()
    {
        if (!hasTarget)
            return;

        // Поступательное движение со сглаживанием.
        Vector3 sup = supportNode.localPosition;
        supportNode.localPosition = Vector3.MoveTowards(
            sup, startSupport + Vector3.forward * targetZ,
            moveSpeed * Time.deltaTime
        );

        Vector3 cross = crossSlideNode.localPosition;
        crossSlideNode.localPosition = Vector3.MoveTowards(
            cross, startCross + Vector3.right * targetX,
            moveSpeed * Time.deltaTime
        );

        // Плавное непрерывное вращение патрона и шпинделя вокруг
        // МИРОВОЙ оси Z. API отдаёт C статичным, поэтому просто крутим
        // со скоростью spinSpeed (град/с) — без дёрганий и обёрток 360°.
        if (spinNodes != null && startSpin != null)
        {
            spinAccum += spinSpeed * spinSign * Time.deltaTime;
            Quaternion R = Quaternion.Euler(0, 0, spinAccum);

            for (int i = 0; i < spinNodes.Length; i++)
            {
                if (spinNodes[i] != null)
                {
                    spinNodes[i].rotation = R * startSpin[i];
                    spinNodes[i].position =
                        spinPivot + R * (startPos[i] - spinPivot);
                }
            }
        }
    }

    private float GetCoordinate(JObject data, params string[] axisNames)
    {
        foreach (JToken section in data["data"])
        {
            if ((string)section[0] != "Оси Канала " + channel)
                continue;

            foreach (JToken row in section[1])
            {
                string label = (string)row[0];
                foreach (string axis in axisNames)
                {
                    if (label == "Перемещение по оси " + axis)
                    {
                        string text = ((string)row[1])
                            .Replace("мм", "")
                            .Replace("°", "")
                            .Trim()
                            .Replace(',', '.');

                        float value = float.Parse(
                            text, CultureInfo.InvariantCulture
                        );

                        if (float.IsNaN(value) || float.IsInfinity(value))
                            throw new FormatException(
                                "Некорректная координата " + axis
                            );

                        return value;
                    }
                }
            }
        }

        throw new FormatException(
            "Не найдена координата " + string.Join("/", axisNames)
        );
    }
}
