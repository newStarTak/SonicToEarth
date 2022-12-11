using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackManager : MonoBehaviour
{
    [SerializeField]
    private float bulletSpeed = 0.002f;

    private Vector3 targetDirection;
    private Transform playerPos;

    private void Start()
    {
        playerPos = GameObject.Find("Player").GetComponent<Transform>();
        targetDirection = playerPos.position - transform.position;
        Destroy(gameObject, 3.0f);
    }

    void Update()
    {
        transform.Translate(targetDirection * bulletSpeed);
    }
}