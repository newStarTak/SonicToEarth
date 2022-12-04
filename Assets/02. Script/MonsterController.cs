using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField]
    private float AISight = 2.5f;
    [SerializeField]
    private float traceSpeed = 0.05f;

    public GameObject player;

    private float toPlayerDistance;
    private Vector3 toPlayerDirection;

    private void FixedUpdate()
    {
        if (OnHit())
            Destroy(gameObject);

        else {
            toPlayerDistance = DistanceToPlayer(player);
            toPlayerDirection = DirectionToPlayer(player);

            if (toPlayerDistance <= AISight) {
                Debug.Log("Attack");
                Debug.Log("Wait");
            }

            else {
                Trace(toPlayerDirection);
            }
        }
    }

    // Decorator [피격 인식]
    private bool OnHit()
    {
        if (GameObject.Find("Trail(Clone)")) {
            float toTrailDistance = Vector3.Distance(GameObject.Find("Trail(Clone)").transform.position, gameObject.transform.position);
            if (toTrailDistance <= gameObject.transform.lossyScale.x / 2) {
                Destroy(gameObject);
                return true;
            }

            else
                return false;
        }

        else {
            return false;
        }
    }

    // Service [Player와의 거리 연산]
    private float DistanceToPlayer(GameObject player)
    {
        return Vector3.Distance(player.transform.position, gameObject.transform.position);
    }

    // Service [Player와의 방향 벡터 연산]
    private Vector3 DirectionToPlayer(GameObject player)
    {
        return (player.transform.position - gameObject.transform.position).normalized;
    }

    // Task [Player를 추격]
    public void Trace(Vector3 traceDirection)
    {
        gameObject.transform.position += traceDirection * traceSpeed;
    }
}