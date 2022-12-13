using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        transform.Translate(targetDirection * bulletSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.collider.tag == "Player")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}