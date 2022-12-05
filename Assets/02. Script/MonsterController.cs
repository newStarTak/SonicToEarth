using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField]
    private float AISight = 2.5f;
    [SerializeField]
    private float traceSpeed = 0.05f;
    [SerializeField]
    private float avoid = 0.3f;

    public GameObject player;

    private float toPlayerDistance;
    private Vector3 toPlayerDirection;

    private bool notRedundantHit = true;

    private void FixedUpdate()
    {
        if (OnHit() && notRedundantHit) {
            RandomHit(avoid);
        }

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

    // Decorator [�ǰ� �ν�]
    private bool OnHit()
    {
        if (GameObject.Find("Trail(Clone)")) {
            float toTrailDistance = Vector3.Distance(GameObject.Find("Trail(Clone)").transform.position, gameObject.transform.position);
            if (toTrailDistance <= gameObject.transform.lossyScale.x / 2) {
                return true;
            }

            notRedundantHit = false;
        }

        return false;
    }

    // Service [Player���� �Ÿ� ����]
    private float DistanceToPlayer(GameObject player)
    {
        return Vector3.Distance(player.transform.position, gameObject.transform.position);
    }

    // Service [Player���� ���� ���� ����]
    private Vector3 DirectionToPlayer(GameObject player)
    {
        return (player.transform.position - gameObject.transform.position).normalized;
    }

    // Task [Player�� �߰�]
    private void Trace(Vector3 traceDirection)
    {
        gameObject.transform.position += traceDirection * traceSpeed;
    }

    // Task [�ǰ� ����]
    private void RandomHit(float avoid)
    {
        notRedundantHit = false;

        float randN = Random.Range(0.0f, 1.0f);

        if(randN < avoid) {
            Debug.Log("Avoidance");
        }

        else {
            Debug.Log("Hit!");
            Destroy(gameObject);
        }
    }
}