
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(
            "Столкновение! Объект " + gameObject.name +
            " столкнулся с объектом " + collision.gameObject.name
        );
    }
}
